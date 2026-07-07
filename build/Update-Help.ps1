<#
.SYNOPSIS
    Regenerates the checked-in PowerShell help artifacts from the markdown docs.

.DESCRIPTION
    The official build runs under 1ES network isolation and cannot reach the
    PowerShell Gallery, so it no longer installs platyPS or runs New-ExternalHelp.
    Instead, the generated help artifacts are checked in under
    src\VSSetup.PowerShell\help and copied to the build output by the project.

    Run this script locally (outside the isolated build) whenever the cmdlet or
    about_ markdown files under docs\VSSetup change, then commit the regenerated
    files under src\VSSetup.PowerShell\help.

.NOTES
    Requires the platyPS module. Install it locally with:
        Install-Module -Name platyPS -Scope CurrentUser
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$docsPath = Join-Path $repoRoot 'docs\VSSetup'
$outputPath = Join-Path $repoRoot 'src\VSSetup.PowerShell\help'

if (-not (Get-Module -ListAvailable -Name platyPS)) {
    throw "platyPS is not installed. Run: Install-Module -Name platyPS -Scope CurrentUser"
}

Import-Module platyPS

if (-not (Test-Path -Path $outputPath)) {
    $null = New-Item -Path $outputPath -ItemType Directory
}

New-ExternalHelp -Path $docsPath -OutputPath $outputPath -Force | Out-Null

Write-Host "Regenerated help artifacts in $outputPath. Review and commit the changes."
