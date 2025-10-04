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

// ============================================
// DATABASE CONFIGURATION
// ============================================

// Önce DATABASE_URL'i kontrol et (Railway production)
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

// Eğer yoksa, appsettings'ten PostgreSQL connection string'i al (yerel Railway test)
var postgresConnectionString = builder.Configuration.GetConnectionString("PostgreSQL");

// SQLite fallback
var sqliteConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=addresses.db";

if (!string.IsNullOrEmpty(databaseUrl))
{
    // PRODUCTION: Railway PostgreSQL (DATABASE_URL env variable)
    Log.Information("🐘 Using PostgreSQL from DATABASE_URL (Railway Production)");
    
    var connectionString = ConvertDatabaseUrl(databaseUrl);
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else if (!string.IsNullOrEmpty(postgresConnectionString))
{
    // LOCAL: Railway PostgreSQL (appsettings.json'dan)
    Log.Information("🐘 Using PostgreSQL from appsettings (Local Railway Test)");
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(postgresConnectionString));
}
else
{
    // LOCAL: SQLite (fallback)
    Log.Information("📁 Using SQLite (Local Development)");
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(sqliteConnectionString));
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
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-migrate
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Log.Information("🔄 Applying database migrations...");
        
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            Log.Information($"📋 Found {pendingMigrations.Count()} pending migration(s)");
            await dbContext.Database.MigrateAsync();
            Log.Information("✅ Migrations applied successfully!");
        }
        else
        {
            Log.Information("✅ Database is up to date!");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ Database migration failed");
        throw; // Production'da uygulama başlamasın
    }
}

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Address API v1");
    c.RoutePrefix = string.Empty; // Swagger root'ta açılsın
});

app.UseSerilogRequestLogging();

// CORS - Use BEFORE other middleware
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();

// Port configuration
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

Log.Information($"🚀 Application starting on port {port}");
Log.Information($"🌍 Environment: {app.Environment.EnvironmentName}");

app.Run();

// ============================================
// HELPER METHOD
// ============================================
static string ConvertDatabaseUrl(string databaseUrl)
{
    try
    {
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');
        
        var connectionString = $"Host={databaseUri.Host};" +
                              $"Port={databaseUri.Port};" +
                              $"Database={databaseUri.LocalPath.TrimStart('/')};" +
                              $"Username={userInfo[0]};" +
                              $"Password={userInfo[1]};" +
                              $"SSL Mode=Require;" +
                              $"Trust Server Certificate=true";
        
        Log.Information($"✅ Converted DATABASE_URL to connection string");
        return connectionString;
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ Failed to parse DATABASE_URL");
        throw;
    }
}