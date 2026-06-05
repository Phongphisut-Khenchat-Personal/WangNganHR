# Wang Ngan HR — เริ่มทุกอย่างพร้อมใช้งาน (คำสั่งเดียว)
# ใช้: .\scripts\start.ps1
#       .\scripts\start.ps1 -Web    (เปิดหน้าเว็บด้วย)

param(
    [switch]$Web
)

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot\..

function Write-Step($n, $total, $msg) {
    Write-Host "[$n/$total] $msg" -ForegroundColor Cyan
}

Write-Host ''
Write-Host '========================================' -ForegroundColor Cyan
Write-Host '  Wang Ngan HR — Start' -ForegroundColor Cyan
Write-Host '========================================' -ForegroundColor Cyan
Write-Host ''

# ── 1. Docker (PostgreSQL + Redis) ───────────────────────
Write-Step 1 5 'Database (PostgreSQL + Redis)...'

$dbUp = docker ps -q -f name=janomehr_db 2>$null
if ($dbUp) {
    Write-Host '  PostgreSQL + Redis already running' -ForegroundColor Green
} else {
    $stopped = docker ps -aq -f name=janomehr_db 2>$null
    if ($stopped) {
        Write-Host '  Starting existing containers...' -ForegroundColor Yellow
        docker start janomehr_db janomehr_redis | Out-Null
    } else {
        Write-Host '  Creating containers (docker compose up)...' -ForegroundColor Yellow
        docker compose up -d
        if ($LASTEXITCODE -ne 0) { throw 'docker compose failed' }
    }
}

$ready = $false
for ($i = 0; $i -lt 30; $i++) {
    docker exec janomehr_db pg_isready -U janome 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) { $ready = $true; break }
    Start-Sleep -Seconds 1
}
if (-not $ready) { throw 'PostgreSQL not ready after 30s' }
Write-Host '  Database ready' -ForegroundColor Green

# ── 2. Trust dev cert (Desktop WDAC) ─────────────────────
Write-Step 2 5 'Dev certificate (Desktop signing)...'
$cert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert -ErrorAction SilentlyContinue |
    Where-Object { $_.Subject -like '*WangNganHR Local Dev*' } |
    Select-Object -First 1
if (-not $cert) {
    & "$PSScriptRoot\sign-assemblies.ps1" -OutputDir (Join-Path $PWD 'WangNganHR.API\bin\Debug\net10.0') 2>$null
    $cert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert -ErrorAction SilentlyContinue |
        Where-Object { $_.Subject -like '*WangNganHR Local Dev*' } |
        Select-Object -First 1
}
if ($cert) {
    foreach ($storeName in @('Root', 'TrustedPublisher')) {
        $store = New-Object System.Security.Cryptography.X509Certificates.X509Store($storeName, 'CurrentUser')
        $store.Open('ReadWrite')
        if ($store.Certificates.Find('FindByThumbprint', $cert.Thumbprint, $false).Count -eq 0) {
            $store.Add($cert)
        }
        $store.Close()
    }
}
Write-Host '  Certificate OK' -ForegroundColor Green

# ── 3. API ───────────────────────────────────────────────
Write-Step 3 5 'API (http://localhost:5083)...'

Get-Process -Name 'WangNganHR.Desktop' -ErrorAction SilentlyContinue | ForEach-Object {
    Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
}

& "$PSScriptRoot\stop-api.ps1" | Out-Null
Start-Sleep -Seconds 1

Start-Process powershell -ArgumentList @(
    '-NoProfile', '-ExecutionPolicy', 'Bypass',
    '-File', (Join-Path $PSScriptRoot 'run-api.ps1')
) -WorkingDirectory (Get-Location)

$apiReady = $false
for ($i = 0; $i -lt 45; $i++) {
    foreach ($port in 5083, 5082) {
        try {
            $r = Invoke-WebRequest -Uri "http://localhost:$port/swagger/index.html" -UseBasicParsing -TimeoutSec 2
            if ($r.StatusCode -eq 200) { $apiReady = $true; break }
        } catch { }
    }
    if ($apiReady) { break }
    Start-Sleep -Seconds 1
}
if (-not $apiReady) { throw 'API did not start — check the API PowerShell window' }
Write-Host '  API ready' -ForegroundColor Green

# ── 4. Desktop ───────────────────────────────────────────
Write-Step 4 5 'Desktop app...'
& "$PSScriptRoot\run-desktop.ps1"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

# ── 5. Web (optional) ────────────────────────────────────
if ($Web) {
    Write-Step 5 5 'Web (http://localhost:5203)...'
    Start-Process powershell -ArgumentList @(
        '-NoProfile', '-ExecutionPolicy', 'Bypass',
        '-File', (Join-Path $PSScriptRoot 'run-web.ps1')
    ) -WorkingDirectory (Get-Location)
    Write-Host '  Web starting in new window' -ForegroundColor Green
} else {
    Write-Step 5 5 'Done'
}

Write-Host ''
Write-Host '========================================' -ForegroundColor Green
Write-Host '  Ready!' -ForegroundColor Green
Write-Host '========================================' -ForegroundColor Green
Write-Host ''
Write-Host '  API:     http://localhost:5083/swagger' -ForegroundColor White
Write-Host '  Web:     http://localhost:5203  (run with -Web)' -ForegroundColor DarkGray
Write-Host '  DB:      .\scripts\connect-db.ps1' -ForegroundColor DarkGray
Write-Host ''
Write-Host '  Login accounts:' -ForegroundColor Yellow
Write-Host '    admin   / admin123   (Admin)' -ForegroundColor White
Write-Host '    hr      / hr123      (HR)' -ForegroundColor White
Write-Host '    manager / manager123 (Manager)' -ForegroundColor White
Write-Host ''
Write-Host '  Keep the API window open while using Desktop/Web.' -ForegroundColor DarkGray
Write-Host ''
