using Microsoft.EntityFrameworkCore;
using AddressApp.Infrastructure.Data;
using AddressApp.Domain.Interfaces;
using AddressApp.Infrastructure.Repositories;
using AddressApp.Application.Services;
using AddressApp.Application.Mappings;
using FluentValidation;
using AddressApp.Application.Validators;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/addressapp-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// DOCKER İÇİN ÖNEMLİ: Port ayarları
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    serverOptions.ListenAnyIP(int.Parse(port));
});

// Add services
builder.Services.AddControllers();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateAddressDtoValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Address API", Version = "v1" });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IAddressRepository, AddressRepository>();

// Services
builder.Services.AddScoped<IAddressService, AddressService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AddressProfile));

// CORS - ÖNEMLİ: Docker'da çalışması için
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

// Swagger her zaman açık (development ve production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Address API V1");
    c.RoutePrefix = "swagger"; // swagger endpoint
});

// HTTPS Redirect KAPATILDI - Docker HTTP kullanır
// app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Root endpoint ekleyelim
app.MapGet("/", () => Results.Ok(new { 
    message = "Address API is running!", 
    health = "/health",
    swagger = "/swagger"
}));

// Migration kontrolü
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
    
    if (pendingMigrations.Any())
    {
        Log.Information("Applying {Count} pending migrations...", pendingMigrations.Count());
        await dbContext.Database.MigrateAsync();
    }
    else
    {
        Log.Information("Database is up to date");
    }
}
catch (Exception ex)
{
    Log.Warning(ex, "Migration check failed - this is okay if tables already exist");
}

Log.Information("Starting Address API on port {Port}", Environment.GetEnvironmentVariable("PORT") ?? "8080");

app.Run();