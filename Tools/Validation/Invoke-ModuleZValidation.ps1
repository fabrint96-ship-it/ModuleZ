[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet("static", "compile", "editmode", "playmode", "tests", "all")]
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

function Invoke-UnityProcess {
    param(
        [Parameter(Mandatory)]
        [string[]]$Arguments,

        [Parameter(Mandatory)]
        [string]$CompletionToken
    )

    $argumentLine = ($Arguments | ForEach-Object {
        if ($_ -match '[\s"]') {
            '"' + $_.Replace('"', '\"') + '"'
        }
        else {
            $_
        }
    }) -join ' '

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

        [void]$seenProcessIds.Add([int]$replacement.ProcessId)
        $process = [System.Diagnostics.Process]::GetProcessById(
            [int]$replacement.ProcessId
        )
    }

    return $exitCode
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
        [string]$TestPlatform
    )

    $artifactName = $TestPlatform.ToLowerInvariant()
    $expectedAssembly = "ModuleZ.$($TestPlatform)Tests"
    $resultsPath = Join-Path $OutputPath "$artifactName-results.xml"
    $logPath = Join-Path $OutputPath "$artifactName.log"

    Remove-Item -LiteralPath $resultsPath -Force -ErrorAction SilentlyContinue

    $exitCode = Invoke-UnityProcess `
        -CompletionToken $logPath `
        -Arguments @(
            "-batchmode",
            "-projectPath", $ProjectPath,
            "-runTests",
            "-testPlatform", $TestPlatform,
            "-testResults", $resultsPath,
            "-logFile", $logPath
        )

    if ($exitCode -ne 0) {
        $script:FailureExitCode = $exitCode
        throw "Unity $TestPlatform tests failed with exit code $exitCode. Log: $logPath"
    }

    if (-not (Test-Path -LiteralPath $logPath -PathType Leaf)) {
        throw "$TestPlatform log was not produced: $logPath"
    }

    Assert-TestResults `
        -ResultsPath $resultsPath `
        -TestPlatform $TestPlatform `
        -ExpectedAssembly $expectedAssembly
    Write-Host "$TestPlatform artifacts: $resultsPath; $logPath"
}

try {
    $script:ResolvedUnityPath = Resolve-UnityExecutable
    $ProjectPath = (Resolve-Path -LiteralPath $ProjectPath).Path
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
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
            Invoke-TestStage -TestPlatform "PlayMode"
        }
        "tests" {
            Invoke-TestStage -TestPlatform "EditMode"
            Invoke-TestStage -TestPlatform "PlayMode"
        }
        "all" {
            Invoke-StaticStage
            Invoke-CompileStage
            Invoke-TestStage -TestPlatform "EditMode"
            Invoke-TestStage -TestPlatform "PlayMode"
        }
    }
}
catch {
    Write-Error $_ -ErrorAction Continue
    exit $script:FailureExitCode
}

exit 0
