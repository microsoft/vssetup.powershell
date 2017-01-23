@echo off

REM Copyright (C) Microsoft Corporation. All rights reserved.
REM Licensed under the MIT license. See LICENSE.txt in the project root for license information.

setlocal

%~dp0run.cmd %* -- -c Invoke-Pester C:\Tests -EnableExit
