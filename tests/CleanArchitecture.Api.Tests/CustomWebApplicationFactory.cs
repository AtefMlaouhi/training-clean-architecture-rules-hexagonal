using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CleanArchitecture.Infrastructure.Database;

namespace CleanArchitecture.Api.Tests;

/// <summary>
/// Custom WebApplicationFactory for testing with in-memory database
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"InMemoryTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set Test environment FIRST so Program.cs skips PostgreSQL registration
        builder.UseEnvironment("Test");

        // Use ConfigureTestServices to ensure this runs AFTER all other service registrations
        builder.ConfigureTestServices(services =>
        {
            // Remove any DbContext registrations (in case they were added)
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.RemoveAll(typeof(DbContextOptions));
            services.RemoveAll(typeof(ApplicationDbContext));

            // Add in-memory database for testing - use instance field so same DB name is used across all requests
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName)
                    .EnableSensitiveDataLogging();
            });
        });
    }
}
