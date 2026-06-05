# หยุด Wang Ngan HR API (port 5082 / 5083)

Set-Location $PSScriptRoot\..

$stopped = $false
foreach ($port in 5082, 5083) {
    Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue |
        Select-Object -ExpandProperty OwningProcess -Unique |
        Where-Object { $_ -gt 0 } |
        ForEach-Object {
            $proc = Get-Process -Id $_ -ErrorAction SilentlyContinue
            if ($proc) {
                Write-Host "Stopping $($proc.ProcessName) (PID $_) on port $port..." -ForegroundColor Yellow
                Stop-Process -Id $_ -Force -ErrorAction SilentlyContinue
                $stopped = $true
            }
        }
}

Get-Process -Name "WangNganHR.API", "dotnet" -ErrorAction SilentlyContinue |
    Where-Object { $_.Path -like "*WangNganHR.API*" -or $_.MainWindowTitle -like "*WangNganHR.API*" } |
    ForEach-Object {
        Write-Host "Stopping $($_.ProcessName) (PID $($_.Id))..." -ForegroundColor Yellow
        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
        $stopped = $true
    }

if (-not $stopped) {
    Write-Host "No Wang Ngan HR API process found." -ForegroundColor DarkGray
} else {
    Start-Sleep -Seconds 1
    Write-Host "API stopped." -ForegroundColor Green
}
