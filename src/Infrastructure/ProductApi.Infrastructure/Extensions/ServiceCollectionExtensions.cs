using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Data.Repositories;

namespace ProductApi.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var provider = configuration["DatabaseProvider"] ?? "SqlServer";

        if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = configuration.GetConnectionString("SqliteConnection")
                ?? "Data Source=ProductApi.db";
            var migrationsAssembly = typeof(ServiceCollectionExtensions).Assembly.FullName;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString, b =>
                    b.MigrationsAssembly(migrationsAssembly)));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is required for SQL Server.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, b =>
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        // Register repositories and UnitOfWork
        services.AddScoped(typeof(Application.Interfaces.IRepository<>), typeof(Repository<>));
        services.AddScoped<Application.Interfaces.IProductRepository, ProductRepository>();
        services.AddScoped<Application.Interfaces.IUnitOfWork, UnitOfWork>();

        return services;
    }
}
