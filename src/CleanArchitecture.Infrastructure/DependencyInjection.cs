using System.Diagnostics.CodeAnalysis;
using CleanArchitecture.Domain.Repositories;
using CleanArchitecture.Infrastructure.Database;
using CleanArchitecture.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure;

/// <summary>
/// Dependency Injection configuration for Infrastructure layer
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool skipDbContext = false)
    {
        // Add DbContext with PostgreSQL (unless skipped for testing)
        if (!skipDbContext)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        // Register repositories
        services.AddScoped<IItemRepository, ItemRepository>();

        return services;
    }
}
