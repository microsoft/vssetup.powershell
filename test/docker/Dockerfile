# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

# Require .NET Framework
FROM microsoft/windowsservercore

# Download and register current query APIs
SHELL ["powershell.exe", "-ExecutionPolicy", "Bypass", "-Command"]
ENV API_VERSION="1.4.174-rc"
RUN $ErrorActionPreference = 'Stop' ; \
    New-Item -Path C:\Downloads -Type Directory | Out-Null ; \
    Invoke-WebRequest -Uri "https://www.nuget.org/api/v2/package/Microsoft.VisualStudio.Setup.Configuration.Native/${env:API_VERSION}" -OutFile C:\Downloads\Microsoft.VisualStudio.Setup.Configuration.Native.zip ; \
    Expand-Archive -Path C:\Downloads\Microsoft.VisualStudio.Setup.Configuration.Native.zip -DestinationPath C:\Downloads\Microsoft.VisualStudio.Setup.Configuration.Native ; \
    C:\Windows\System32\regsvr32.exe /s C:\Downloads\Microsoft.VisualStudio.Setup.Configuration.Native\tools\x64\Microsoft.VisualStudio.Setup.Configuration.Native.dll ; \
    C:\Windows\SysWOW64\regsvr32.exe /s C:\Downloads\Microsoft.VisualStudio.Setup.Configuration.Native\tools\x86\Microsoft.VisualStudio.Setup.Configuration.Native.dll

# Install latest version of Pester for integration testing
RUN $ErrorActionPreference = 'Stop' ; \
    Install-PackageProvider -Name nuget -MinimumVersion 2.8.5.201 -Force ; \
    Install-Module -Name Pester -Scope CurrentUser -SkipPublisherCheck -Force

ENTRYPOINT ["powershell.exe", "-ExecutionPolicy", "Unrestricted"]
