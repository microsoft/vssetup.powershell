Visual Studio Setup PowerShell Module
=====================================

This PowerShell module contains cmdlets to query instances of Visual Studio 2017 and newer. It also serves as a more useful sample of using the Setup Configuration APIs than the previously [published samples][samples] though those also have samples using VB and VC++.

## Installing

With Windows Management Framework 5.0 or newer (which installs PowerShell and comes with Windows 10), or [PowerShellGet][psget] for PowerShell 3.0 or 4.0, you can download and install this module easily.

```powershell
Install-Module VSSetup -Scope CurrentUser
```

To install for all users, pass `AllUsers` instead of `CurrentUser`, or just leave the `-Scope` parameter out of the command entirely.

You can also download the ZIP package from the [Releases][releases] page on this project site and extract to a directory named _VSSetup_ under a directory in your `$env:PSMODULEPATH`.

```powershell
Expand-Archive VSSetup.zip "$([Environment]::GetFolderPath("MyDocuments"))\WindowsPowerShell\Modules\VSSetup"
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

To file issues or suggestions, please use the [Issues][issues] page for this project on GitHub.

## Status

This project uses a Git flow model releasing from the `master` branch with development based on and stabilize in the `develop` branch.

Branch  | Status
------  | ------
master  | [![build status: master](https://ci.appveyor.com/api/projects/status/4c1feyut6rvmw1dk/branch/master?svg=true)](https://ci.appveyor.com/project/heaths/vssetup-powershell/branch/master)
develop | [![build status: develop](https://ci.appveyor.com/api/projects/status/4c1feyut6rvmw1dk/branch/develop?svg=true)](https://ci.appveyor.com/project/heaths/vssetup-powershell/branch/develop)

  [issues]: https://github.com/Microsoft/vssetup.powershell/issues
  [psget]: http://go.microsoft.com/fwlink/?LinkID=746217
  [releases]: https://github.com/Microsoft/vssetup.powershell/releases
  [samples]: https://aka.ms/setup/configuration/samples
