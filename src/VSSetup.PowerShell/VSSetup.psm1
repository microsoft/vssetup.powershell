# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

$ExecutionContext.SessionState.PSVariable.Set(
    (New-Object 'Microsoft.VisualStudio.Setup.PowerShell.VersionTable')
)

Export-ModuleMember -Cmdlet * -Variable VSSetupVersionTable
