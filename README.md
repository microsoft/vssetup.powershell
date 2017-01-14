Visual Studio Setup PowerShell Module
=====================================

This PowerShell module contains cmdlets to query instances of Visual Studio 2017 and newer. It also serves as a more useful sample of using the Setup Configuration APIs than the previously [published samples](https://github.com/microsoft/vs-setup-samples) though those also have samples using VB and VC++.

## Installing

You can download packages from the Releases page for this project on GitHub, but with Windows Management Framework 5.0 or newer (which installs PowerShell and comes with Windows 10), you can download and install this module even easier.

```powershell
Install-Module VSSetup -Scope CurrentUser
```

## Using

You can query all usable instances of Visual Studio and other products installed by the Visual Studio installer.

```powershell
Get-VSSetupInstance
```

To get the instance for a specific installation directory, you can run the following.

```powershell
Get-VSSetupInstance 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community'
```

If you want to select the latest instance that contains the .NET desktop development workload, you can pipe all instances - usable or not - to `Select-VSSetupInstance` that provides more fine grain control over which instances you enumerate.

```powershell
Get-VSSetupInstance -All | Select-VSSetupInstance -Require 'Microsoft.VisualStudio.Workload.ManagedDesktop' -Latest
```

## Feedback

To file issues or suggestions, please use the Issues page for this project on GitHub.
