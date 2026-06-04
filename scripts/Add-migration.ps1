$migration_name = $args[0]
Set-Location -Path ..\src\CleanArchitecture.Api
dotnet ef migrations add $migration_name --project ..\CleanArchitecture.Infrastructure\CleanArchitecture.Infrastructure.csproj -o Database/Migrations --no-build --verbose
Set-Location -Path ..\..\scripts