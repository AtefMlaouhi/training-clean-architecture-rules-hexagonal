# Solution Structure

This document describes the current structure of the CleanArchitecture solution and how each area is intended to be used.

## Top-Level Layout

```text
CleanArchitecture/
├── CleanArchitecture.slnx                 # Solution entry
├── Directory.Build.props                  # Shared MSBuild config
├── Directory.Packages.props               # Central package versions
├── package.json                           # Husky and JS tooling metadata
├── .husky/
│   └── pre-push                           # Branch-name validation hook
├── src/                                   # Production code
├── tests/                                 # Automated test suites
├── docs/                                  # Project documentation
├── scripts/                               # Migration and database helper scripts
├── artifacts/                             # Reports and generated artifacts
└── TestResults/                           # Test and coverage outputs
```

## Source Projects

```text
src/
├── CleanArchitecture.Domain/              # Entities, errors, repository contracts
├── CleanArchitecture.Application/         # Use cases, services, request/response models
├── CleanArchitecture.Infrastructure/      # Database, repositories, infrastructure DI
└── CleanArchitecture.Api/                 # Minimal API endpoints and startup
```

### Layer Dependency Rule

- Domain: no dependencies on outer layers
- Application: depends on Domain only
- Infrastructure: depends on Domain and Application
- API: composition root; wires all layers together

## Test Projects

```text
tests/
├── CleanArchitecture.Domain.Tests/
├── CleanArchitecture.Application.Tests/
├── CleanArchitecture.Infrastructure.Tests/
├── CleanArchitecture.Api.Tests/
├── CleanArchitecture.IntegrationTests/
├── CleanArchitecture.ArchitectureTests/
├── CleanArchitecture.AcceptanceTests/
├── CleanArchitecture.PerformanceTests/
├── CleanArchitecture.SmokeTests/
└── CleanArchitecture.StressTests/
```

## Performance Test Host

The performance project is an executable BenchmarkDotNet host.

- Entry point: `tests/CleanArchitecture.PerformanceTests/Program.cs`
- Benchmarks: `tests/CleanArchitecture.PerformanceTests/Benchmarks/`
- Run command:

```bash
dotnet run -c Release --project tests/CleanArchitecture.PerformanceTests/CleanArchitecture.PerformanceTests.csproj
```

## Documentation Layout

```text
docs/
├── API_GUIDE.md
├── ARCHITECTURE.md
├── ARCHITECTURE_UPDATE.md
├── DEPLOYMENT.md
├── ERROROR_GUIDE.md
├── GETTING_STARTED.md
├── HUSKY_RULES.md
├── IMPLEMENTATION_SUMMARY.md
├── QUICKSTART.md
├── README_TESTING.md
├── SOLUTION_STRUCTURE.md
├── TESTING_STRATEGY.md
└── TESTING_SUMMARY.md
```

## Husky and Branch Rules

Husky is configured in `package.json` and currently installs and uses a pre-push hook.

- Installation trigger: `npm install` runs `npm run prepare`, which installs hooks
- Enforced hook: `.husky/pre-push`
- Current command: `npx --no-install validate-branch-name`

For full details and customization options, see `docs/HUSKY_RULES.md`.
