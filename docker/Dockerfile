# Copyright (C) Microsoft Corporation. All rights reserved.
# Licensed under the MIT license. See LICENSE.txt in the project root for license information.

FROM heaths/vssetup:1.8.24
SHELL ["powershell.exe", "-ExecutionPolicy", "Bypass", "-Command"]

# Download and install Remote Debugger
RUN $ErrorActionPreference = 'Stop' ; \
    $VerbosePreference = 'Continue' ; \
    New-Item -Path C:\Downloads -Type Directory | Out-Null ; \
    Invoke-WebRequest -Uri 'https://go.microsoft.com/fwlink/?LinkId=615470&clcid=0x409' -OutFile C:\Downloads\rtools_setup_x64.exe ; \
    Start-Process -Wait -FilePath C:\Downloads\rtools_setup_x64.exe -ArgumentList '-q' ; \
    Remove-Item -Path C:\Downloads\rtools_setup_x64.exe

# Configure Remote Debugger
EXPOSE 3702 4020 4021
RUN $ErrorActionPreference = 'Stop' ; \
    $VerbosePreference = 'Continue' ; \
    Start-Process -Wait -FilePath 'C:\Program Files\Microsoft Visual Studio 14.0\Common7\IDE\Remote Debugger\x64\msvsmon.exe' -ArgumentList '/prepcomputer', '/private', '/quiet'

# Start Remote Debugger if no other command is passed to PowerShell
ENTRYPOINT ["powershell.exe", "-ExecutionPolicy", "Unrestricted"]
CMD ["-NoExit", "-Command", "& 'C:\\Program Files\\Microsoft Visual Studio 14.0\\Common7\\IDE\\Remote Debugger\\x64\\msvsmon.exe' /silent /noauth /anyuser"]
