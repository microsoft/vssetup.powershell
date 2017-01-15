Contributing
============

## Prerequisites

This project uses the following software. Newer versions may work but backward compatibility must be maintained.

* [Visual Studio 2015](https://www.visualstudio.com/en-us/downloads/visual-studio-2015-downloads-vs.aspx)

### Optional

Some projects require optional software to open or otherwise use in Visual Studio. They are not required to build the solution using MSBuild.

* [NuProj Package Project](https://marketplace.visualstudio.com/items?itemName=NuProjTeam.NuGetPackageProject)

## Coding

Code analysis and style cop rules are defined for this solution, but are currently not enforced during build for performance reasons or treated as errors. This may change in the future. Please resolve any build warnings in the code editor or **Error List** window.

If you add any commands please update the `Tags` property of the _VSSetup.nuproj_ project as appropriate. This project is used instead of `Publish-Module` from the _PowerShellGet_ module because it works better with the build systems and can be tested on developer machines without also publishing.

## Building

Before you can build this project from the command line with MSBuild or within Visual Studio, you must restore packages.

* In Visual Studio, make sure Package Restore is enabled.
* On the command line and assuming _nuget.exe_ is in your `PATH`, in the solution directory run: `nuget restore`

Note again that to build the full solution in Visual Studio some optional software may be required.

## Testing

All available tests are discovered after a complete build in Test Explorer within Visual Studio.

On the command line, you can run the following commands from the solution directory. Replace `<version>` with whatever version was downloaded.

```
nuget install xunit.runner.console -outputdirectory packages
packages\xunit.runner.console.<version>\tools\xunit.runner.console test\VSSetup.PowerShell.Test\bin\Debug\Microsoft.VisualStudio.Setup.PowerShell.Test.dll
```

## Pull Requests

We welcome pull requests for both bug fixes and new features that solve a common enough problem to benefit the community. Please note the following requirements.

1. Code changes for bug fixes and new features are accompanied by new tests or, only if required, modifications to existing tests. Modifying existing tests when not required may introduce regressions.
2. All tests must pass. We have automated PR builds that will verify any PRs before they can be merged, but you are encouraged to run all tests in your development environment prior to pushing to your remote.

Thank you for your contributions!
