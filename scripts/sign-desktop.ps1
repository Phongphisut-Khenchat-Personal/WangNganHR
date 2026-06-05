param(
    [Parameter(Mandatory = $true)]
    [string]$OutputDir
)

$ErrorActionPreference = 'Stop'
& "$PSScriptRoot\sign-assemblies.ps1" -OutputDir $OutputDir
