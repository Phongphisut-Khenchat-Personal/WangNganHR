# Wang Ngan HR Web (Blazor WASM) — ปิด process เก่าที่พอร์ต 5203 ก่อนรัน
# รัน API ก่อน: dotnet run --project WangNganHR.API

Set-Location $PSScriptRoot\..

$port = 5203
$pids = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue |
    Select-Object -ExpandProperty OwningProcess -Unique |
    Where-Object { $_ -gt 0 }

foreach ($procId in $pids) {
    $proc = Get-Process -Id $procId -ErrorAction SilentlyContinue
    if ($proc) {
        Write-Host "Stopping $($proc.ProcessName) (PID $procId) on port $port..." -ForegroundColor Yellow
        Stop-Process -Id $procId -Force -ErrorAction SilentlyContinue
    }
}

if ($pids) { Start-Sleep -Seconds 2 }

Write-Host "Building WangNganHR.Web..." -ForegroundColor Cyan
dotnet build WangNganHR.Web\WangNganHR.Web.csproj -v q
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Starting WangNganHR.Web at http://localhost:$port ..." -ForegroundColor Cyan
Write-Host "Hard refresh browser: Ctrl+Shift+R" -ForegroundColor DarkGray
dotnet run --project WangNganHR.Web --launch-profile http --no-build
