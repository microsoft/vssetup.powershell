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

        It 'Returns 2 instances' {
            $instances.Count | Should Be 2
        }
    }

    Context 'Version range' {
        It 'Throws on missing range' {
            { Get-VSSetupInstance | Select-VSSetupInstance -Version } | Should Throw
        }

        It 'Throws on invalid range' {
            { Get-VSSetupInstance | Select-VSSetupInstance -Version 'invalid' } | Should Throw
        }

        It 'Matches single version' {
            $instances = Get-VSSetupInstance | Select-VSSetupInstance -Version '15.0.26116'
            $instances.Count | Should Be 2
        }

        It 'Multiple inclusive matches' {
            $instances = Get-VSSetupInstance | Select-VSSetupInstance -Version '[15.0.26116,)'
            $instances.Count | Should Be 2
        }

        It 'Single exclusive match' {
            $instances = @(Get-VSSetupInstance | Select-VSSetupInstance -Version '(15.0.26116,)')
            $instances.Count | Should Be 1
        }

        It 'No matches' {
            $instances = @(Get-VSSetupInstance | Select-VSSetupInstance -Version '(15.0.26117,)')
            $instances.Count | Should Be 0
        }

        It 'Returns latest capped version' {
            $instance = Get-VSSetupInstance | Select-VSSetupInstance -Version '[,15.0.26116]' -Latest

            $instance.InstanceId | Should Be 1
        }
    }

    Context 'Product wildcards' {
        It 'Returns all instances' {
            $instances = @(Get-VSSetupInstance | Select-VSSetupInstance -Product *)
            $instances.Count | Should Be 3
        }

        It 'Returns matching instance' {
            $instances = @(Get-VSSetupInstance | Select-VSSetupInstance -Product *BuildTools)
            $instances.Count | Should Be 1
            $instances[0].InstanceId | Should Be 4
        }

        It 'Returns all instances with workload' {
            $instances = @(Get-VSSetupInstance | Select-VSSetupInstance -Product * -Require 'Microsoft.VisualStudio.Workload.ManagedDesktop')
            $instances.Count | Should Be 2
        }
    }
}
