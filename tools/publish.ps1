#Require -Version 3.0
[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)]
    $Configuration,

    [Parameter(Mandatory=$true)]
    $ApiKey,

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    $Only = 'Release'
)

if ($Configuration -ne $Only) {
    write-verbose "Skipping publishing for configuration: $configuration"
    return
}

if ($PSVersionTable.Version -lt '5.0' -and !(get-module -list 'powershellget')) {
    # If MSI was installed environment may not be updated in console host.
    $env:PSMODULEPATH = [Environment]::GetEnvironmentVariable('PSMODULEPATH', 'Machine')
}

$solutionDir = resolve-path $PSScriptRoot\..
$projectOutputPath = join-path $solutionDir "src\VSSetup.PowerShell\bin\$Configuration"
$outputPath = join-path $solutionDir "bin\$Configuration\VSSetup"

if (-not (test-path $projectOutputPath)) {
    write-error "Project has not been built: $projectOutputPath"
    exit 3
}

if (-not (test-path $outputPath)) {
    write-verbose "Creating directory: $outputPath"
    $null = new-item $outputPath -type directory
}

write-verbose "Copying from: $projectOutputPath, to: $outputPath"
copy-item "$projectOutputPath\*" $outputPath -container -recurse

import-module PowerShellGet
publish-module -path $outputPath -nugetapikey $ApiKey
