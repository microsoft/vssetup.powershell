# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

Describe 'Get-VSSetupInstance' {
    $en = New-Object System.Globalization.CultureInfo 'en-US'
    $de = New-Object System.Globalization.CultureInfo 'de-DE'

    Context 'Launchable instances' {
        [System.Globalization.CultureInfo]::CurrentUICulture = $en
        $instances = Get-VSSetupInstance

        It 'Returns 3 instances' {
            $instances.Count | Should Be 3
        }

        It 'Returns launchable instances' {
            $instances | ForEach-Object { $_.IsLaunchable | Should Be $true }
        }
    }

    Context 'All instances' {
        [System.Globalization.CultureInfo]::CurrentUICulture = $en
        $instances = Get-VSSetupInstance -All

        It 'Returns 4 instances' {
            $instances.Count | Should Be 4
        }
    }

    Context 'Specified path' {
        It 'Accepts pipeline input' {
            [System.Globalization.CultureInfo]::CurrentUICulture = $en
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
            [System.Globalization.CultureInfo]::CurrentUICulture = $en
            $instance = Get-VSSetupInstance 'C:\VS\Community'

            $instance.InstanceId | Should Be 1
            $instance.InstallationVersion | Should Be '15.0.26116.0'
        }
    }

    Context 'Contains additional properties' {
        [System.Globalization.CultureInfo]::CurrentUICulture = $en
        $instance = Get-VSSetupInstance C:\VS\Community

        It 'Contains "ChannelId"' {
            $instance.ChannelId | Should Be 'VisualStudio.15.Release/public.d15rel/15.0.26116.0'
        }

        It 'Contains "EnginePath"' {
            $instance.EnginePath | Should Be 'C:\Program Files (x86)\Microsoft Visual Studio\Installer\resources\app\ServiceHub\Services\Microsoft.VisualStudio.Setup.Service'
        }
    }
}
