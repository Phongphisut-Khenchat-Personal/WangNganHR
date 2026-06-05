param(
    [Parameter(Mandatory = $true)]
    [string]$OutputDir
)

$ErrorActionPreference = 'Stop'

if (-not (Test-Path $OutputDir)) {
    Write-Error "Output directory not found: $OutputDir"
}

$cert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert -ErrorAction SilentlyContinue |
    Where-Object { $_.Subject -like '*JanomeHR Local Dev*' } |
    Select-Object -First 1

if (-not $cert) {
    $cert = New-SelfSignedCertificate `
        -Type CodeSigningCert `
        -Subject 'CN=JanomeHR Local Dev' `
        -CertStoreLocation 'Cert:\CurrentUser\My' `
        -NotAfter (Get-Date).AddYears(3)
    Write-Host 'Created local code-signing certificate: JanomeHR Local Dev' -ForegroundColor Yellow
}

$signed = 0
foreach ($path in @(
    Get-ChildItem -Path $OutputDir -Filter 'JanomeHR*.dll' -File -ErrorAction SilentlyContinue
    Get-ChildItem -Path $OutputDir -Filter 'JanomeHR*.exe' -File -ErrorAction SilentlyContinue
)) {
    Set-AuthenticodeSignature -FilePath $path.FullName -Certificate $cert | Out-Null
    Write-Host "Signed: $($path.Name)" -ForegroundColor DarkGray
    $signed++
}

if ($signed -eq 0) {
    Write-Warning "No JanomeHR assemblies found in $OutputDir"
}
