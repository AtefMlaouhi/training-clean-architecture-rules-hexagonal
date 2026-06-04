# Getting Started

Use this guide to set up, run, and validate the solution quickly.

## Prerequisites

- .NET 10 SDK
- PostgreSQL (for API and integration scenarios)
- Docker Desktop (required for container-based integration scenarios)
- Node.js and npm (for Husky hook installation)

## Initial Setup

```bash
# Restore .NET dependencies
dotnet restore

# Build all projects
dotnet build

# Install Husky hooks
npm install
```

## Run the API

```bash
dotnet run --project src/CleanArchitecture.Api
```

Typical endpoints:

- API root: `https://localhost:5214`
- Health: `https://localhost:5214/health`
- OpenAPI: `https://localhost:5214/openapi/v1.json`

## Run Tests

```bash
# All test projects
dotnet test

# Skip integration tests when Docker is unavailable
dotnet test --filter "FullyQualifiedName!~IntegrationTests"
```

## Run Performance Benchmarks

```bash
# Run benchmark suites
dotnet run -c Release --project tests/CleanArchitecture.PerformanceTests/CleanArchitecture.PerformanceTests.csproj

# List benchmark names
dotnet run -c Release --project tests/CleanArchitecture.PerformanceTests/CleanArchitecture.PerformanceTests.csproj -- --list flat
```

## Solution Structure (At a Glance)

```text
src/
├── CleanArchitecture.Domain/
├── CleanArchitecture.Application/
├── CleanArchitecture.Infrastructure/
└── CleanArchitecture.Api/

tests/
├── CleanArchitecture.Domain.Tests/
├── CleanArchitecture.Application.Tests/
├── CleanArchitecture.Infrastructure.Tests/
├── CleanArchitecture.Api.Tests/
├── CleanArchitecture.IntegrationTests/
└── CleanArchitecture.ArchitectureTests/
```

For a complete tree and ownership details, see `docs/SOLUTION_STRUCTURE.md`.

## Husky Rules

Husky is used for Git hook enforcement.

- Hook installer: `npm run prepare`
- Active hook: `.husky/pre-push`
- Current rule: validate branch name before push via `validate-branch-name`

Details, troubleshooting, and extension patterns are in `docs/HUSKY_RULES.md`.

## Next Documentation

- Architecture details: `docs/ARCHITECTURE.md`
- Testing strategy: `docs/TESTING_STRATEGY.md`
- API details: `docs/API_GUIDE.md`
