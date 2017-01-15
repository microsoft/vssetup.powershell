#Require -Version 3.0
[CmdletBinding()]
param ()

if ($PSVersionTable.Version -lt '5.0' -and !(get-module -list 'powershellget')) {
    $url = 'https://download.microsoft.com/download/C/4/1/C41378D4-7F41-4BBE-9D0D-0E4F98585C61/'
    if ([Environment]::Is64BitOperatingSystem) {
        $package = 'PackageManagement_x64.msi'
        $url += $package
    } else {
        $package = 'PackageManagement_x86.msi'
        $url += $package
    }

    write-verbose "Downloading $package"
    invoke-webrequest $url -outfile "${env:TEMP}\$package"

    $logpath = join-path $env:TEMP ($package -replace '.msi$', '.log')

    write-verbose "Installing $package"
    $p = start-process msiexec.exe -args '/i', "${env:TEMP}\$package", '/l*v', "$logpath", '/qn' -passthru
    $p.WaitForExit()

    if ($p.ExitCode) {
        write-error "Failed to install $package`: $($p.ExitCode), log: $logpath"
        exit $p.ExitCode
    }
}
