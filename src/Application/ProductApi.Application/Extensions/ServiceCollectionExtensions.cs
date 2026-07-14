using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Services;

namespace ProductApi.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register application services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
