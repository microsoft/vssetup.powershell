# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

Describe 'VSSetupVersionTable' {
    BeforeEach {
        Import-Module VSSetup
    }

    Context 'ModuleVersion' {
        It 'Is Version' {
            $VSSetupVersionTable.ModuleVersion -is [System.Version] | Should Be $true
        }
    }

    Context 'QueryVersion' {
        It 'Is Version' {
            $VSSetupVersionTable.QueryVersion -is [System.Version] | Should Be $true
        }
    }

    Context 'Variable' {
        $Error.Clear()

        It 'Can Be Removed' {
            Remove-Module VSSetup
            $Error.Count | Should Be 0
        }
    }
}
