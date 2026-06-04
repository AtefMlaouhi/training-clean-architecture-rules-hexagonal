using System.Reflection;
using CleanArchitecture.Application.Markers;
using CleanArchitecture.Domain.Markers;
using CleanArchitecture.Infrastructure.Markers;
using CleanArchitecture.Api.Markers;

namespace CleanArchitecture.ArchitectureTests;

public static class ArchitectureTestAssemblies
{
    public static readonly Assembly Domain = typeof(IAssemblyMarkerDomain).Assembly;
    public static readonly Assembly Application = typeof(IAssemblyMarkerApplication).Assembly;
    public static readonly Assembly Infrastructure = typeof(IAssemblyMarkerInfrastructure).Assembly;
    public static readonly Assembly Api = typeof(IAssemblyMarkerApi).Assembly;
}