using System.Diagnostics.CodeAnalysis;
using CleanArchitecture.Api.Endpoints;
using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

[assembly: ExcludeFromCodeCoverage]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container following hexagonal architecture
builder.Services.AddApplication();      // Application layer

// Skip DbContext registration in test environment (test factory will provide it)
var skipDbContext = builder.Environment.IsEnvironment("Test");
builder.Services.AddInfrastructure(builder.Configuration, skipDbContext);  // Infrastructure layer

// Add API services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply pending migrations automatically (skip for test environment)
if (!app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();

// Map API endpoints
app.MapItemEndpoints();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

await app.RunAsync();