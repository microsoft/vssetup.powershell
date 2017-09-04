# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

Describe 'VSSetupVersionTable' {
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
}
