$migration_name = $args[0]
Set-Location -Path ..\src\CleanArchitecture.Api
dotnet ef migrations remove $migration_name --project ..\CleanArchitecture.Infrastructure\CleanArchitecture.Infrastructure.csproj
Set-Location -Path ..\..\scripts