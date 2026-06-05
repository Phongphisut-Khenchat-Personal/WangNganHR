# Rename root folder JanomeHR -> WangNganHR (close Cursor first)
$ErrorActionPreference = 'Stop'
$projects = 'C:\Projects'
$old = Join-Path $projects 'JanomeHR'
$new = Join-Path $projects 'WangNganHR'

if (-not (Test-Path $old)) {
    Write-Host "Not found: $old (may already be renamed)" -ForegroundColor Yellow
    exit 0
}

if (Test-Path $new) {
    $item = Get-Item $new -Force
    if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) {
        cmd /c rmdir $new
        Write-Host "Removed temporary junction" -ForegroundColor DarkGray
    } else {
        Write-Error "Folder already exists: $new (not a junction). Check manually."
    }
}

Get-Process WangNganHR.Desktop,dotnet -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 1

Rename-Item -Path $old -NewName 'WangNganHR'
Write-Host "Renamed to: $new" -ForegroundColor Green
Write-Host "Reopen Cursor at: $new" -ForegroundColor Cyan

foreach ($stale in @('JanomeHR.API','JanomeHR.Web','JanomeHR.Desktop','JanomeHR.Shared')) {
    $p = Join-Path $projects $stale
    if (Test-Path $p) {
        Remove-Item $p -Recurse -Force
        Write-Host "Removed stale folder: $stale" -ForegroundColor DarkGray
    }
}
$staleSlnx = Join-Path $projects 'JanomeHR.slnx'
if (Test-Path $staleSlnx) {
    Remove-Item $staleSlnx -Force
}
