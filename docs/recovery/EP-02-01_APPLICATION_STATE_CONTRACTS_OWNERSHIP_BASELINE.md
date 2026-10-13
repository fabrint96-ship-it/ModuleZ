# EP-02-01 — Application State contracts and ownership baseline

## Scope and status

This is a characterization of `develop` at `6f06df89beb349d604f239463de2f0f47d0c08bd`, not a new state architecture. Production remains in `Assembly-CSharp`; this slice changes only EditMode tests and this document. REC-008 remains **PARTIALLY MIGRATED / STABILIZED**. REC-007, F1.6 and EP-01 remain open. The [transitional integration protocol](../workflows/TRANSITIONAL_INTEGRATION_VALIDATION_PROTOCOL.md) classifies this PR as D1 (tests/validators); Unity execution needs separate Project Owner authorization.

## Vocabulary and authority

P = serialized by `ModuleZSaveManager`; D = derived from gameplay/progression facts (some derived values are also serialized); T = transient runtime/return state; C = configuration persisted by `ModuleZSettingsManager`; L = Legacy compatibility projection. Combined labels are intentional. “Functional owner” names the present writer or consumer responsible for an operation, **not** exclusive ownership of a public mutable field. For example, save loading and `ResetGameState()` write many fields. `DuelContext` is the accepted transition/runtime context; `DuelResult` is the accepted result. Legacy fields do not supersede those contracts after acceptance. `ModuleZDuelSessionState` is transitional and separate from `ModuleZGameState`.

Evidence key: GS = [`ModuleZGameState.cs`](../../Assets/ModuleZ/Core/Managers/ModuleZGameState.cs); SM = [`ModuleZSaveManager.cs`](../../Assets/ModuleZ/Core/SaveSystem/ModuleZSaveManager.cs); ST = [`ModuleZSettingsManager.cs`](../../Assets/ModuleZ/Core/Settings/ModuleZSettingsManager.cs); TB = [`DuelTransitionBridge.cs`](../../Assets/ModuleZ/Game/DuelTransition/DuelTransitionBridge.cs); DB = [`DuelBootstrap.cs`](../../Assets/ModuleZ/Duel3D/Runtime/DuelBootstrap.cs); RB = [`Duel3DRuntimeBuilder.cs`](../../Assets/ModuleZ/Duel3D/Runtime/Duel3DRuntimeBuilder.cs); RM = [`Duel3DResultManager.cs`](../../Assets/ModuleZ/Duel3D/Rules/Duel3DResultManager.cs); RP = [`ModuleZRivalProgression.cs`](../../Assets/ModuleZ/OpenWorld/Encounters/ModuleZRivalProgression.cs); OW = [`OpenWorldRuntimeBuilder.cs`](../../Assets/ModuleZ/OpenWorld/Runtime/OpenWorldRuntimeBuilder.cs); ZP = [`OpenWorldZonePortal.cs`](../../Assets/ModuleZ/OpenWorld/Runtime/OpenWorldZonePortal.cs). Every row is declared and reset in GS except the two C fields, which `ResetGameState()` preserves.

## Public `ModuleZGameState` inventory (32/32)

| # | Field | Class | Current functional owner / principal writers | Principal readers / use |
| ---: | --- | --- | --- | --- |
| 1 | `OpenWorldReturnPosition` | P/L | TB, RB, ZP; SM load | OW spawn, DB direct-scene fallback, SM save |
| 2 | `ReturningFromDuel` | T/L | TB, RM, OW, ZP | OW return spawn |
| 3 | `CurrentDuelRival` | T/L | TB, RB | Duel match config, arena, music, HUD, RB |
| 4 | `RivalMadridDefeated` | P | RP; SM load | RP, OW content, progress UI, achievements, SM save |
| 5 | `RivalBarcelonaDefeated` | P | RP; SM load | RP, progress UI, achievements, SM save |
| 6 | `RivalValenciaDefeated` | P | RP; SM load | RP, progress UI, achievements, SM save |
| 7 | `RivalAndaluciaDefeated` | P | RP; SM load | RP, progress UI, achievements, SM save |
| 8 | `AndaluciaUnlocked` | P/D | RP `UpdateUnlocks`; SM load | OW theme content, HUD/guide, SM save |
| 9 | `DuelsWon` | P | RM; SM load | achievements, progress UI, RB AI settings, SM save |
| 10 | `DuelsLost` | P | RM; SM load | achievements, progress UI, RB AI settings, SM save |
| 11 | `DuelsAbandoned` | P | RM; SM load | achievements, progress UI, RB AI settings, SM save |
| 12 | `DuelWasCancelled` | T/L | TB, RM; SM load reset | No current production read found; compatibility debt |
| 13 | `DuelCompleted` | T/L | TB, RM; SM load reset | Duel pause menu |
| 14 | `DuelWasLost` | T/L | TB, RM; SM load reset | No current production read found; compatibility debt |
| 15 | `DuelWasAbandoned` | T/L | TB, RM; SM load reset | No current production read found; compatibility debt |
| 16 | `LastDuelResultMessage` | T/L | RM, OW clear | No current production display read found; compatibility debt |
| 17 | `PendingOpenWorldMessage` | T | RM, OW clear | OW return-message presentation |
| 18 | `IsPaused` | T | Pause controllers, TB, RM; SM load reset | Player controller, RB |
| 19 | `AudioEnabled` | C | ST, Main Menu toggle | ST applies audio and persists settings; Main Menu |
| 20 | `FullscreenEnabled` | C | ST, Main Menu toggle | ST applies display and persists settings; Main Menu |
| 21 | `CurrentOpenWorldTheme` | P | ZP; SM load | OW theme, UI/guide, achievements, SM save |
| 22 | `MainProgressionCompleted` | P/D | RP; SM load | Duel starter, achievements, progress UI, SM save |
| 23 | `CurrentDuelIsRematch` | T/L | TB, RB, RM clear; SM load reset | DB direct-scene fallback |
| 24 | `RematchesWon` | P | RM; SM load | achievements, progress UI, RB AI settings, SM save |
| 25 | `RematchesLost` | P | RM; SM load | achievements, progress UI, RB AI settings, SM save |
| 26 | `RematchesAbandoned` | P | RM; SM load | achievements, progress UI, RB AI settings, SM save |
| 27 | `IsOverlayOpen` | T | HUD overlay coordinator | Player controller |
| 28 | `RivalMadridPersonalityCompleted` | P | RB; SM load | achievements, SM save |
| 29 | `RivalBarcelonaPersonalityCompleted` | P | RB; SM load | achievements, SM save |
| 30 | `RivalValenciaPersonalityCompleted` | P | RB; SM load | achievements, SM save |
| 31 | `RivalAndaluciaPersonalityCompleted` | P | RB; SM load | achievements, SM save |
| 32 | `PendingDuelRival` | T/L | TB | DB direct-scene fallback |

