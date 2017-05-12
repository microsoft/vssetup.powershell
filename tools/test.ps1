# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

[CmdletBinding()]
param (
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string] $Configuration = $env:CONFIGURATION,

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string] $Platform = $env:PLATFORM
)

if (-not $Configuration) {
    $Configuration = 'Debug'
}

if (-not $Platform) {
    $Platform = 'x86'
}

$ErrorActionPreference = 'SilentlyContinue'

# Find vstest.console.exe.
$cmd = get-command vstest.console.exe | select-object -expand Path
if (-not $cmd) {
    $vswhere = get-childitem "$PSScriptRoot\..\packages\vswhere*" -filter vswhere.exe -recurse | select-object -first 1 -expand FullName
    if (-not $vswhere) {
        write-error 'Please run "nuget restore" on the solution to download vswhere.'
        exit 1
    }

    $path = & $vswhere -latest -requires Microsoft.VisualStudio.Component.ManagedDesktop.Core -property installationPath
    if (-not $path) {
        write-error 'No instance of Visual Studio found with vstest.console.exe. Please start a developer command prompt.'
        exit 1
    }

    $cmd = join-path $path 'Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe'
}

if (-not (test-path $cmd)) {
    write-error 'Could not find vstest.console.exe. Please start a developer command prompt.'
    exit 1
}

# Set up logger for AppVeyor.
$logger = if ($env:APPVEYOR -eq 'true') {
    write-verbose 'Using AppVeyor logger when running in an AppVeyor build.'
    '/logger:appveyor'
}

# Discover test assemblies for the current configuration.
$assemblies = get-childitem test -include *.test.dll -recurse | where-object {
    $_.fullname -match "\\bin\\$Configuration\\"
} | foreach-object {
    [string] $path = $_.FullName

    write-verbose "Discovered test assembly '$path'."
    "$path"
}

# Find the test adapter.
$adapter = get-childitem "$PSScriptRoot\..\packages\xunit.runner.visualstudio*" -filter xunit.runner.visualstudio.testadapter.dll -recurse | select-object -first 1 -expand FullName | foreach-object {
    "/TestAdapterPath:`"$($_.FullName)`""
}

if (-not $adapter) {
    write-warning 'Could not find test adapter. Unit tests may not be discovered.'
}

# Run unit tests.
& $cmd $logger $assemblies /parallel /platform:$Platform
if (-not $?) {
    exit $LASTEXITCODE
}

# Run docker integration tests in a separate process to set environment variables without affecting current process.
if (get-command docker-compose -ea SilentlyContinue) {
    [string] $path = resolve-path "$PSScriptRoot\..\docker\appveyor\docker-compose.yml"
    write-verbose "Running tests in $path"

    docker-compose -f "$path" run test -c Invoke-Pester C:\Tests -EnableExit -OutputFile C:\Tests\Results.xml -OutputFormat NUnitXml
    if ($env:APPVEYOR -eq 'true') {
        invoke-webrequest "https://ci.appveyor.com/api/testresults/nunit/${env:APPVEYOR_JOB_ID}" -method POST -contentType 'multipart/form-data' -inFile "$PSScriptRoot\..\Results.xml"
    }

    exit $LASTEXITCODE
}