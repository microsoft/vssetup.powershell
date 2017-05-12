# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

# Require .NET Framework on the latest image cached by AppVeyor
FROM microsoft/windowsservercore@sha256:9b736c12978e3475cec83e93a12c8b3be2ba445eb733902827e1ccb9f499bc18
SHELL ["powershell.exe", "-ExecutionPolicy", "Bypass", "-Command"]

# Download and register current query APIs
ENV API_VERSION="1.8.24"
RUN $ErrorActionPreference = 'Stop' ; \
    $VerbosePreference = 'Continue' ; \
    Invoke-WebRequest -Uri "https://www.nuget.org/api/v2/package/Microsoft.VisualStudio.Setup.Configuration.Native/${env:API_VERSION}" -OutFile "${env:TEMP}\Microsoft.VisualStudio.Setup.Configuration.Native.zip" ; \
    Expand-Archive -Path "${env:TEMP}\Microsoft.VisualStudio.Setup.Configuration.Native.zip" -DestinationPath "${env:TEMP}\Microsoft.VisualStudio.Setup.Configuration.Native" ; \
    C:\Windows\System32\regsvr32.exe /s "${env:TEMP}\Microsoft.VisualStudio.Setup.Configuration.Native\tools\x64\Microsoft.VisualStudio.Setup.Configuration.Native.dll" ; \
    C:\Windows\SysWOW64\regsvr32.exe /s "${env:TEMP}\Microsoft.VisualStudio.Setup.Configuration.Native\tools\x86\Microsoft.VisualStudio.Setup.Configuration.Native.dll"

ENTRYPOINT ["powershell.exe", "-ExecutionPolicy", "Unrestricted"]
CMD ["-NoExit"]
