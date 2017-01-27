# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

Describe 'Select-VSSetupInstance' {
    Context 'Defaults to VS' {
        $instances = Get-VSSetupInstance | Select-VSSetupInstance

        It 'Returns 2 instances' {
            $instances.Count | Should Be 2
        }
    }

    Context 'Returns latest launchable VS' {
        $instances = @(Get-VSSetupInstance | Select-VSSetupInstance -Latest)

        It 'Returns 1 instance' {
            $instances.Count | Should Be 1
        }

        It 'Returns Enterprise instance' {
            $instances[0].InstanceId | Should Be 2
            $instances[0].Product.Id | Should Be 'Microsoft.VisualStudio.Product.Enterprise'
        }
    }

    Context 'Returns latest VS' {
        $instances = @(Get-VSSetupInstance -All | Select-VSSetupInstance -Latest)

        It 'Returns 1 instance' {
            $instances.Count | Should Be 1
        }

        It 'Returns Professional instance' {
            $instances[0].InstanceId | Should Be 3
            $instances[0].Product.Id | Should Be 'Microsoft.VisualStudio.Product.Professional'
        }
    }

    Context 'Supports other products' {
        $instances = @(Get-VSSetupInstance | Select-VSSetupInstance -Product 'Microsoft.VisualStudio.Product.BuildTools')

        It 'Returns 1 instance' {
            $instances.Count | Should Be 1
        }

        It 'Returns Build Tools instance' {
            $instances[0].InstanceId | Should Be 4
            $instances[0].Product.Id | Should Be 'Microsoft.VisualStudio.Product.BuildTools'
        }
    }

    Context 'Queries specified workloads' {
        $instances = Get-VSSetupInstance | Select-VSSetupInstance -Require 'Microsoft.VisualStudio.Workload.ManagedDesktop'

        It 'Returns 2 insances' {
            $instances.Count | Should Be 2
        }
    }
}
