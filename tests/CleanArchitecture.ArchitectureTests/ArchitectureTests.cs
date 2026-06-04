using FluentAssertions;
using NetArchTest.Rules;

namespace CleanArchitecture.ArchitectureTests;
/// <summary>
/// Architecture tests to enforce Clean Architecture principles
/// These tests ensure layer dependencies are correct and naming conventions are followed
/// </summary>
public class ArchitectureTests
{
    #region Layer Dependency Rules

    [Fact]
    public void Domain_ShouldNotHaveDependencyOnOtherLayers()
    {
        // Arrange
        var otherLayers = new[]
        {
            "CleanArchitecture.Application",
            "CleanArchitecture.Infrastructure",
            "CleanArchitecture.Api"
        };

        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Domain)
            .ShouldNot()
            .HaveDependencyOnAny(otherLayers)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain layer should not depend on any other layer");
    }

    [Fact]
    public void Application_ShouldOnlyDependOnDomain()
    {
        // Arrange
        var forbiddenLayers = new[]
        {
            "CleanArchitecture.Infrastructure",
            "CleanArchitecture.Api"
        };

        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Application)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenLayers)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Application layer should only depend on Domain layer");
    }

    [Fact]
    public void Infrastructure_ShouldOnlyDependOnApplicationAndDomain()
    {
        // Arrange
        var forbiddenLayers = new[] { "CleanArchitecture.Api" };

        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Infrastructure)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenLayers)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Infrastructure layer should not depend on Api layer");
    }

    [Fact]
    public void Api_CanDependOnAllLayers()
    {
        // This test documents that API layer is the composition root
        // and can depend on all other layers through its project references
        // Domain reference is transitive through Application and Infrastructure

        // Act - API layer should have references to Application and Infrastructure assemblies
        var apiReferences = ArchitectureTestAssemblies.Api.GetReferencedAssemblies();

        // Assert - Direct references (Domain is transitive)
        apiReferences.Should().Contain(a => a.Name == "CleanArchitecture.Application",
            "API should reference Application assembly");
        apiReferences.Should().Contain(a => a.Name == "CleanArchitecture.Infrastructure",
            "API should reference Infrastructure assembly");
    }

    #endregion

    #region Domain Layer Rules

    [Fact]
    public void Domain_Entities_ShouldBeSealed()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Domain)
            .That()
            .ResideInNamespace("CleanArchitecture.Domain.Entities")
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain entities should be sealed to prevent inheritance");
    }

    [Fact]
    public void Domain_Entities_ShouldInheritFromEntity()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Domain)
            .That()
            .ResideInNamespace("CleanArchitecture.Domain.Entities")
            .Should()
            .Inherit(typeof(CleanArchitecture.Domain.Common.Entity))
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "All domain entities should inherit from Entity base class");
    }

    [Fact]
    public void Domain_ShouldNotHaveDependencyOnEntityFramework()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Domain)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain should not depend on Entity Framework");
    }

    [Fact]
    public void Domain_RepositoryInterfaces_ShouldResideInRepositoriesNamespace()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Domain)
            .That()
            .ResideInNamespace("CleanArchitecture.Domain.Repositories")
            .And()
            .AreInterfaces()
            .Should()
            .HaveNameEndingWith("Repository")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Domain repository interfaces should reside in CleanArchitecture.Domain.Repositories and end with 'Repository'");
    }

    #endregion

    #region Application Layer Rules

    [Fact]
    public void Application_Services_ShouldBeSealed()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Application)
            .That()
            .ResideInNamespace("CleanArchitecture.Application.Services")
            .And()
            .AreClasses()
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Application services should be sealed");
    }

    [Fact]
    public void Application_Services_ShouldHaveNameEndingWithService()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Application)
            .That()
            .ResideInNamespace("CleanArchitecture.Application.Services")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Service")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Service classes should end with 'Service'");
    }

    [Fact]
    public void Application_Interfaces_ShouldStartWithI()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Application)
            .That()
            .ResideInNamespace("CleanArchitecture.Application.Interfaces")
            .And()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Interfaces should start with 'I'");
    }

    [Fact]
    public void Application_ShouldNotHaveDependencyOnEntityFramework()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Application)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Application should not depend on Entity Framework");
    }

    #endregion

    #region Infrastructure Layer Rules

    [Fact]
    public void Infrastructure_Repositories_ShouldBeSealed()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Infrastructure)
            .That()
            .ResideInNamespace("CleanArchitecture.Infrastructure.Repositories")
            .And()
            .AreClasses()
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Repository implementations should be sealed");
    }

    [Fact]
    public void Infrastructure_Repositories_ShouldHaveNameEndingWithRepository()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Infrastructure)
            .That()
            .ResideInNamespace("CleanArchitecture.Infrastructure.Repositories")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Repository")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Repository classes should end with 'Repository'");
    }

    [Fact]
    public void Infrastructure_Configurations_ShouldBeInternal()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Infrastructure)
            .That()
            .ResideInNamespace("CleanArchitecture.Infrastructure.Data.Configurations")
            .Should()
            .NotBePublic()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "EF Core configurations should be internal");
    }

    [Fact]
    public void Infrastructure_Configurations_ShouldBeSealed()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Infrastructure)
            .That()
            .ResideInNamespace("CleanArchitecture.Infrastructure.Data.Configurations")
            .And()
            .AreClasses()
            .Should()
            .BeSealed()
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "EF Core configurations should be sealed");
    }

    #endregion

    #region API Layer Rules

    [Fact]
    public void Api_Endpoints_ShouldBeStaticClasses()
    {
        // Act
        var endpointClasses = Types.InAssembly(ArchitectureTestAssemblies.Api)
            .That()
            .ResideInNamespace("CleanArchitecture.Api.Endpoints")
            .GetTypes()
            .Where(t => t.IsClass);

        // Assert
        endpointClasses.Should().OnlyContain(t => t.IsAbstract && t.IsSealed,
            "Endpoint classes should be static");
    }

    [Fact]
    public void Api_Endpoints_ShouldHaveNameEndingWithEndpoints()
    {
        // Act
        var result = Types.InAssembly(ArchitectureTestAssemblies.Api)
            .That()
            .ResideInNamespace("CleanArchitecture.Api.Endpoints")
            .And()
            .AreClasses()
            .Should()
            .HaveNameEndingWith("Endpoints")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue(
            "Endpoint classes should end with 'Endpoints'");
    }

    #endregion

    #region Naming Convention Rules

    [Fact]
    public void Interfaces_ShouldStartWithI()
    {
        // Act - Check across all assemblies
        var assemblies = new[]
        {
            ArchitectureTestAssemblies.Domain,
            ArchitectureTestAssemblies.Application,
            ArchitectureTestAssemblies.Infrastructure,
            ArchitectureTestAssemblies.Api
        };

        foreach (var assembly in assemblies)
        {
            var result = Types.InAssembly(assembly)
                .That()
                .AreInterfaces()
                .Should()
                .HaveNameStartingWith("I")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                $"All interfaces in {assembly.GetName().Name} should start with 'I'");
        }
    }

    [Fact]
    public void Records_ForModels_ShouldHaveAppropriateNames()
    {
        // Act
        var modelTypes = Types.InAssembly(ArchitectureTestAssemblies.Application)
            .That()
            .ResideInNamespace("CleanArchitecture.Application.Models")
            .GetTypes()
            .Where(t => t.IsClass);

        // Assert
        modelTypes.Should().OnlyContain(
            t => t.Name.EndsWith("Request") || t.Name.EndsWith("Response") || t.Name.EndsWith("Model"),
            "Model types should end with Request, Response, or Model");
    }

    #endregion

    #region Immutability Rules

    [Fact]
    public void Domain_Errors_ShouldBeStaticClasses()
    {
        // Act
        var errorClasses = Types.InAssembly(ArchitectureTestAssemblies.Domain)
            .That()
            .ResideInNamespace("CleanArchitecture.Domain.Errors")
            .GetTypes()
            .Where(t => t.IsClass);

        // Assert
        errorClasses.Should().OnlyContain(t => t.IsAbstract && t.IsSealed,
            "Error classes should be static");
    }

    #endregion
}
