using ProductApi.API.Extensions;
using ProductApi.Infrastructure.Extensions;
using Serilog;

// Configure Serilog from appsettings
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog as the logging provider
    builder.Host.UseSerilog();

    // Add infrastructure layer (DbContext, repositories, UnitOfWork)
    builder.Services.AddInfrastructureLayer(builder.Configuration);

    // Add JWT Authentication
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // Add API layer services (controllers, Swagger, CORS, compression, etc.)
    builder.Services.AddApiLayer(builder.Configuration);

    var app = builder.Build();

    // Configure middleware pipeline
    app.UseApiLayer();

    // HTTPS redirection
    app.UseHttpsRedirection();

    // Authentication & Authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Map controllers
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make Program accessible to integration tests
public partial class Program { }
