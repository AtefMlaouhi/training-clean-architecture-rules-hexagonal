# CleanArchitecture

Production-ready .NET Web API template built with Clean Architecture, DDD principles, and comprehensive automated testing.

## Solution Overview

This repository is organized into four application layers and a broad test matrix:

- Domain: core business rules and entities
- Application: use cases and orchestration
- Infrastructure: persistence and external integrations
- API: Minimal API endpoints and composition root

## Current Solution Structure

```text
CleanArchitecture/
├── CleanArchitecture.slnx
├── Directory.Build.props
├── Directory.Packages.props
├── package.json
├── src/
│   ├── CleanArchitecture.Api/
│   ├── CleanArchitecture.Application/
│   ├── CleanArchitecture.Domain/
│   └── CleanArchitecture.Infrastructure/
├── tests/
│   ├── CleanArchitecture.Api.Tests/
│   ├── CleanArchitecture.Application.Tests/
│   ├── CleanArchitecture.ArchitectureTests/
│   ├── CleanArchitecture.Domain.Tests/
│   └── CleanArchitecture.Infrastructure.Tests/
├── scripts/
├── docs/
└── artifacts/
```

For an expanded breakdown, see `docs/SOLUTION_STRUCTURE.md`.

## Quick Commands

```bash
# Build everything
dotnet build

# Run API
dotnet run --project src/CleanArchitecture.Api

# Run all tests
dotnet test

# Run performance benchmarks
dotnet run -c Release --project tests/CleanArchitecture.PerformanceTests/CleanArchitecture.PerformanceTests.csproj
```

## Husky Rules

Git hooks are managed by Husky and currently enforce branch-name validation at push time.

- Hook file: `.husky/pre-push`
- Rule command: `npx --no-install validate-branch-name`

Detailed behavior, troubleshooting, and customization examples are documented in `docs/HUSKY_RULES.md`.

## Documentation Map

- [Getting started](docs/GETTING_STARTED.md)
- [Quick start](docs/QUICKSTART.md)
- [Solution structure](docs/SOLUTION_STRUCTURE.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Testing strategy](docs/TESTING_STRATEGY.md)
- [Testing summary](docs/TESTING_SUMMARY.md)
- [API guide](docs/API_GUIDE.md)
- [Deployment](docs/DEPLOYMENT.md)
- [ErrorOr guide](docs/ERROROR_GUIDE.md)
- [Husky rules](docs/HUSKY_RULES.md)
- [CI guide](docs/CI_GUIDE.md)

## Contributor

- Atef Mlaouhi ([atef.mlaouhi@outlook.com](mailto:atef.mlaouhi@outlook.com))
