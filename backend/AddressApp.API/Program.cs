using Microsoft.EntityFrameworkCore;
using AddressApp.Application.Interfaces;
using AddressApp.Application.Services;
using AddressApp.Application.Mappings;
using AddressApp.Core.Interfaces;
using AddressApp.Infrastructure.Data;
using AddressApp.Infrastructure.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Address Management API", 
        Version = "v1",
        Description = "REST API for managing delivery addresses"
    });
});

// DATABASE CONFIGURATION
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // RAILWAY POSTGRESQL
    Log.Information("🐘 Using PostgreSQL (Railway)");
    
    var databaseUri = new Uri(databaseUrl);
    var userInfo = databaseUri.UserInfo.Split(':');
    
    var connectionString = $"Host={databaseUri.Host};" +
                          $"Port={databaseUri.Port};" +
                          $"Database={databaseUri.LocalPath.TrimStart('/')};" +
                          $"Username={userInfo[0]};" +
                          $"Password={userInfo[1]};" +
                          $"SSL Mode=Require;" +
                          $"Trust Server Certificate=true";
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // LOCAL SQLITE
    Log.Information("📁 Using SQLite (Local Development)");
    
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=addresses.db";
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(connectionString));
}

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Services
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressParserService, AddressParserService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()  // Railway deployment için geçici olarak tüm origin'lere izin
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Log.Information("🔄 Applying database migrations...");
        dbContext.Database.Migrate();
        Log.Information("✅ Database ready!");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ Database migration failed");
    }
}

// Swagger (always enabled for demo)
app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Address API v1");
    c.RoutePrefix = string.Empty; // Swagger on root URL
});

app.UseSerilogRequestLogging();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

// Railway uses PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "5257";
app.Urls.Add($"http://0.0.0.0:{port}");

Log.Information($"🚀 Application starting on port {port}");
app.Run();