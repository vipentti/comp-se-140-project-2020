#!/usr/bin/env pwsh
[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $BuildConfig = "",

    [Parameter()]
    [string]
    $Project = "./amqp.sln",

    [Parameter()]
    [switch]
    $Build,

    [Parameter()]
    [string]
    $Filter = "$($env:TEST_FILTER)",

    [Parameter()]
    [string]
    $ResultsDirectory = "./testresults"
)

$ThisPath = $PSScriptRoot
$InvokeScript = Join-Path -Path $ThisPath "invoke-script.ps1"

if (!(Test-Path $ResultsDirectory)) {
    New-Item -ItemType Directory -Force -Path $ResultsDirectory
}

if ($BuildConfig.Length -eq 0) {
    if ("$env:BUILD_CONFIG" -gt 0) {
        $BuildConfig = "$env:BUILD_CONFIG"
    }
    elseif ("$env:ENV_BUILD_CONFIG" -gt 0) {
        $BuildConfig = "$env:ENV_BUILD_CONFIG"
    }
    else {
        $BuildConfig = "Release"
    }
}

$ResultsDirectory = $(Resolve-Path $ResultsDirectory)
$buildArg = $Build ? "" : "--no-build"

$Now = (Get-Date -Format "yyyyMMdd")

$LogPath = "$ResultsDirectory/${Now}_test_log.txt"

$filters = ""

# If filter argument was provided
if ($Filter.Length -gt 0) {
    $filters = "--filter $Filter"
}

# Logger arguments based on
# https://docs.gitlab.com/ee/ci/unit_test_reports.html#net-example
$junitLogger = "--logger:`"junit;LogFilePath=${ResultsDirectory}/{assembly}-test-result.xml;MethodFormat=Class;FailureBodyFormat=Verbose`""

$runCommand =
@"
dotnet test -c ${BuildConfig} ``
    ${buildArg} ``
    ${filters} ``
    ${junitLogger} ``
    --logger:`"console;verbosity=detailed`" ``
    --results-directory ${ResultsDirectory} /p:CollectCoverage=true ``
    ${Project}
"@

Write-Output "Running command: $runCommand" | Out-File -FilePath "$LogPath" -Append

&$InvokeScript -Command $runCommand -LogPath $LogPath

Write-Output "Returned $?. Wrote logs to $LogPath"

exit $LASTEXITCODE
