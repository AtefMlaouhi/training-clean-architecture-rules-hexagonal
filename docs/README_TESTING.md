# Testing Guide

This repository uses multiple test suites to validate architecture, behavior, reliability, and performance.

## Test Suite Structure

```text
tests/
├── CleanArchitecture.Domain.Tests/          # Domain unit tests
├── CleanArchitecture.Application.Tests/     # Application unit tests
├── CleanArchitecture.Infrastructure.Tests/  # Infrastructure unit tests
├── CleanArchitecture.Api.Tests/             # API tests
├── CleanArchitecture.IntegrationTests/      # End-to-end integration tests
└── CleanArchitecture.ArchitectureTests/     # Dependency and scenarios
```

## Common Commands

```bash
# Run all test projects
dotnet test

# Run a specific suite
dotnet test tests/CleanArchitecture.Application.Tests

# Run only performance benchmarks
dotnet run -c Release --project tests/CleanArchitecture.PerformanceTests/CleanArchitecture.PerformanceTests.csproj
```

## Coverage

```bash
dotnet test --collect:"XPlat Code Coverage" --settings:.runsettings
```

## Husky Rule Impact on Testing Workflow

Before pushing benchmark or test changes, Husky validates branch naming.

- Active hook: `.husky/pre-push`
- Rule command: `npx --no-install validate-branch-name`

If your push is blocked by naming rules, rename the branch and push again.

See `docs/HUSKY_RULES.md` for details and customization options.
