# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

Describe 'Get-VSSetupInstance' {
    $en = New-Object System.Globalization.CultureInfo 'en-US'
    $de = New-Object System.Globalization.CultureInfo 'de-DE'

    BeforeAll {
        # Always write to 32-bit registry key.
        $key = New-Item -Path Registry::HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\Setup\Reboot -Force
        $null = $key | New-ItemProperty -Name 3 -Value 1 -Force
    }

    Context 'Launchable instances' {
        $instances = Get-VSSetupInstance

        It 'Returns 3 instances' {
            $instances.Count | Should Be 3
        }

        It 'Returns launchable instances' {
            $instances | ForEach-Object { $_.IsLaunchable | Should Be $true }
        }
    }

    Context 'All instances' {
        $instances = Get-VSSetupInstance -All

        It 'Returns 4 instances' {
            $instances.Count | Should Be 4
        }
    }

    Context 'Specified path' {
        It 'Accepts pipeline input' {
            $instance = Get-Item C:\VS\Community | Get-VSSetupInstance

            $instance.InstanceId | Should Be 1
        }
        It 'Returns Community properties' {
            [System.Globalization.CultureInfo]::CurrentUICulture = $en
            $instance = Get-VSSetupInstance 'C:\VS\Community'

            $instance.InstanceId | Should Be 1
            $instance.InstallationName | Should Be 'VisualStudio/public.d15rel/15.0.26116.0'
            $instance.InstallationPath | Should Be 'C:\VS\Community'
            $instance.InstallationVersion | Should Be '15.0.26116.0'
            $instance.DisplayName | Should Be 'Visual Studio Community 2017'
            $instance.Description | Should Match '^Free'
            $instance.EnginePath | Should Be 'C:\Program Files (x86)\Microsoft Visual Studio\Installer\resources\app\ServiceHub\Services\Microsoft.VisualStudio.Setup.Service'
            $instance.Product.Id | Should Be 'Microsoft.VisualStudio.Product.Community'
            $instance.Product.Version | Should Be '15.0.26116.0'
            $instance.Packages.Count | Should Be 4
        }

        It 'Returns Community properties (de-DE)' {
            [System.Globalization.CultureInfo]::CurrentUICulture = $de
            $instance = Get-VSSetupInstance 'C:\VS\Community'

            $instance.InstanceId | Should Be 1
            $instance.DisplayName | Should Be 'Visual Studio Community 2017'
            $instance.Description | Should Match '^Kostenlose'
        }

        It 'Returns normalized version' {
            $instance = Get-VSSetupInstance 'C:\VS\Community'

            $instance.InstanceId | Should Be 1
            $instance.InstallationVersion | Should Be '15.0.26116.0'
        }

        It 'Does not contain errors' {
            $instance = Get-VSSetupInstance 'C:\VS\Community'

            $instance.State  -band 'NoErrors' | Should Be 'NoErrors'
            $instance.Errors | Should Be $null
        }
    }

    Context 'Contains custom properties' {
        $instance = Get-VSSetupInstance C:\VS\Community

        It 'Contains "ChannelId"' {
            $instance.ChannelId | Should Be 'VisualStudio.15.Release/public.d15rel/15.0.26116.0'
        }
    }

    Context 'Contains additional properties' {
        It 'Contains "Nickname"' {
            $instance = Get-VSSetupInstance C:\VS\Community

            $instance.Properties.Count | Should Be 1
            $instance.Properties['Nickname'] | Should Be 'Community'
        }

        It 'Contains empty properties' {
            $instance = Get-VSSetupInstance C:\BuildTools

            $instance.Properties.Count | Should Be 0
        }
    }

    Context 'Contains errors' {
        $instance = Get-VSSetupInstance C:\VS\Enterprise

        It 'Contains errors' {
            $instance.State -band 'NoErrors' | Should Be 0
            $instance.Errors | Should Not Be $null
        }

        It 'Contains failed packages' {
            $instance.Errors.FailedPackages.Count | Should Be 1
            $instance.Errors.FailedPackages[0].Id | Should Be 'Microsoft.VisualStudio.Workload.Office'
        }

        It 'Contains skipped packages' {
            $instance.Errors.SkippedPackages.Count | Should Be 1
            $instance.Errors.SkippedPackages[0].Id | Should Be 'Microsoft.VisualStudio.Component.Sharepoint.Tools'
        }
    }

    Context 'Prerelease' {
        It 'Contains 4 instances' {
            $instances = Get-VSSetupInstance -Prerelease
            $instances.Count | Should Be 4
        }

        It 'Contains 1 prerelease' {
            $instances = @(Get-VSSetupInstance -Prerelease | Where-Object { $_.IsPrerelease })
            $instances.Count | Should Be 1
        }

        It 'Always with path' {
            $instances = @(Get-VSSetupInstance C:\VS\Preview)
            $instances.Count | Should Be 1
            $instances[0].InstanceId | Should Be 5
        }
    }
}
