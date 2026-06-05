# Apply EF migrations — signs assemblies first (fixes WDAC 0x800711C7 blocking dotnet ef)
# Requires PostgreSQL running with connection string from JanomeHR.API/appsettings.json

Set-Location $PSScriptRoot\..

Write-Host "Building JanomeHR.API..." -ForegroundColor Cyan
dotnet build JanomeHR.API\JanomeHR.API.csproj -v q
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$apiOut = Join-Path (Get-Location) 'JanomeHR.API\bin\Debug\net10.0'
$sharedOut = Join-Path (Get-Location) 'JanomeHR.Shared\bin\Debug\net10.0'

Write-Host "Signing assemblies (WDAC)..." -ForegroundColor Cyan
& "$PSScriptRoot\sign-assemblies.ps1" -OutputDir $apiOut
& "$PSScriptRoot\sign-assemblies.ps1" -OutputDir $sharedOut

Write-Host "Applying database migrations..." -ForegroundColor Cyan
dotnet ef database update --project JanomeHR.API\JanomeHR.API.csproj --startup-project JanomeHR.API\JanomeHR.API.csproj
exit $LASTEXITCODE
