# Wang Ngan HR Desktop — build Release, sign (WDAC), then launch .exe
# รัน API ก่อน: dotnet run --project WangNganHR.API

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot\..

$outDir = Join-Path $PWD 'WangNganHR.Desktop\bin\Release\net10.0-windows'
$exe    = Join-Path $outDir 'WangNganHR.Desktop.exe'

Write-Host 'Building WangNganHR.Desktop (Release)...' -ForegroundColor Cyan
dotnet build WangNganHR.Desktop -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& "$PSScriptRoot\sign-desktop.ps1" -OutputDir $outDir

Write-Host 'Starting WangNganHR.Desktop...' -ForegroundColor Green
try {
    $proc = Start-Process -FilePath $exe -PassThru
} catch {
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host 'WDAC blocked launch. Re-run this script (sign-assemblies trusts the local dev cert), or ask IT to allow this project folder.' -ForegroundColor Yellow
    exit 1
}

if ($proc.HasExited -and $proc.ExitCode -ne 0) {
    Write-Host "WangNganHR.Desktop exited immediately (code $($proc.ExitCode))." -ForegroundColor Red
    Write-Host 'If you see Application Control (0x800711C7), ask IT to allow this project folder or run from Visual Studio.' -ForegroundColor Yellow
    exit $proc.ExitCode
}

Write-Host "WangNganHR.Desktop running (PID $($proc.Id))." -ForegroundColor Green
