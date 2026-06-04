Set-Location -Path ..\src\CleanArchitecture.Api
dotnet ef migrations remove --force --project ..\CleanArchitecture.Infrastructure\CleanArchitecture.Infrastructure.csproj
Set-Location -Path ..\..\scripts