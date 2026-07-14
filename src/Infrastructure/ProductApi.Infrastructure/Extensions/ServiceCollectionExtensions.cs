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
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Register repositories and UnitOfWork
        services.AddScoped(typeof(Application.Interfaces.IRepository<>), typeof(Repository<>));
        services.AddScoped<Application.Interfaces.IProductRepository, ProductRepository>();
        services.AddScoped<Application.Interfaces.IUnitOfWork, UnitOfWork>();

        return services;
    }
}
