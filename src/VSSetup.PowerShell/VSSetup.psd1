# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.
@{
GUID = '440e8fb1-19c4-4d39-8f75-37424bc4265a'
Author = 'Microsoft Corporation'
CompanyName = 'Microsoft Corporation'
Copyright = 'Copyright (C) Microsoft Corporation. All rights reserved.'
Description = 'Visual Studio Setup PowerShell Module'
ModuleVersion = '$BuildVersion$'
PowerShellVersion = '2.0'
CLRVersion = '2.0'
ModuleToProcess = 'VSSetup.psm1'
NestedModules = 'Microsoft.VisualStudio.Setup.PowerShell.dll'
RequiredAssemblies = @(
  'Microsoft.VisualStudio.Setup.PowerShell.dll',
  'Microsoft.VisualStudio.Setup.Configuration.Interop.dll'
)
CmdletsToExport = @(
  'Get-VSSetupInstance'
  'Select-VSSetupInstance'
)
TypesToProcess = 'VSSetup.types.ps1xml'
PrivateData = @{
  PSData = @{
    ProjectUri = 'https://github.com/Microsoft/vssetup.powershell'
    LicenseUri = 'https://github.com/Microsoft/vssetup.powershell/raw/$CommitId$/LICENSE.txt'
  }
}
}
