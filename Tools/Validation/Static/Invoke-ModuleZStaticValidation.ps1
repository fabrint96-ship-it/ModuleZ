[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Resolve-Path(
        (Join-Path $PSScriptRoot "..\..\..")
    )).Path,

    [string]$OutputPath = (Join-Path(
        (Resolve-Path (Join-Path $PSScriptRoot "..\..\..")).Path
    ) "artifacts\validation")
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$ValidatorVersion = "1.0.0"
$ResultSchemaVersion = 1
$BaselinePath = Join-Path $PSScriptRoot "ModuleZ.StaticBaseline.json"
$Results = [System.Collections.Generic.List[object]]::new()
$LogLines = [System.Collections.Generic.List[string]]::new()

function Convert-ToRepositoryPath {
    param([Parameter(Mandatory)][string]$Path)

    return $Path.Substring($RepositoryRoot.Length + 1).Replace("\", "/")
}

function Read-RepositoryText {
    param([Parameter(Mandatory)][string]$RelativePath)

    $path = Join-Path $RepositoryRoot $RelativePath
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Required repository file is missing: $RelativePath"
    }

    return [System.IO.File]::ReadAllText($path)
}

function Get-MatchCount {
    param(
        [Parameter(Mandatory)][string]$Text,
        [Parameter(Mandatory)][string]$Pattern,
        [System.Text.RegularExpressions.RegexOptions]$Options =
            [System.Text.RegularExpressions.RegexOptions]::None
    )

    return [regex]::Matches($Text, $Pattern, $Options).Count
}

function Add-Result {
    param(
        [Parameter(Mandatory)][string]$Id,
        [Parameter(Mandatory)][string]$Name,
        [Parameter(Mandatory)]
        [ValidateSet("STATIC_EXACT", "STATIC_HEURISTIC")]
        [string]$Classification,
        [Parameter(Mandatory)]
        [ValidateSet("PASS", "FAIL", "WARN", "SKIP")]
        [string]$Status,
        [Parameter(Mandatory)][bool]$Gating,
        [Parameter(Mandatory)][string]$Message,
        [string[]]$Evidence = @(),
        [string]$BaselineUse = "none"
    )

    $result = [ordered]@{
        id = $Id
        name = $Name
        category = ($Id -replace '^STATIC-', 'static-')
        classification = $Classification
        status = $Status
        gating = $Gating
        message = $Message
        evidence = @($Evidence)
        baselineUse = $BaselineUse
    }
    $Results.Add([pscustomobject]$result)

    $line = "[$Status] $Id $Name"
    if ($Status -in @("FAIL", "WARN")) {
        $line += ": $Message"
    }
    $LogLines.Add($line)
    Write-Host $line
}

function Add-BooleanCheck {
    param(
        [string]$Id,
        [string]$Name,
        [string]$Classification,
        [bool]$Gating,
        [bool]$Passed,
        [string]$PassMessage,
        [string]$FailMessage,
        [string[]]$Evidence = @(),
        [string]$BaselineUse = "none"
    )

    Add-Result `
        -Id $Id `
        -Name $Name `
        -Classification $Classification `
        -Status $(if ($Passed) { "PASS" } else { "FAIL" }) `
        -Gating $Gating `
        -Message $(if ($Passed) { $PassMessage } else { $FailMessage }) `
        -Evidence $Evidence `
        -BaselineUse $BaselineUse
}

function Get-ProductionCSharpFiles {
    $root = Join-Path $RepositoryRoot "Assets\ModuleZ"
    return @(Get-ChildItem -LiteralPath $root -Recurse -Filter "*.cs" -File |
        Where-Object { $_.FullName -notmatch '[\\/]Tests[\\/]' })
}

function Add-Occurrence {
    param(
        [Parameter(Mandatory)][hashtable]$Table,
        [Parameter(Mandatory)][string]$Key,
        [int]$Amount = 1
    )

    if ($Table.ContainsKey($Key)) {
        $Table[$Key] += $Amount
    }
    else {
        $Table[$Key] = $Amount
    }
}

function Test-AllowlistRatchet {
    param(
        [string]$Id,
        [string]$Name,
        [string]$Classification,
        [bool]$Gating,
        [hashtable]$Current,
        [hashtable]$Allowed,
        [string]$BaselineSection,
        [ValidateSet("FAIL", "WARN")][string]$ViolationStatus
    )

    $violations = [System.Collections.Generic.List[string]]::new()
    foreach ($key in $Current.Keys | Sort-Object) {
        $allowedCount = if ($Allowed.ContainsKey($key)) {
            [int]$Allowed[$key]
        }
        else {
            0
        }

        if ([int]$Current[$key] -gt $allowedCount) {
            $violations.Add(
                "$key current=$($Current[$key]) allowed=$allowedCount"
            )
        }
    }

    $currentCount = ($Current.Values | Measure-Object -Sum).Sum
    if ($null -eq $currentCount) { $currentCount = 0 }
    $allowedCountTotal = ($Allowed.Values | Measure-Object -Sum).Sum
    if ($null -eq $allowedCountTotal) { $allowedCountTotal = 0 }

    if ($violations.Count -eq 0) {
        Add-Result `
            -Id $Id `
            -Name $Name `
            -Classification $Classification `
            -Status "PASS" `
            -Gating $Gating `
            -Message "$currentCount current occurrence(s); $allowedCountTotal reviewed baseline allowance(s); removed debt is allowed." `
            -Evidence @($Current.Keys | Sort-Object) `
            -BaselineUse "ModuleZ.StaticBaseline.json:$BaselineSection"
    }
    else {
        Add-Result `
            -Id $Id `
            -Name $Name `
            -Classification $Classification `
            -Status $ViolationStatus `
            -Gating $Gating `
            -Message "Unapproved occurrence growth detected." `
            -Evidence @($violations) `
            -BaselineUse "ModuleZ.StaticBaseline.json:$BaselineSection"
    }
}

