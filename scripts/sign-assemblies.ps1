param(
    [Parameter(Mandatory = $true)]
    [string]$OutputDir
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $OutputDir)) {
    Write-Error "Output directory not found: $OutputDir"
}

$cert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert -ErrorAction SilentlyContinue |
    Where-Object { $_.Subject -like '*WangNganHR Local Dev*' } |
    Select-Object -First 1

if (-not $cert) {
    $cert = New-SelfSignedCertificate `
        -Type CodeSigningCert `
        -Subject 'CN=WangNganHR Local Dev' `
        -CertStoreLocation 'Cert:\CurrentUser\My' `
        -NotAfter (Get-Date).AddYears(3)
    Write-Host 'Created local code-signing certificate: WangNganHR Local Dev' -ForegroundColor Yellow
}

foreach ($storeName in @('Root', 'TrustedPublisher')) {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store($storeName, 'CurrentUser')
    $store.Open('ReadWrite')
    if ($store.Certificates.Find('FindByThumbprint', $cert.Thumbprint, $false).Count -eq 0) {
        $store.Add($cert)
        Write-Host "Trusted cert in CurrentUser\$storeName" -ForegroundColor DarkGray
    }
    $store.Close()
}

$signed = 0
foreach ($path in @(
    Get-ChildItem -Path $OutputDir -Filter 'WangNganHR*.dll' -File -ErrorAction SilentlyContinue
    Get-ChildItem -Path $OutputDir -Filter 'WangNganHR*.exe' -File -ErrorAction SilentlyContinue
)) {
    Set-AuthenticodeSignature -FilePath $path.FullName -Certificate $cert | Out-Null
    Write-Host "Signed: $($path.Name)" -ForegroundColor DarkGray
    $signed++
}

if ($signed -eq 0) {
    Write-Warning "No WangNganHR assemblies found in $OutputDir"
}
