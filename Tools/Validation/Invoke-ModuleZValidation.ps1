[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet("static", "compile", "editmode", "playmode", "tests", "persistence", "shutdown", "all")]
    [string]$Stage = "tests",

    [string]$UnityPath,

    [string]$ProjectPath = (Resolve-Path(
        (Join-Path $PSScriptRoot "..\..")
    )).Path,

    [string]$OutputPath = (Join-Path(
        (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
    ) "artifacts\validation")
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$script:FailureExitCode = 1

$RequiredUnityPath =
    "C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe"

function Resolve-UnityExecutable {
    if ([string]::IsNullOrWhiteSpace($UnityPath)) {
        $candidate = $RequiredUnityPath
    }
    else {
        $candidate = $UnityPath
    }

    if (-not (Test-Path -LiteralPath $candidate -PathType Leaf)) {
        throw "Unity 6000.3.13f1 executable was not found: $candidate"
    }

    return (Resolve-Path -LiteralPath $candidate).Path
}

function Get-RealProfileRoot {
    $settingsPath = Join-Path $ProjectPath "ProjectSettings\ProjectSettings.asset"
    if (-not (Test-Path -LiteralPath $settingsPath -PathType Leaf)) {
        throw "Project Settings are unavailable; real-profile protection cannot be established."
    }

    $settings = Get-Content -LiteralPath $settingsPath -Raw
    $names = @{}
    foreach ($key in @("companyName", "productName")) {
        $match = [regex]::Match($settings, "(?m)^  $($key): (?<value>.+?)\r?$")
        if (-not $match.Success) {
            throw "Cannot resolve $key; refusing to launch Unity without a profile guard."
        }

        $value = $match.Groups["value"].Value.Trim().Trim('"')
        $value = [regex]::Replace(
            $value,
            '\\x([0-9A-Fa-f]{2})',
            [System.Text.RegularExpressions.MatchEvaluator]{
                param($hexMatch)
                [string][char][Convert]::ToInt32($hexMatch.Groups[1].Value, 16)
            }
        )
        if ([string]::IsNullOrWhiteSpace($value) -or
            $value -eq "." -or $value -eq ".." -or
            $value -match '[\\/:]') {
            throw "Invalid $key; refusing to launch Unity without a profile guard."
        }
        $names[$key] = $value
    }

    if ([string]::IsNullOrWhiteSpace($env:USERPROFILE)) {
        throw "USERPROFILE is unavailable; refusing to launch Unity without a profile guard."
    }
    $localLow = Join-Path $env:USERPROFILE "AppData\LocalLow"
    $companyRoot = Join-Path $localLow $names.companyName
    return [IO.Path]::GetFullPath((Join-Path $companyRoot $names.productName))
}

function Assert-NoReparseAncestors {
    param([Parameter(Mandatory)][string]$Path)

    $cursor = [IO.Path]::GetFullPath($Path)
    while ($true) {
        $item = Get-Item -LiteralPath $cursor -Force -ErrorAction SilentlyContinue
        if ($null -ne $item) {
            if (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
                throw "Reparse point in validation path: $cursor. Refusing to launch Unity."
            }
        }
        elseif (Test-Path -LiteralPath $cursor) {
            throw "Validation path cannot be inspected: $cursor. Refusing to launch Unity."
        }

        $parent = [IO.Directory]::GetParent($cursor)
        if ($null -eq $parent) {
            break
        }
        $cursor = $parent.FullName
    }
}

function Assert-IsolatedRootSafe {
    param(
        [Parameter(Mandatory)][string]$IsolatedRoot,
        [Parameter(Mandatory)][string]$RealProfileRoot
    )

    $isolated = [IO.Path]::GetFullPath($IsolatedRoot).TrimEnd('\', '/')
    $real = [IO.Path]::GetFullPath($RealProfileRoot).TrimEnd('\', '/')
    Assert-NoReparseAncestors $isolated
    Assert-NoReparseAncestors $real

    if ($isolated.Equals($real, [StringComparison]::OrdinalIgnoreCase) -or
        $isolated.StartsWith($real + [IO.Path]::DirectorySeparatorChar,
            [StringComparison]::OrdinalIgnoreCase) -or
        $real.StartsWith($isolated + [IO.Path]::DirectorySeparatorChar,
            [StringComparison]::OrdinalIgnoreCase)) {
        throw "Validation storage overlaps the real profile; refusing to launch Unity."
    }
}

function Get-ProfileFingerprint {
    param([Parameter(Mandatory)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        return [pscustomobject]@{
            Exists = $false
            Size = 0L
            LastWriteUtcTicks = 0L
            Sha256 = ""
        }
    }

    $file = Get-Item -LiteralPath $Path
    return [pscustomobject]@{
        Exists = $true
        Size = $file.Length
        LastWriteUtcTicks = $file.LastWriteTimeUtc.Ticks
        Sha256 = (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash
    }
}

function Assert-RealProfileUnchanged {
    param(
        [Parameter(Mandatory)]$Before,
        [Parameter(Mandatory)][string]$ProfileRoot
    )

    foreach ($fileName in @("modulez_save.json", "module_z_settings.json")) {
        $after = Get-ProfileFingerprint (Join-Path $ProfileRoot $fileName)
        $prior = $Before[$fileName]
        if ($after.Exists -ne $prior.Exists -or
            $after.Size -ne $prior.Size -or
            $after.LastWriteUtcTicks -ne $prior.LastWriteUtcTicks -or
            $after.Sha256 -cne $prior.Sha256) {
            $script:FailureExitCode = 1
            throw "REAL PROFILE CHANGED after Unity exit: $fileName. Validation failed regardless of Unity result XML."
        }
    }

    Write-Host "real-profile guard PASS after Unity exit (save and settings unchanged)"
}

function Invoke-UnityProcess {
    param(
        [Parameter(Mandatory)]
        [string[]]$Arguments,

        [Parameter(Mandatory)]
        [string]$CompletionToken,

        [switch]$RequireExitSave
    )

    $argumentLine = ($Arguments | ForEach-Object {
        if ($_ -match '[\s"]') {
            '"' + $_.Replace('"', '\"') + '"'
        }
        else {
            $_
        }
    }) -join ' '

    $realProfileRoot = Get-RealProfileRoot
    $before = @{}
    foreach ($fileName in @("modulez_save.json", "module_z_settings.json")) {
        $before[$fileName] = Get-ProfileFingerprint (
            Join-Path $realProfileRoot $fileName
        )
    }

    $processRoot = [IO.Path]::GetFullPath((Join-Path $OutputPath (
        Join-Path "process-data" ([Guid]::NewGuid().ToString("N")))))
    Assert-IsolatedRootSafe -IsolatedRoot $processRoot `
        -RealProfileRoot $realProfileRoot
    New-Item -ItemType Directory -Path $processRoot -Force | Out-Null
    Assert-IsolatedRootSafe -IsolatedRoot $processRoot `
        -RealProfileRoot $realProfileRoot

    $environmentName = "MODULEZ_PERSISTENCE_ROOT"
    $markerName = "MODULEZ_VALIDATION_ISOLATED"
    $previousRoot = [Environment]::GetEnvironmentVariable(
        $environmentName, "Process"
    )
    $previousMarker = [Environment]::GetEnvironmentVariable(
        $markerName, "Process"
    )
    try {
        [Environment]::SetEnvironmentVariable(
            $environmentName, $processRoot, "Process"
        )
        [Environment]::SetEnvironmentVariable(
            $markerName, "1", "Process"
        )
        if ([Environment]::GetEnvironmentVariable($environmentName, "Process") -ne
                $processRoot -or
            [Environment]::GetEnvironmentVariable($markerName, "Process") -ne
                "1") {
            throw "Validation isolation environment could not be established."
        }
        Write-Host "Unity process storage isolated under artifacts/validation/process-data."

        $process = Start-Process `
            -FilePath $script:ResolvedUnityPath `
            -ArgumentList $argumentLine `
            -NoNewWindow `
            -PassThru

        $seenProcessIds = [System.Collections.Generic.HashSet[int]]::new()
        [void]$seenProcessIds.Add($process.Id)

        while ($null -ne $process) {
            $process.WaitForExit()
            $exitCode = $process.ExitCode
            $replacement = $null
            $discoveryDeadline = [DateTime]::UtcNow.AddSeconds(3)

            do {
                $replacement = Get-CimInstance Win32_Process `
                    -Filter "Name = 'Unity.exe'" | Where-Object {
                        -not $seenProcessIds.Contains([int]$_.ProcessId) -and
                        $null -ne $_.CommandLine -and
                        $_.CommandLine.IndexOf(
                            $CompletionToken,
                            [StringComparison]::OrdinalIgnoreCase
                        ) -ge 0
                    } | Select-Object -First 1

                if ($null -eq $replacement) {
                    Start-Sleep -Milliseconds 100
                }
            } while (
                $null -eq $replacement -and
                [DateTime]::UtcNow -lt $discoveryDeadline
            )

            if ($null -eq $replacement) {
                return $exitCode
            }

            if ($replacement.CommandLine.IndexOf(
                    "-batchmode",
                    [StringComparison]::OrdinalIgnoreCase
                ) -lt 0) {
                throw "Unity replacement lost -batchmode; persistence isolation cannot be proven."
            }

            [void]$seenProcessIds.Add([int]$replacement.ProcessId)
            $process = [System.Diagnostics.Process]::GetProcessById(
                [int]$replacement.ProcessId
            )
        }

        return $exitCode
    }
    finally {
        [Environment]::SetEnvironmentVariable(
            $environmentName, $previousRoot, "Process"
        )
        [Environment]::SetEnvironmentVariable(
            $markerName, $previousMarker, "Process"
        )
        Assert-RealProfileUnchanged -Before $before -ProfileRoot $realProfileRoot
        if ($RequireExitSave) {
            $exitSave = Join-Path $processRoot "modulez_save.json"
            if (-not (Test-Path -LiteralPath $exitSave -PathType Leaf) -or
                (Get-Item -LiteralPath $exitSave).Length -le 0) {
                $script:FailureExitCode = 1
                throw "Exit-time autosave did not write the isolated process root."
            }
            Write-Host "exit-time autosave PASS (isolated root only)"
        }
    }
}

function Invoke-CompileStage {
    $logPath = Join-Path $OutputPath "compile.log"
    $exitCode = Invoke-UnityProcess `
        -CompletionToken $logPath `
        -Arguments @(
            "-batchmode",
            "-quit",
            "-projectPath", $ProjectPath,
            "-logFile", $logPath
        )

    if ($exitCode -ne 0) {
        $script:FailureExitCode = $exitCode
        throw "Unity compile/import failed with exit code $exitCode. Log: $logPath"
    }

    if (-not (Test-Path -LiteralPath $logPath -PathType Leaf)) {
        throw "Unity compile/import log was not produced: $logPath"
    }

    Write-Host "compile PASS (exit 0): $logPath"
}

function Invoke-StaticStage {
    $validatorPath = Join-Path $PSScriptRoot `
        "Static\Invoke-ModuleZStaticValidation.ps1"
    $resultsPath = Join-Path $OutputPath "static-results.json"
    $logPath = Join-Path $OutputPath "static.log"

    if (-not (Test-Path -LiteralPath $validatorPath -PathType Leaf)) {
        throw "Static validator was not found: $validatorPath"
    }

    Remove-Item -LiteralPath $resultsPath -Force -ErrorAction SilentlyContinue
    Remove-Item -LiteralPath $logPath -Force -ErrorAction SilentlyContinue

    & pwsh -NoProfile -File $validatorPath `
        -RepositoryRoot $ProjectPath `
        -OutputPath $OutputPath
    $exitCode = $LASTEXITCODE

    if ($exitCode -ne 0) {
        $script:FailureExitCode = $exitCode
        throw "Static validation failed with exit code $exitCode. Log: $logPath"
    }

    if (-not (Test-Path -LiteralPath $resultsPath -PathType Leaf)) {
        throw "Static result JSON was not produced: $resultsPath"
    }

    $result = Get-Content -LiteralPath $resultsPath -Raw | ConvertFrom-Json
    if ($result.overall -ne "PASS") {
        throw "Static result JSON reports $($result.overall), not PASS."
    }

    if (-not (Test-Path -LiteralPath $logPath -PathType Leaf)) {
        throw "Static validation log was not produced: $logPath"
    }

    Write-Host "static PASS: $resultsPath; $logPath"
}

function Assert-TestResults {
    param(
        [Parameter(Mandatory)]
        [string]$ResultsPath,

        [Parameter(Mandatory)]
        [string]$TestPlatform,

        [Parameter(Mandatory)]
        [string]$ExpectedAssembly
    )

    if (-not (Test-Path -LiteralPath $ResultsPath -PathType Leaf)) {
        throw "$TestPlatform result XML was not produced: $ResultsPath"
    }

    [xml]$document = Get-Content -LiteralPath $ResultsPath -Raw
    $testRun = $document.'test-run'
    if ($null -eq $testRun) {
        throw "$TestPlatform result XML has no test-run root."
    }

    $assemblyNode = $testRun.SelectSingleNode(
        ".//test-suite[@type='Assembly' and @name='$ExpectedAssembly.dll']"
    )
    if ($null -eq $assemblyNode) {
        throw "$TestPlatform did not discover $ExpectedAssembly."
    }

    $testCases = @($assemblyNode.SelectNodes(".//test-case"))
    $total = $testCases.Count
    $passed = @($testCases | Where-Object { $_.result -eq "Passed" }).Count
    $failed = $total - $passed

    if ($total -lt 1) {
        throw "$TestPlatform discovered zero tests."
    }

    if ($failed -ne 0 -or $passed -ne $total) {
        throw "$TestPlatform tests failed: total=$total passed=$passed failed=$failed."
    }

    Write-Host "$TestPlatform PASS ($ExpectedAssembly): total=$total passed=$passed failed=$failed"
}

function Invoke-TestStage {
    param(
        [Parameter(Mandatory)]
        [ValidateSet("EditMode", "PlayMode")]
        [string]$TestPlatform,

        [string]$CategoryFilter,

        [string]$ArtifactName
    )

    if ([string]::IsNullOrWhiteSpace($ArtifactName)) {
        $ArtifactName = $TestPlatform.ToLowerInvariant()
    }
    $expectedAssembly = "ModuleZ.$($TestPlatform)Tests"
    $resultsPath = Join-Path $OutputPath "$ArtifactName-results.xml"
    $logPath = Join-Path $OutputPath "$ArtifactName.log"

    Remove-Item -LiteralPath $resultsPath -Force -ErrorAction SilentlyContinue

    $arguments = @(
        "-batchmode",
        "-projectPath", $ProjectPath,
        "-runTests",
        "-testPlatform", $TestPlatform,
        "-testResults", $resultsPath,
        "-logFile", $logPath
    )
    if (-not [string]::IsNullOrWhiteSpace($CategoryFilter)) {
        $arguments += @("-testCategory", $CategoryFilter)
    }

    $exitCode = Invoke-UnityProcess `
        -CompletionToken $logPath `
        -Arguments $arguments `
        -RequireExitSave:($ArtifactName -eq "shutdown")

    if ($exitCode -ne 0) {
        $script:FailureExitCode = $exitCode
        throw "Unity $TestPlatform tests failed with exit code $exitCode. Log: $logPath"
    }

    if (-not (Test-Path -LiteralPath $logPath -PathType Leaf)) {
        throw "$TestPlatform log was not produced: $logPath"
    }

    $displayName = if ($ArtifactName -eq "shutdown") { "Shutdown" }
        elseif ($ArtifactName -eq "persistence") { "Persistence" }
        else { $TestPlatform }
    Assert-TestResults `
        -ResultsPath $resultsPath `
        -TestPlatform $displayName `
        -ExpectedAssembly $expectedAssembly
    Write-Host "$displayName artifacts: $resultsPath; $logPath"
}

try {
    $script:ResolvedUnityPath = Resolve-UnityExecutable
    $ProjectPath = (Resolve-Path -LiteralPath $ProjectPath).Path
    Assert-NoReparseAncestors $OutputPath
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Assert-NoReparseAncestors $OutputPath
    $OutputPath = (Resolve-Path -LiteralPath $OutputPath).Path

    switch ($Stage) {
        "static" {
            Invoke-StaticStage
        }
        "compile" {
            Invoke-CompileStage
        }
        "editmode" {
            Invoke-TestStage -TestPlatform "EditMode"
        }
        "playmode" {
            Invoke-TestStage -TestPlatform "PlayMode" -CategoryFilter "!Shutdown"
        }
        "tests" {
            Invoke-TestStage -TestPlatform "EditMode"
            Invoke-TestStage -TestPlatform "PlayMode" -CategoryFilter "!Shutdown"
        }
        "persistence" {
            Invoke-TestStage -TestPlatform "PlayMode" `
                -CategoryFilter "Persistence" -ArtifactName "persistence"
        }
        "shutdown" {
            Invoke-TestStage -TestPlatform "PlayMode" `
                -CategoryFilter "Shutdown" -ArtifactName "shutdown"
        }
        "all" {
            Invoke-StaticStage
            Invoke-CompileStage
            Invoke-TestStage -TestPlatform "EditMode"
            Invoke-TestStage -TestPlatform "PlayMode" -CategoryFilter "!Shutdown"
        }
    }
}
catch {
    Write-Error $_ -ErrorAction Continue
    exit $script:FailureExitCode
}

exit 0
