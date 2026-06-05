# Janome HR Desktop — build Release, sign (WDAC), then launch .exe
# รัน API ก่อน: dotnet run --project JanomeHR.API

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot\..

$outDir = Join-Path $PWD 'JanomeHR.Desktop\bin\Release\net10.0-windows'
$exe    = Join-Path $outDir 'JanomeHR.Desktop.exe'

Write-Host 'Building JanomeHR.Desktop (Release)...' -ForegroundColor Cyan
dotnet build JanomeHR.Desktop -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& "$PSScriptRoot\sign-desktop.ps1" -OutputDir $outDir

Write-Host 'Starting JanomeHR.Desktop...' -ForegroundColor Green
$proc = Start-Process -FilePath $exe -PassThru

if ($proc.HasExited -and $proc.ExitCode -ne 0) {
    Write-Host "JanomeHR.Desktop exited immediately (code $($proc.ExitCode))." -ForegroundColor Red
    Write-Host 'If you see Application Control (0x800711C7), ask IT to allow this project folder or run from Visual Studio.' -ForegroundColor Yellow
    exit $proc.ExitCode
}

Write-Host "JanomeHR.Desktop running (PID $($proc.Id))." -ForegroundColor Green
