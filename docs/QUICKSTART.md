# Quick Start

## 1. Build and Run

```bash
dotnet restore
dotnet build
dotnet run --project src/CleanArchitecture.Api
```

## 2. Run Tests

```bash
dotnet test
```

## 3. Run Performance Benchmarks

```bash
dotnet run -c Release --project tests/CleanArchitecture.PerformanceTests/CleanArchitecture.PerformanceTests.csproj
```

## 4. Understand the Solution Layout

```text
src/: Domain, Application, Infrastructure, Api
tests/: unit, architecture, integration, acceptance, smoke, stress, performance
```

Full breakdown: `docs/SOLUTION_STRUCTURE.md`.

## 5. Husky Branch Rule

This repository enforces branch-name validation on push.

- Hook: `.husky/pre-push`
- Command: `npx --no-install validate-branch-name`

More details: `docs/HUSKY_RULES.md`.