function Get-MethodBody {
    param(
        [Parameter(Mandatory)][string]$Text,
        [Parameter(Mandatory)][string]$SignaturePattern
    )

    $signature = [regex]::Match($Text, $SignaturePattern)
    if (-not $signature.Success) { return $null }

    $openingBrace = $Text.IndexOf("{", $signature.Index + $signature.Length)
    if ($openingBrace -lt 0) { return $null }

    $depth = 0
    for ($index = $openingBrace; $index -lt $Text.Length; $index++) {
        if ($Text[$index] -eq '{') { $depth++ }
        elseif ($Text[$index] -eq '}') {
            $depth--
            if ($depth -eq 0) {
                return $Text.Substring(
                    $openingBrace + 1,
                    $index - $openingBrace - 1
                )
            }
        }
    }

    return $null
}

try {
    $RepositoryRoot = (Resolve-Path -LiteralPath $RepositoryRoot).Path
    if (-not (Test-Path -LiteralPath $BaselinePath -PathType Leaf)) {
        throw "Static baseline is missing: $BaselinePath"
    }

    $Baseline = Get-Content -LiteralPath $BaselinePath -Raw |
        ConvertFrom-Json
    if ([int]$Baseline.schemaVersion -ne 1) {
        throw "Unsupported static baseline schema: $($Baseline.schemaVersion)"
    }

    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    $OutputPath = (Resolve-Path -LiteralPath $OutputPath).Path
    $ResultPath = Join-Path $OutputPath "static-results.json"
    $LogPath = Join-Path $OutputPath "static.log"

    $productionFiles = Get-ProductionCSharpFiles
    $buildSettingsPath = "ProjectSettings/EditorBuildSettings.asset"
    $openWorldScenePath = "Assets/ModuleZ/Scenes/OpenWorld.unity"
    $duelScenePath = "Assets/ModuleZ/Scenes/Duel.unity"
    $openWorldScene = Read-RepositoryText $openWorldScenePath
    $duelScene = Read-RepositoryText $duelScenePath

    # STATIC-001
    $buildSettings = Read-RepositoryText $buildSettingsPath
    $enabledScenes = @([regex]::Matches(
        $buildSettings,
        '- enabled:\s*1\s*\r?\n\s*path:\s*([^\r\n]+)'
    ) | ForEach-Object { $_.Groups[1].Value.Trim() })
    $expectedScenes = @(
        "Assets/ModuleZ/Scenes/Boot.unity",
        "Assets/ModuleZ/Scenes/OpenWorld.unity",
        "Assets/ModuleZ/Scenes/Duel.unity"
    )
    $sceneOrderMatches =
        $enabledScenes.Count -eq $expectedScenes.Count -and
        @(Compare-Object $expectedScenes $enabledScenes -SyncWindow 0).Count -eq 0
    Add-BooleanCheck "STATIC-001" "Build scene order" "STATIC_EXACT" $true `
        $sceneOrderMatches "Enabled scenes exactly match Boot, OpenWorld, Duel." `
        "Enabled scene order differs from Boot, OpenWorld, Duel." $enabledScenes

    # STATIC-002
    $debugEvidence = [System.Collections.Generic.List[string]]::new()
    $debugPatterns = @(
        "Duel3DDebugBootstrap",
        "Duel3D_DebugBootstrap",
        "7b34cdc96a938584e816e44d085e3a90"
    )
    $productionAssetExtensions = @("*.cs", "*.unity", "*.prefab", "*.asset")
    foreach ($extension in $productionAssetExtensions) {
        Get-ChildItem (Join-Path $RepositoryRoot "Assets\ModuleZ") -Recurse `
            -Filter $extension -File | Where-Object {
                $_.FullName -notmatch '[\\/]Tests[\\/]'
            } | ForEach-Object {
                $text = [System.IO.File]::ReadAllText($_.FullName)
                foreach ($pattern in $debugPatterns) {
                    if ($text.Contains($pattern)) {
                        $debugEvidence.Add(
                            "$(Convert-ToRepositoryPath $_.FullName)|$pattern"
                        )
                    }
                }
            }
    }
    Add-BooleanCheck "STATIC-002" "No production Duel DebugBootstrap" `
        "STATIC_EXACT" $true ($debugEvidence.Count -eq 0) `
        "Production assets contain no debug-bootstrap symbol, name, or legacy GUID." `
        "Production debug-bootstrap reference found." @($debugEvidence)

    # STATIC-003 / 004
    $authoredCameraCount = Get-MatchCount $openWorldScene '^--- !u!20 &' `
        ([Text.RegularExpressions.RegexOptions]::Multiline)
    $authoredMainCameraNames = Get-MatchCount $openWorldScene `
        '^\s*m_Name:\s*Main Camera\s*$' `
        ([Text.RegularExpressions.RegexOptions]::Multiline)
    $authoredMainCameraTags = Get-MatchCount $openWorldScene `
        '^\s*m_TagString:\s*MainCamera\s*$' `
        ([Text.RegularExpressions.RegexOptions]::Multiline)
    Add-BooleanCheck "STATIC-003" "OpenWorld authored camera absence" `
        "STATIC_EXACT" $true `
        ($authoredCameraCount -eq 0 -and $authoredMainCameraNames -eq 0 -and $authoredMainCameraTags -eq 0) `
        "OpenWorld YAML has zero Camera components, Main Camera names, and MainCamera tags." `
        "OpenWorld YAML contains an authored camera ownership marker." `
        @("Camera=$authoredCameraCount", "Main Camera=$authoredMainCameraNames", "MainCamera tag=$authoredMainCameraTags", $openWorldScenePath)

    $authoredListenerCount = Get-MatchCount $openWorldScene '^--- !u!81 &' `
        ([Text.RegularExpressions.RegexOptions]::Multiline)
    Add-BooleanCheck "STATIC-004" "OpenWorld authored AudioListener absence" `
        "STATIC_EXACT" $true ($authoredListenerCount -eq 0) `
        "OpenWorld YAML has zero AudioListener components." `
        "OpenWorld YAML contains an authored AudioListener." `
        @("AudioListener=$authoredListenerCount", $openWorldScenePath)

    # STATIC-005 / 006
    $openWorldRuntimeFiles = @(Get-ChildItem `
        (Join-Path $RepositoryRoot "Assets\ModuleZ\OpenWorld") -Recurse `
        -Filter "*.cs" -File)
    $openWorldRuntimeText = ($openWorldRuntimeFiles | ForEach-Object {
        [System.IO.File]::ReadAllText($_.FullName)
    }) -join "`n"
    $runtimeBuilderText = Read-RepositoryText `
        "Assets/ModuleZ/OpenWorld/Runtime/OpenWorldRuntimeBuilder.cs"
    $cameraBuilderText = Read-RepositoryText `
        "Assets/ModuleZ/OpenWorld/Runtime/OpenWorldGameplayCameraBuilder.cs"
    $cameraDelegateCount = Get-MatchCount $runtimeBuilderText `
        'OpenWorldGameplayCameraBuilder\s*\.\s*Create\s*\('
    $mainCameraCreationCount = Get-MatchCount $openWorldRuntimeText `
        'new\s+GameObject\s*\(\s*"Main Camera"\s*\)'
    $cameraComponentCreationCount = Get-MatchCount $openWorldRuntimeText `
        'AddComponent\s*<\s*Camera\s*>\s*\('
    Add-BooleanCheck "STATIC-005" "OpenWorld gameplay camera creation path" `
        "STATIC_EXACT" $true `
        ($cameraDelegateCount -eq 1 -and $mainCameraCreationCount -eq 1 -and $cameraComponentCreationCount -eq 1) `
        "One builder delegation, Main Camera construction, and Camera component path remain." `
        "OpenWorld gameplay camera creation topology is not singular." `
        @("delegation=$cameraDelegateCount", "Main Camera creation=$mainCameraCreationCount", "Camera AddComponent=$cameraComponentCreationCount")

    $listenerCreationCount = Get-MatchCount $openWorldRuntimeText `
        'AddComponent\s*<\s*AudioListener\s*>\s*\('
    $listenerInCameraBuilder = Get-MatchCount $cameraBuilderText `
        'AddComponent\s*<\s*AudioListener\s*>\s*\('
    Add-BooleanCheck "STATIC-006" "OpenWorld AudioListener creation path" `
        "STATIC_EXACT" $true `
        ($listenerCreationCount -eq 1 -and $listenerInCameraBuilder -eq 1) `
        "Exactly one OpenWorld AudioListener creation exists in the gameplay-camera builder." `
        "OpenWorld AudioListener creation topology is not singular." `
        @("OpenWorld=$listenerCreationCount", "camera builder=$listenerInCameraBuilder")

    # STATIC-007 / 008
    $startPattern = '\b(?:public|private|protected|internal)?\s*(?:void|IEnumerator)\s+Start\s*\('
    $openWorldBuilderStarts = Get-MatchCount $runtimeBuilderText $startPattern
    Add-BooleanCheck "STATIC-007" "OpenWorld RuntimeBuilder no Start lifecycle" `
        "STATIC_EXACT" $true ($openWorldBuilderStarts -eq 0) `
        "OpenWorldRuntimeBuilder declares no Start lifecycle entry." `
        "OpenWorldRuntimeBuilder regained a Start lifecycle entry." `
        @("Start declarations=$openWorldBuilderStarts")

    $duelRuntimeBuilderPath =
        "Assets/ModuleZ/Duel3D/Runtime/Duel3DRuntimeBuilder.cs"
    $duelRuntimeBuilderText = Read-RepositoryText $duelRuntimeBuilderPath
    $duelBuilderStarts = Get-MatchCount $duelRuntimeBuilderText $startPattern
    Add-BooleanCheck "STATIC-008" "Duel RuntimeBuilder no Start lifecycle" `
        "STATIC_EXACT" $true ($duelBuilderStarts -eq 0) `
        "Duel3DRuntimeBuilder declares no Start lifecycle entry." `
        "Duel3DRuntimeBuilder regained a Start lifecycle entry." `
        @("Start declarations=$duelBuilderStarts")

    # STATIC-009 / 010 / 011
    function Get-ScriptGuid([string]$MetaPath) {
        $meta = Read-RepositoryText $MetaPath
        $match = [regex]::Match($meta, '(?m)^guid:\s*([a-f0-9]+)\s*$')
        if (-not $match.Success) { throw "GUID missing from $MetaPath" }
        return $match.Groups[1].Value
    }
    $openWorldRootGuid = Get-ScriptGuid `
        "Assets/ModuleZ/OpenWorld/Runtime/OpenWorldSceneRoot.cs.meta"
    $openWorldRootCount = Get-MatchCount $openWorldScene `
        "guid:\s*$openWorldRootGuid"
    Add-BooleanCheck "STATIC-009" "One OpenWorldSceneRoot serialized owner" `
        "STATIC_EXACT" $true ($openWorldRootCount -eq 1) `
        "OpenWorld serializes exactly one OpenWorldSceneRoot." `
        "OpenWorldSceneRoot serialized owner count is not one." `
        @("guid=$openWorldRootGuid", "count=$openWorldRootCount", $openWorldScenePath)

    $duelRootGuid = Get-ScriptGuid `
        "Assets/ModuleZ/Duel3D/Runtime/DuelSceneRoot.cs.meta"
    $duelRootCount = Get-MatchCount $duelScene "guid:\s*$duelRootGuid"
    Add-BooleanCheck "STATIC-010" "One DuelSceneRoot serialized owner" `
        "STATIC_EXACT" $true ($duelRootCount -eq 1) `
        "Duel serializes exactly one DuelSceneRoot." `
        "DuelSceneRoot serialized owner count is not one." `
        @("guid=$duelRootGuid", "count=$duelRootCount", $duelScenePath)

    $duelBootstrapGuid = Get-ScriptGuid `
        "Assets/ModuleZ/Duel3D/Runtime/DuelBootstrap.cs.meta"
    $duelBootstrapCount = Get-MatchCount $duelScene `
        "guid:\s*$duelBootstrapGuid"
    Add-BooleanCheck "STATIC-011" "DuelBootstrap production composition" `
        "STATIC_EXACT" $true ($duelBootstrapCount -eq 1) `
        "Duel serializes exactly one production DuelBootstrap." `
        "DuelBootstrap serialized owner count is not one." `
        @("guid=$duelBootstrapGuid", "count=$duelBootstrapCount", $duelScenePath)

    # STATIC-012
    $testScriptGuids = [System.Collections.Generic.List[string]]::new()
    Get-ChildItem (Join-Path $RepositoryRoot "Assets\ModuleZ\Tests") `
        -Recurse -Filter "*.cs.meta" -File | ForEach-Object {
            $match = [regex]::Match(
                [System.IO.File]::ReadAllText($_.FullName),
                '(?m)^guid:\s*([a-f0-9]+)\s*$'
            )
            if ($match.Success) { $testScriptGuids.Add($match.Groups[1].Value) }
        }
    $testSceneEvidence = [System.Collections.Generic.List[string]]::new()
    Get-ChildItem (Join-Path $RepositoryRoot "Assets\ModuleZ\Scenes") `
        -Filter "*.unity" -File | ForEach-Object {
            $sceneText = [System.IO.File]::ReadAllText($_.FullName)
            foreach ($guid in $testScriptGuids) {
                if ($sceneText -match "guid:\s*$guid") {
                    $testSceneEvidence.Add(
                        "$(Convert-ToRepositoryPath $_.FullName)|$guid"
                    )
                }
            }
        }
    Add-BooleanCheck "STATIC-012" "No production test components" `
        "STATIC_EXACT" $true ($testSceneEvidence.Count -eq 0) `
        "Production scenes serialize no scripts from Assets/ModuleZ/Tests." `
        "A production scene serializes a test script." @($testSceneEvidence)

    # Build ratchet occurrence sets.
    $ddolCurrent = @{}
    $inputCurrent = @{}
    $lookupCurrent = @{}
    $coreDependencyCurrent = @{}
    $globalCurrent = @{}
    $hardcodedCurrent = @{}
    foreach ($file in $productionFiles) {
        $path = Convert-ToRepositoryPath $file.FullName
        $text = [System.IO.File]::ReadAllText($file.FullName)
        foreach ($match in [regex]::Matches(
            $text,
            'DontDestroyOnLoad\s*\(\s*(?<argument>[^\)]+?)\s*\)'
        )) {
            $expression = "DontDestroyOnLoad($($match.Groups['argument'].Value -replace '\s+', ''))"
            Add-Occurrence $ddolCurrent "$path|$expression"
        }
        foreach ($match in [regex]::Matches(
            $text,
            '\b(?:UnityEngine\.)?Input\.(?<api>[A-Za-z0-9_]+)'
        )) {
            Add-Occurrence $inputCurrent "$path|$($match.Groups['api'].Value)"
        }
        foreach ($match in [regex]::Matches(
            $text,
            '\b(?<api>FindObjectOfType|FindFirstObjectByType|FindAnyObjectByType|GameObject\.Find(?:GameObjectWithTag|GameObjectsWithTag)?)\s*(?:<[^>]+>)?\s*\('
        )) {
            Add-Occurrence $lookupCurrent "$path|$($match.Groups['api'].Value)"
        }
        if ($path.StartsWith("Assets/ModuleZ/Core/")) {
            foreach ($match in [regex]::Matches(
                $text,
                'ModuleZ\.(?<domain>OpenWorld|Game)(?:\.[A-Za-z0-9_]+)*'
            )) {
                Add-Occurrence $coreDependencyCurrent `
                    "$path|$($match.Groups['domain'].Value)"
            }
        }
        foreach ($line in ($text -split "\r?\n")) {
            $field = [regex]::Match(
                $line,
                '^\s*(?:public|internal|private|protected)\s+static\s+(?!readonly\b)(?<type>[A-Za-z0-9_.<>\[\],?]+)\s+(?<name>[A-Za-z0-9_]+)\s*(?:=(?!>)|;)'
            )
            if ($field.Success) {
                Add-Occurrence $globalCurrent `
                    "$path|$($field.Groups['name'].Value)"
            }
            $property = [regex]::Match(
                $line,
                '^\s*(?:public|internal|private|protected)\s+static\s+[A-Za-z0-9_.<>\[\],?]+\s+(?<name>[A-Za-z0-9_]+)\s*\{[^}]*\bset;'
            )
            if ($property.Success) {
                Add-Occurrence $globalCurrent `
                    "$path|$($property.Groups['name'].Value)"
            }
            $visible = [regex]::Match(
                $line,
                '\.(?:text)\s*=\s*"(?<literal>(?:\\.|[^"\r\n])*)"'
            )
            if (-not $visible.Success) {
                $visible = [regex]::Match(
                    $line,
                    '\.SetText\s*\(\s*"(?<literal>(?:\\.|[^"\r\n])*)"'
                )
            }
            if ($visible.Success -and
                -not [string]::IsNullOrEmpty($visible.Groups['literal'].Value)) {
                Add-Occurrence $hardcodedCurrent `
                    "$path|$($visible.Groups['literal'].Value)"
            }
        }
    }

    $ddolAllowed = @{}
    foreach ($entry in $Baseline.ddolAllowlist) {
        Add-Occurrence $ddolAllowed "$($entry.file)|$($entry.expression)" `
            ([int]$entry.count)
    }
    $inputAllowed = @{}
    foreach ($entry in $Baseline.legacyInputAllowlist) {
        Add-Occurrence $inputAllowed "$($entry.file)|$($entry.api)" `
            ([int]$entry.count)
    }
    $lookupAllowed = @{}
    foreach ($entry in $Baseline.objectLookupAllowlist) {
        Add-Occurrence $lookupAllowed "$($entry.file)|$($entry.api)" `
            ([int]$entry.count)
    }
    $coreAllowed = @{}
    foreach ($entry in $Baseline.coreDependencyAllowlist) {
        Add-Occurrence $coreAllowed "$($entry.file)|$($entry.domain)" `
            ([int]$entry.count)
    }
    $globalAllowed = @{}
    foreach ($entry in $Baseline.globalMutableAllowlist) {
        foreach ($symbol in $entry.symbols) {
            Add-Occurrence $globalAllowed "$($entry.file)|$symbol"
        }
    }
    $hardcodedAllowed = @{}
    foreach ($entry in $Baseline.hardcodedUiAllowlist) {
        foreach ($literal in $entry.literals) {
            Add-Occurrence $hardcodedAllowed "$($entry.file)|$literal"
        }
    }

    # STATIC-013 / 014
    Test-AllowlistRatchet "STATIC-013" "DontDestroyOnLoad allowlist" `
        "STATIC_EXACT" $true $ddolCurrent $ddolAllowed "ddolAllowlist" "FAIL"
    Test-AllowlistRatchet "STATIC-014" "Legacy Input allowlist" `
        "STATIC_EXACT" $true $inputCurrent $inputAllowed `
        "legacyInputAllowlist" "FAIL"

    # STATIC-015 / 016
    $sceneLocalDdol = @($ddolCurrent.Keys | Where-Object {
        $_ -match '^Assets/ModuleZ/(OpenWorld|Duel3D)/'
    })
    Add-BooleanCheck "STATIC-015" "Scene-local DDOL promotion" `
        "STATIC_EXACT" $true ($sceneLocalDdol.Count -eq 0) `
        "OpenWorld and Duel3D contain no DDOL call." `
        "Scene-local OpenWorld/Duel3D code contains DDOL." $sceneLocalDdol

    $productionAsmdefs = @(Get-ChildItem `
        (Join-Path $RepositoryRoot "Assets\ModuleZ") -Recurse `
        -Filter "*.asmdef" -File | Where-Object {
            $_.FullName -notmatch '[\\/]Tests[\\/]'
        } | ForEach-Object { Convert-ToRepositoryPath $_.FullName })
    Add-BooleanCheck "STATIC-016" "Production asmdef ratchet" `
        "STATIC_EXACT" $true ($productionAsmdefs.Count -eq 0) `
        "No non-test ModuleZ production asmdef exists." `
        "A non-test ModuleZ production asmdef exists." $productionAsmdefs

    # STATIC-017 / 018 / 019 / 020 / 021
    Test-AllowlistRatchet "STATIC-017" "Object lookup dependency heuristic" `
        "STATIC_HEURISTIC" $true $lookupCurrent $lookupAllowed `
        "objectLookupAllowlist" "FAIL"

    $openWorldCurrent = @{}
    $openWorldAllowed = @{}
    $gameCurrent = @{}
    $gameAllowed = @{}
    foreach ($key in $coreDependencyCurrent.Keys) {
        if ($key.EndsWith("|OpenWorld")) {
            $openWorldCurrent[$key] = $coreDependencyCurrent[$key]
        }
        elseif ($key.EndsWith("|Game")) {
            $gameCurrent[$key] = $coreDependencyCurrent[$key]
        }
    }
    foreach ($key in $coreAllowed.Keys) {
        if ($key.EndsWith("|OpenWorld")) {
            $openWorldAllowed[$key] = $coreAllowed[$key]
        }
        elseif ($key.EndsWith("|Game")) {
            $gameAllowed[$key] = $coreAllowed[$key]
        }
    }
    Test-AllowlistRatchet "STATIC-018" "Core to OpenWorld coupling heuristic" `
        "STATIC_HEURISTIC" $true $openWorldCurrent $openWorldAllowed `
        "coreDependencyAllowlist:OpenWorld" "FAIL"
    Test-AllowlistRatchet "STATIC-019" "Core to Game coupling heuristic" `
        "STATIC_HEURISTIC" $true $gameCurrent $gameAllowed `
        "coreDependencyAllowlist:Game" "FAIL"
    Test-AllowlistRatchet "STATIC-020" "Global mutable-state growth heuristic" `
        "STATIC_HEURISTIC" $false $globalCurrent $globalAllowed `
        "globalMutableAllowlist" "WARN"
    Test-AllowlistRatchet "STATIC-021" "Visible hardcoded UI heuristic" `
        "STATIC_HEURISTIC" $false $hardcodedCurrent $hardcodedAllowed `
        "hardcodedUiAllowlist" "WARN"

    # STATIC-022 / 023
    $duelStarterText = Read-RepositoryText `
        "Assets/ModuleZ/OpenWorld/Encounters/DuelStarterInteractable.cs"
    $duelTransitionBridgeText = Read-RepositoryText `
        "Assets/ModuleZ/Game/DuelTransition/DuelTransitionBridge.cs"
    $contextTopology =
        $duelStarterText -match 'new\s+DuelContext\s*\(' -and
        $duelStarterText -match 'DuelTransitionBridge\s*\.\s*Begin\s*\(' -and
        $duelStarterText -match 'DuelTransitionBridge\s*\.\s*CompleteAfterDelay\s*\(' -and
        $duelTransitionBridgeText -match 'ModuleZDuelSessionState\s*\.\s*StartDuel\s*\(' -and
        $duelTransitionBridgeText -match 'LoadDuel\s*\('
    Add-BooleanCheck "STATIC-022" "DuelContext normal producer path" `
        "STATIC_EXACT" $true $contextTopology `
        "DuelStarterInteractable constructs DuelContext and delegates transition to DuelTransitionBridge." `
        "Canonical DuelContext producer/bridge topology is incomplete." `
        @("Assets/ModuleZ/OpenWorld/Encounters/DuelStarterInteractable.cs", "Assets/ModuleZ/Game/DuelTransition/DuelTransitionBridge.cs")

    $noPostBootstrapOverride =
        $duelRuntimeBuilderText -notmatch 'PendingDuelRival' -and
        $duelRuntimeBuilderText -notmatch 'ModuleZDuelSessionState\s*\.' -and
        $duelRuntimeBuilderText -notmatch 'CurrentDuelRival\s*=\s*ModuleZGameState\s*\.\s*PendingDuelRival' -and
        $duelRuntimeBuilderText -match 'Initialize\s*\(\s*DuelContext\s+context'
    Add-BooleanCheck "STATIC-023" "No competing post-bootstrap DuelContext override" `
        "STATIC_EXACT" $true $noPostBootstrapOverride `
        "Duel runtime accepts context and contains no Pending/session rival override." `
        "Duel runtime contains a competing Legacy context source or lacks explicit context initialization." `
        @($duelRuntimeBuilderPath)

    # STATIC-024 / 025 / 026
    $resultManagerPath =
        "Assets/ModuleZ/Duel3D/Rules/Duel3DResultManager.cs"
    $resultManagerText = Read-RepositoryText $resultManagerPath
    $duelResultBridgeText = Read-RepositoryText `
        "Assets/ModuleZ/Game/DuelTransition/DuelResultBridge.cs"
    $resultTopology =
        $duelRuntimeBuilderText -match 'new\s+DuelResult\s*\(' -and
        $resultManagerText -match 'Complete\s*\(\s*DuelResult\s+result' -and
        $resultManagerText -match 'DuelResultBridge\s*\.\s*Complete\s*\(' -and
        $duelResultBridgeText -match 'result\.RivalId\s*!=\s*acceptedContext\.RivalId'
    Add-BooleanCheck "STATIC-024" "DuelResult canonical path" `
        "STATIC_EXACT" $true $resultTopology `
        "Runtime result construction reaches ResultManager and identity-validating DuelResultBridge." `
        "Canonical DuelResult topology is incomplete." `
        @($duelRuntimeBuilderPath, $resultManagerPath, "Assets/ModuleZ/Game/DuelTransition/DuelResultBridge.cs")

    $legacyResultApiEvidence = [System.Collections.Generic.List[string]]::new()
    foreach ($file in $productionFiles) {
        $text = [System.IO.File]::ReadAllText($file.FullName)
        if ($text -match '\b(?:WinDuel|LoseDuel)\b') {
            $legacyResultApiEvidence.Add(
                (Convert-ToRepositoryPath $file.FullName)
            )
        }
    }
    Add-BooleanCheck "STATIC-025" "No WinDuel or LoseDuel legacy result API" `
        "STATIC_EXACT" $true ($legacyResultApiEvidence.Count -eq 0) `
        "Production source contains no WinDuel/LoseDuel operation." `
        "Legacy WinDuel/LoseDuel production operation exists." `
        @($legacyResultApiEvidence)

    $drawCases = Get-MatchCount $duelRuntimeBuilderText `
        'case\s+Duel3DMatchResult\.Draw\s*:'
    $drawToDefeat = Get-MatchCount $duelRuntimeBuilderText `
        'case\s+Duel3DMatchResult\.Draw\s*:\s*outcome\s*=\s*DuelOutcome\.Defeat\s*;' `
        ([Text.RegularExpressions.RegexOptions]::Singleline)
    Add-BooleanCheck "STATIC-026" "Draw maps to Defeat" `
        "STATIC_EXACT" $true ($drawCases -eq 1 -and $drawToDefeat -eq 1) `
        "Exactly one Draw case maps explicitly to DuelOutcome.Defeat." `
        "Draw-to-Defeat mapping is absent or ambiguous." `
        @("Draw cases=$drawCases", "Draw-to-Defeat mappings=$drawToDefeat", $duelRuntimeBuilderPath)

    # STATIC-027 / 028
    $sessionPath =
        "Assets/ModuleZ/Core/Managers/ModuleZDuelSessionState.cs"
    $sessionText = Read-RepositoryText $sessionPath
    $startDuelBody = Get-MethodBody $sessionText `
        'public\s+static\s+void\s+StartDuel\s*\('
    $payloadIndexes = @(
        $startDuelBody.IndexOf('RivalId = rivalId;'),
        $startDuelBody.IndexOf('IsRematch = isRematch;'),
        $startDuelBody.IndexOf('ReturnPosition = returnPosition;')
    )
    $activeIndex = $startDuelBody.IndexOf('HasActiveDuel = true;')
    $publicationOrder =
        $null -ne $startDuelBody -and
        @($payloadIndexes | Where-Object { $_ -lt 0 }).Count -eq 0 -and
        $activeIndex -gt ($payloadIndexes | Measure-Object -Maximum).Maximum
    Add-BooleanCheck "STATIC-027" "StartDuel publication order" `
        "STATIC_EXACT" $true $publicationOrder `
        "StartDuel writes RivalId, IsRematch, ReturnPosition before publishing active state." `
        "StartDuel payload-before-active order is not preserved." @($sessionPath)

    $clearBody = Get-MethodBody $sessionText `
        'public\s+static\s+void\s+Clear\s*\('
    $requiredResets = @(
        'HasActiveDuel\s*=\s*false\s*;',
        'RivalId\s*=\s*default\s*;',
        'IsRematch\s*=\s*false\s*;',
        'ReturnPosition\s*=\s*Vector3\.zero\s*;'
    )
    $missingResets = @($requiredResets | Where-Object {
        $null -eq $clearBody -or $clearBody -notmatch $_
    })
    Add-BooleanCheck "STATIC-028" "Clear full reset surface" `
        "STATIC_EXACT" $true ($missingResets.Count -eq 0) `
        "Clear resets active state and the complete session payload." `
        "Clear is missing one or more required session resets." `
        @($missingResets + $sessionPath)

    # STATIC-029
    $duelBootstrapPath = "Assets/ModuleZ/Duel3D/Runtime/DuelBootstrap.cs"
    $duelBootstrapText = Read-RepositoryText $duelBootstrapPath
    $fallbackBody = Get-MethodBody $duelBootstrapText `
        'private\s+static\s+bool\s+TryResolveDirectSceneFallback\s*\('
    $fallbackIsolated =
        $null -ne $fallbackBody -and
        $fallbackBody -match 'new\s+DuelContext\s*\(' -and
        $fallbackBody -notmatch 'ModuleZDuelSessionState\s*\.\s*StartDuel\s*\('
    Add-BooleanCheck "STATIC-029" "Direct-scene fallback isolation" `
        "STATIC_EXACT" $true $fallbackIsolated `
        "DuelBootstrap fallback creates context without fabricating an active session." `
        "Direct-scene fallback is absent or activates production session state." `
        @($duelBootstrapPath)

    # STATIC-030 / 031 / 032
    $requiredTestAsmdefs = @(
        "Assets/ModuleZ/Tests/EditMode/ModuleZ.EditModeTests.asmdef",
        "Assets/ModuleZ/Tests/PlayMode/ModuleZ.PlayModeTests.asmdef"
    )
    $missingTestAsmdefs = @($requiredTestAsmdefs | Where-Object {
        -not (Test-Path -LiteralPath (Join-Path $RepositoryRoot $_) -PathType Leaf)
    })
    Add-BooleanCheck "STATIC-030" "Required test assemblies present" `
        "STATIC_EXACT" $true ($missingTestAsmdefs.Count -eq 0) `
        "Required EditMode and PlayMode test asmdefs exist." `
        "A required test asmdef is missing." $missingTestAsmdefs

    $allTestAsmdefs = @(Get-ChildItem `
        (Join-Path $RepositoryRoot "Assets\ModuleZ\Tests") -Recurse `
        -Filter "*.asmdef" -File)
    $invalidTestAsmdefs = [System.Collections.Generic.List[string]]::new()
    foreach ($asmdef in $allTestAsmdefs) {
        $relative = Convert-ToRepositoryPath $asmdef.FullName
        $definition = Get-Content -LiteralPath $asmdef.FullName -Raw |
            ConvertFrom-Json
        if (@($definition.optionalUnityReferences) -notcontains "TestAssemblies" -or
            -not $relative.StartsWith("Assets/ModuleZ/Tests/")) {
            $invalidTestAsmdefs.Add($relative)
        }
    }
    Add-BooleanCheck "STATIC-031" "Test assemblies only" `
        "STATIC_EXACT" $true `
        ($allTestAsmdefs.Count -ge 2 -and $invalidTestAsmdefs.Count -eq 0) `
        "All ModuleZ test asmdefs remain under Tests and use TestAssemblies support." `
        "A test asmdef is misplaced or lacks TestAssemblies support." `
        @($invalidTestAsmdefs)

    $wrapperPath = "Tools/Validation/Invoke-ModuleZValidation.ps1"
    $wrapperText = Read-RepositoryText $wrapperPath
    $wrapperPresent =
        $wrapperText -match '"static"' -and
        $wrapperText -match 'Invoke-ModuleZStaticValidation\.ps1'
    Add-BooleanCheck "STATIC-032" "Validation wrapper present" `
        "STATIC_EXACT" $true $wrapperPresent `
        "Validation wrapper exposes and invokes the static stage." `
        "Validation wrapper is missing static-stage integration." `
        @($wrapperPath)

    $gatingFailures = @($Results | Where-Object {
        $_.gating -and $_.status -eq "FAIL"
    })
    $overall = if ($gatingFailures.Count -eq 0) { "PASS" } else { "FAIL" }
    $exactResults = @($Results | Where-Object {
        $_.classification -eq "STATIC_EXACT"
    })
    $heuristicResults = @($Results | Where-Object {
        $_.classification -eq "STATIC_HEURISTIC"
    })
    $summary = [ordered]@{
        total = $Results.Count
        pass = @($Results | Where-Object status -eq "PASS").Count
        warn = @($Results | Where-Object status -eq "WARN").Count
        fail = @($Results | Where-Object status -eq "FAIL").Count
        skip = @($Results | Where-Object status -eq "SKIP").Count
        gating = @($Results | Where-Object gating).Count
        exact = [ordered]@{
            total = $exactResults.Count
            pass = @($exactResults | Where-Object status -eq "PASS").Count
            fail = @($exactResults | Where-Object status -eq "FAIL").Count
        }
        heuristic = [ordered]@{
            total = $heuristicResults.Count
            pass = @($heuristicResults | Where-Object status -eq "PASS").Count
            warn = @($heuristicResults | Where-Object status -eq "WARN").Count
            fail = @($heuristicResults | Where-Object status -eq "FAIL").Count
        }
    }
    $document = [ordered]@{
        schemaVersion = $ResultSchemaVersion
        validatorVersion = $ValidatorVersion
        timestampUtc = [DateTime]::UtcNow.ToString("o")
        repositoryRoot = $RepositoryRoot
        baseline = [ordered]@{
            path = Convert-ToRepositoryPath $BaselinePath
            schemaVersion = [int]$Baseline.schemaVersion
            commit = [string]$Baseline.baselineCommit
        }
        overall = $overall
        summary = $summary
        checks = @($Results)
    }

    $document | ConvertTo-Json -Depth 10 |
        Set-Content -LiteralPath $ResultPath -Encoding UTF8
    $LogLines.Add(
        "Static validation $overall"
    )
    $LogLines.Add(
        "Exact: $($summary.exact.pass) pass / $($summary.exact.fail) fail"
    )
    $LogLines.Add(
        "Heuristic: $($summary.heuristic.pass) pass / $($summary.heuristic.warn) warn / $($summary.heuristic.fail) fail"
    )
    $LogLines | Set-Content -LiteralPath $LogPath -Encoding UTF8

    Write-Host "Static validation $overall"
    Write-Host "Exact: $($summary.exact.pass) pass / $($summary.exact.fail) fail"
    Write-Host "Heuristic: $($summary.heuristic.pass) pass / $($summary.heuristic.warn) warn / $($summary.heuristic.fail) fail"
    Write-Host "Artifacts: $ResultPath; $LogPath"

    if ($overall -eq "FAIL") { exit 1 }
    exit 0
}
catch {
    try {
        New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
        $fatal = [ordered]@{
            schemaVersion = $ResultSchemaVersion
            validatorVersion = $ValidatorVersion
            timestampUtc = [DateTime]::UtcNow.ToString("o")
            repositoryRoot = $RepositoryRoot
            overall = "FAIL"
            fatalError = $_.Exception.Message
            checks = @($Results)
        }
        $fatal | ConvertTo-Json -Depth 10 | Set-Content `
            -LiteralPath (Join-Path $OutputPath "static-results.json") `
            -Encoding UTF8
        "Static validation FAIL: $($_.Exception.Message)" | Set-Content `
            -LiteralPath (Join-Path $OutputPath "static.log") `
            -Encoding UTF8
    }
    catch {
        # Preserve the original validation failure even if artifacts cannot be written.
    }
    Write-Error $_ -ErrorAction Continue
    exit 1
}
