# Apply EF migrations — signs assemblies first (fixes WDAC 0x800711C7 blocking dotnet ef)
# Requires PostgreSQL running with connection string from WangNganHR.API/appsettings.json

Set-Location $PSScriptRoot\..

Write-Host "Building WangNganHR.API..." -ForegroundColor Cyan
dotnet build WangNganHR.API\WangNganHR.API.csproj -v q
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$apiOut = Join-Path (Get-Location) 'WangNganHR.API\bin\Debug\net10.0'
$sharedOut = Join-Path (Get-Location) 'WangNganHR.Shared\bin\Debug\net10.0'

Write-Host "Signing assemblies (WDAC)..." -ForegroundColor Cyan
& "$PSScriptRoot\sign-assemblies.ps1" -OutputDir $apiOut
& "$PSScriptRoot\sign-assemblies.ps1" -OutputDir $sharedOut

Write-Host "Applying database migrations..." -ForegroundColor Cyan
dotnet ef database update --project WangNganHR.API\WangNganHR.API.csproj --startup-project WangNganHR.API\WangNganHR.API.csproj
exit $LASTEXITCODE