The matrix records principal usage, not an exhaustive call graph. GS uses public static mutable fields; other systems can technically write them without a guard. `ResetGameState()` is a broad reset writer for 30 entries. SM rehydrates only its persisted subset and resets selected transient flags. ST uses a separate settings file, not the save schema. `ModuleZDuelSessionState.StartDuel/Clear` owns the *session payload*, not these 32 fields. `DuelResultBridge` validates the accepted result before RM projects compatibility effects and saves.

## Existing invariants and new EditMode contracts

- **STATE-RESET-001:** capture all 32 public values, assign distinguishable sentinels, invoke public `ResetGameState()`, assert 30 explicit defaults and preservation of `AudioEnabled`/`FullscreenEnabled`, then restore all 32 in `finally`. This ratchets the observed reset contract, not a claim that New Game fully resets all application state.
- **STATE-ENTRY-001:** the existing `L1_CONTEXT_002` now asserts all nine Legacy projections of a valid `DuelTransitionBridge.Begin`, session payload, and nonmutation of both session and projections after a rejected invalid rival. The fixture snapshots/restores the pre-test session, and the test restores prior Legacy values. `L1_CONTEXT_001` still covers invalid context from an inactive session. The production path remains `DuelContext` → TB → session → DB accepted context.
- **STATE-LOAD-ABSENT-001:** select a unique, nonexistent temporary root through public `ModuleZPersistencePaths.SetRootOverride` *before* calling `ModuleZSaveManager.LoadGame()`. Assert no save is present and no GS/session value changes, then clear the override and restore snapshots in `finally`. No save is written, deleted, backed up, or read from the real profile.

These tests probe public production members because EditMode test asmdefs cannot reference predefined `Assembly-CSharp` directly. No private production reflection or production API change is introduced. Existing PlayMode persistence tests cover isolated save/settings routing, and static ratchets cover transition topology; neither replaces the three new value-level checks. Unity execution is **NOT RUN** pending explicit authorization. No runtime PASS is inferred from source inspection.

## Lifecycle and deferred decisions

Current session path: inactive → `StartDuel` publishes payload before active bit → DB copies one accepted `DuelContext` → RM resolves accepted `DuelResult`, saves and clears session → inactive. `Clear` resets the four session fields. Direct-scene fallback does not activate a production session. Reset and missing-save load are distinct operations: `ResetGameState()` does not clear session; `LoadGame()` with no file returns before changing either state. This is a factual boundary, **not** endorsement of an active session surviving New Game or Continue.

Current Main Menu New Game deletes save, resets GS and loads OpenWorld; Continue loads save and OpenWorld. Neither path explicitly clears an already active session. EP-02-02 should decide and test the desired New Game/Continue session policy, failed transition rollback and broader lifecycle ownership before production changes. EP-02-03 should assess Legacy result/return-message flags, positive LoadGame rehydration, redundant readers/writers and eventual REC-008 scope. Do not freeze anomalous stale-session behavior as a permanent contract. The existing `OpenWorldZonePortal` save ordering (save before return-position update) is also debt to review separately, not changed here.

## Validation and integration evidence

Allowed now: static validator, source/diff inspection and `git diff --check`. Forbidden now: Unity `compile`, `editmode`, PlayMode, smoke or any gameplay execution. A later authorized D1 verification must pin the PR head SHA and checkout, Unity `6000.3.13f1`, and run `compile` then `editmode` through `Tools/Validation/Invoke-ModuleZValidation.ps1`, preserving exit codes, logs, XML, discovered/passed/failed/skipped counts (expected EditMode total: **8**, including the infrastructure case), and external post-exit save/settings fingerprints. Stop at the first failure or real-profile change. GitHub's required static check is separate and does not constitute Unity CI. Owner review and merge authorization remain outstanding.
