using System.Diagnostics.CodeAnalysis;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application;

/// <summary>
/// Dependency Injection configuration for Application layer
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IItemService, ItemService>();

        return services;
    }
}
