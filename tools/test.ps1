# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

[CmdletBinding()]
param (
    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string] $Configuration = $env:CONFIGURATION,

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string] $Platform = $env:PLATFORM,

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string] $Framework = $env:FRAMEWORK,

    [Parameter()]
    [ValidateSet('Unit', 'Integration', 'Functional', 'Runtime')]
    [string[]] $Type = @('Functional', 'Runtime'),

    [Parameter()]
    [switch] $Download
)

if (-not $Configuration) {
    $Configuration = 'Debug'
}

if (-not $Platform) {
    $Platform = 'x86'
}

if (-not $Framework) {
    $Framework = get-childitem "$PSScriptRoot\..\src\VSSetup.PowerShell\bin\$Configuration" | select-object -first 1 -expand Name
    Write-Verbose "Using framework $Framework"
}

[bool] $Failed = $false

if ($Type -contains 'Unit' -or $Type -contains 'Functional')
{
    # Find vstest.console.exe.
    $cmd = get-command vstest.console.exe -ea SilentlyContinue | select-object -expand Path
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

    # Discover test assemblies for the current configuration.
    $assemblies = get-childitem test -include *.test.dll -recurse | where-object {
        $_.fullname -match "\\bin\\$Configuration\\"
    } | foreach-object {
        [string] $path = $_.FullName

        write-verbose "Discovered test assembly '$path'."
        "$path"
    }

    # Run unit tests.
    & $cmd $logger $assemblies /parallel /platform:$Platform
    if (-not $?) {
        $Failed = $true
    }
}

if ($Type -contains 'Integration' -or $Type -contains 'Runtime')
{
    # Run docker runtime tests.
    $cmd = (get-command docker-compose -ea SilentlyContinue).Path
    if (-not $cmd -and $Download) {
        invoke-webrequest 'https://github.com/docker/compose/releases/download/1.11.2/docker-compose-Windows-x86_64.exe' -outfile "${env:TEMP}\docker-compose.exe"
        $cmd = "${env:TEMP}\docker-compose.exe"
    }

    if ($cmd) {
        [string] $path = resolve-path "$PSScriptRoot\..\docker\docker-compose.yml"

        $verbose = if ($VerbosePreference -eq 'Continue') {
            '--verbose'
        }

        # Set environment variables based on parameters so docker-compose uses them (-e doesn't seem to work on Windows).
        $OldConfiguration = $env:CONFIGURATION
        $env:CONFIGURATION = $Configuration

        $OldPlatform = $env:PLATFORM
        $env:PLATFORM = $Platform

        $OldFramework = $env:FRAMEWORK
        $env:FRAMEWORK = $Framework

        write-verbose "Running tests in '$path'"
        try {
            & $cmd -f "$path" $verbose run --rm test
            if (-not $?) {
                $Failed = $true
            }
        } finally {
            $env:CONFIGURATION = $OldConfiguration
            $env:PLATFORM = $OldPlatform
            $env:FRAMEWORK = $OldFramework
        }
    } else {
        write-warning 'Failed to find docker-compose; integration tests will not be performed.'
    }

    if ($Failed) {
        exit 1
    }
}

<#
.SYNOPSIS
Runs unit and integration tests.

.DESCRIPTION
Use this script to run unit and integration tests on this project.
You can also run one or the other using the -Type parameter.

When run in AppVeyor, test results will be posted to the run.

.PARAMETER Configuration
Set the build configuration. Defaults to the $env:CONFIGURATION environment variable;
otherwise, "Debug" if the environment variable is not set.

.PARAMETER Platform
Set the build platform. Defaults to the $env:PLATFORM environment variable;
otherwise, "x86" if the environment variable is not set.

.PARAMETER Framework
Set the target .NET framework. Defaults to $env:FRAMEWORK environment variable;
otherwise, the first subdirectory of the build output directory if the environment variable is not set.

.PARAMETER Type
Specify the type of tests to run. Values include "unit" and "integration".

.EXAMPLE
tools\test.ps1 -configuration release -type integration -v

This will run integration tests using the release binaries (if built) with verbose output.
#>
