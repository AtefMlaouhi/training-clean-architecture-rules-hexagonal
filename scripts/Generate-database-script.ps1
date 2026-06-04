$rootPath = Split-Path -Parent $PSScriptRoot
$now = [DateTimeOffset]::Now.ToUnixTimeSeconds().ToString()
$migrationFileName = "database-$($now).sql";
dotnet ef migrations script --project "$rootPath/src/CleanArchitecture.Api/CleanArchitecture.Api.csproj" -o "$rootPath/database/$($migrationFileName)" --idempotent --no-build
$raw_content = Get-Content "$rootPath/database/$($migrationFileName)"
$formatted_content = $raw_content | Where-Object { $_.trim() -ne "" }
$formatted_content | Set-Content "$rootPath/database/$($migrationFileName)"
