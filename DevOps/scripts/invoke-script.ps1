[CmdletBinding()]
param (
    [Parameter(Mandatory = $True)]
    [string]
    $Command,

    [Parameter()]
    [switch]
    $Quiet,

    [Parameter(Mandatory = $False)]
    [string]
    $LogPath = ""
)

if (-not ($Command -match '\;\$\?$')) {
    $Command += ';$Success=$?;$ExitCode=$LASTEXITCODE'
}

$Verbose = -not $Quiet

if ($Verbose) {
    Write-Output "Running using script $Command'"
}

if ($LogPath -eq "") {
    Invoke-Expression -Command $Command
} else {
    Invoke-Expression -Command $Command | Tee-Object "$LogPath" -Append
}

if ($Verbose) {
    Write-Host "Command returned $Success ($ExitCode)"
}

exit $ExitCode
