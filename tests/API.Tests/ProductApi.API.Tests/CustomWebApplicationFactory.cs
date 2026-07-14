using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductApi.API.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set test environment
        builder.UseEnvironment("Development");

        // Override configuration for testing
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "TestSuperSecretKeyThatIsAtLeast32CharactersLong!",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["JwtSettings:AccessTokenExpirationMinutes"] = "15"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove SQL Server DbContext registration
            var optionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<Infrastructure.Data.ApplicationDbContext>));
            if (optionsDescriptor != null)
                services.Remove(optionsDescriptor);

            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(Infrastructure.Data.ApplicationDbContext));
            if (contextDescriptor != null)
                services.Remove(contextDescriptor);

            // Add InMemory database
            services.AddDbContext<Infrastructure.Data.ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("ProductApiTestDb");
            });
        });
    }
}
