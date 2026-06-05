# Wang Ngan HR API — ปิด process เก่าที่พอร์ต 5082/5083 ก่อนรัน

Set-Location $PSScriptRoot\..

$ports = 5082, 5083
$pids = @()
foreach ($port in $ports) {
    $pids += Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue |
        Select-Object -ExpandProperty OwningProcess -Unique |
        Where-Object { $_ -gt 0 }
}

$pids = $pids | Select-Object -Unique
foreach ($procId in $pids) {
    $proc = Get-Process -Id $procId -ErrorAction SilentlyContinue
    if ($proc) {
        Write-Host "Stopping $($proc.ProcessName) (PID $procId)..." -ForegroundColor Yellow
        Stop-Process -Id $procId -Force -ErrorAction SilentlyContinue
    }
}

Get-Process -Name "WangNganHR.API" -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Host "Stopping $($_.ProcessName) (PID $($_.Id))..." -ForegroundColor Yellow
    Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
}

if ($pids) { Start-Sleep -Seconds 2 }

Write-Host "Building WangNganHR.API..." -ForegroundColor Cyan
dotnet build WangNganHR.API\WangNganHR.API.csproj -v q
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Starting WangNganHR.API at http://localhost:5082 and http://localhost:5083 ..." -ForegroundColor Cyan
dotnet run --project WangNganHR.API --launch-profile http --no-build
