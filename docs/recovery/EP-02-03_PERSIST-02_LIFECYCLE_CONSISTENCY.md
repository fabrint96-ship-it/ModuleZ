# EP-02-03 / PERSIST-02 — Lifecycle & Consistency Guards

Status: implementation proposed for review. Baseline: `develop` at
`300d8674086337ff774a9a2dbb5b697161c70236`. This does not close REC-008,
REC-007, F1.6 or EP-01. Unity validation is **NOT RUN** pending separate Owner
authorization.

## Scope and authority

PERSIST-01's save filename, JSON schema, `SaveGame()` compatibility entry,
isolated root, verified pending write, first `File.Move`, existing `File.Replace`,
backup preservation and read-only backup fallback remain unchanged. This slice
adds a save-publication gate, a bridge-owned provisional-transition token, a
result-only save retry in Duel, and portal save-before-navigation behavior. No
transition marker is serialized. `DuelContext` and `DuelResult` remain the typed
authorities; `ModuleZGameState` remains a Legacy projection.

## Save-site inventory

| Caller | Previously | This slice |
| --- | --- | --- |
| `ModuleZAutoSaveController.OnApplicationPause/OnApplicationQuit` | `SaveGame()` | Same compatible call, now gated before capture; rejected saves log a warning. An accepted, applied Duel result is allowed. |
| `OpenWorldPauseMenuController` Save/Save-and-exit | Ignored `SaveGame()` outcome | Uses `TrySaveGame()`; reports failure, and Save-and-exit remains in scene on failure. |
| `OpenWorldZonePortal.OnTriggerEnter` | Saved theme before updating return position | Validates destination, publishes both, saves, and only then loads scene. |
| `Duel3DResultManager` Victory/Defeat/Abandon | Applied effects, called void `SaveGame()`, cleared session and returned regardless | Applies once, examines `TrySaveGame()`, clears/returns only on Success; failure stays in Duel with retry button. |
| `ModuleZSaveBootstrap.Awake` | Compatibility `LoadGame()` | Same entry; `TryLoadGame()` now records an uncertain load after read/apply failure so later autosave cannot publish provisional state. |

## Concrete contracts

| ID | Invariant |
| --- | --- |
| LIFE-01 | Stable exploration with no unresolved session/transition/load may save. |
| LIFE-02 | A load in progress rejects saves before state capture or achievement evaluation. |
| LIFE-03 | A bridge-owned provisional transition rejects saves until its exact token is released. |
| LIFE-04 | A failed read/application blocks publication until a successful load or a complete New Game sequence (delete, reset, session clear, confirmation); `NotFound` alone does not poison a fresh game. |
| LIFE-05 | An active Duel session is not the sole gate: only a result applied through the validated bridge records an authorization bound to the current session revision, allowing save/retry and pause/quit autosave. A Legacy `DuelCompleted` flag alone cannot authorize save. |
| LIFE-06 | `Duel3DResultManager` validates through `DuelResultBridge` and applies one result once. |
| LIFE-07 | Failed result publication keeps the applied result in memory, session active and Duel scene present; retry calls only `TrySaveGame()`. Return and session clear require Success. |
| LIFE-08 | Portal theme and return position are prepared together, persisted before navigation, and rolled back on failure only while their published values remain present. Duplicate trigger activation is rejected while operating. |
| AUTH-01 | `ModuleZSaveManager` alone decides save status and checks the gate before `CaptureSaveData()`/`EvaluateAll()`. |
| AUTH-02 | `TryLoadGame()` brackets its synchronous application and exposes explicit load status; successful load clears load uncertainty. |
| AUTH-03 | `DuelTransitionBridge.Attempt` retains transition authority; only its exact token may release its save gate. Stale attempts cannot release another token. |
| AUTH-04 | Session `Revision` and owned Legacy projections remain bridge rollback evidence. Ownership loss enters conservative uncertainty instead of publishing a potentially mixed snapshot. |
| AUTH-05 | An accepted `DuelResult` controls result effects and registers the applied-result save authorization; storage retry has no path back to `ApplyVictory`, `ApplyDefeat` or `ApplyAbandon`. |
| AUTH-06 | The gate is transient publication coordination, not a new persisted Application State, DDOL owner, or replacement for `ModuleZDuelSessionState`. |
| SG-01 | Lifecycle rejection has a distinct `LifecycleRejected` save status; it is not confused with storage failure or Success. |
| SG-02 | PERSIST-01's pending-file verification, no orphan promotion, and explicit error statuses remain. |
| SG-03 | PERSIST-01's existing-save replacement/backup behavior remains; an old valid save is not treated as a newly saved result. |
| SG-04 | UI/navigation never claims new progress was saved without `Success`. |

## Authorization matrix

| State | Save | Recovery |
| --- | --- | --- |
| Stable OpenWorld | Allowed | Normal PERSIST-01 result status. |
| Synchronous load in progress | `LifecycleRejected` | Load finishes; success clears uncertainty. |
| Invalid/read/apply failed load | `LifecycleRejected` | Successful load or complete New Game reset. |
| Bridge attempt pending | `LifecycleRejected` | Exact owner commits or cancels. |
| Bridge ownership lost / partial Legacy rollback | `LifecycleRejected` | Successful load or complete New Game reset; do not assume foreign fields are owned. |
| Active unresolved Duel | `LifecycleRejected` | Only a validated result from the same accepted session may begin result application. |
| Direct Duel scene fallback | `LifecycleRejected` | Trial gameplay is allowed; results are rejected before effects, with an explicit unsaved exit to MainMenu. |
| Accepted/applied Duel result | Allowed when its transient authorization matches session `Revision` | On failure, explicit retry in Duel; pause/quit may also attempt save. |

`HasActiveDuel` is never the sole test for authorization. The current Duel
scene is also guarded because the transitional direct-scene fallback has no
active session but can mutate personality progress before a result. The bridge's attempt
identity, session revision, load bracket and applied-result authorization all have distinct
roles. `LoadDuel()` only requests the scene: the attempt gate is released only
after the matching session-backed `DuelSceneRoot` reaches `Ready`. Cancel releases only
its own token; cancellation with lost ownership marks state uncertain. The
compatibility `Begin(context, out string)` now uses a tracked attempt rather
than publishing an unguarded projection. Its caller does not receive the token,
so production must continue to use the token-returning overload. Deleting a
save alone does not clear an uncertainty: the menu must reset GameState, clear
the session, then call its internal fresh-start confirmation. A deletion during
an owned transition is rejected before touching disk.

## Failure matrix and sequences

| Operation | Failure | Effect |
| --- | --- | --- |
| Load | Missing save | `NotFound`; no state applied; fresh game is not blocked. |
| Load | Invalid/read/apply error | Existing rollback behavior remains; save publication is blocked until explicit recovery. |
| Duel transition | Rejection/cancel | Owned projections/session restored; gate released only for exact attempt. |
| Duel transition | Revision/Legacy ownership lost | Foreign values not overwritten; unresolved state conservatively blocks save. |
| Duel result | Save error | Result remains applied in memory, no session clear or return, visible retry button. |
| Duel result | Retry succeeds | No outcome effects repeated; session clears and return is scheduled. |
| Portal | Bad destination/controller | No state update or scene load. |
| Portal | Save error | Stay in scene, restore only still-owned theme/position/return flag. |
| Portal | Scene-load exception after successful save | Error logged; saved destination remains committed; no false claim of scene arrival. |

Result sequence: `DuelResultBridge.Complete` → one compatibility application →
`TrySaveGame` → on Success clear session and return; otherwise show retry and
stay. Retry begins at `TrySaveGame`, not at result application. This is an
in-memory retry only: forced process termination or power loss is not durably
recoverable. A PERSIST-01 diagnostic `.pending` retained after publication
failure may require external recovery before any retry can succeed; it is not
silently overwritten or promoted.

Portal sequence: validate theme, finite spawn and SceneController → remember
three prior fields → publish target theme, return position and return flag →
`TrySaveGame` → only on Success call `LoadOpenWorld`. Rollback compares values
before restoring to avoid overwriting a differing producer. Value equality is
not a full ownership proof if another producer writes the same value; this is
a documented transitional limitation. A synchronous scene-load exception after
successful publication cannot be rolled back as an atomic disk transaction.
The saved theme, return position and return flag remain committed after a
synchronous scene-load exception. The player remains in the current scene and
may reactivate the portal to retry the load; this is not a second save
transaction or a guarantee that later scene initialization succeeds.

## REVIEW-03 readiness and result-identity corrections

The bridge retains the exact attempt while scene loading is requested. Its
`sceneLoaded` observation binds the requested Duel scene handle; the
`DuelSceneRoot` reports `Ready` only after successful runtime composition.
Confirmation requires that handle, the same accepted `DuelContext`, an active
session with the original revision and matching payload, and a session-backed
bootstrap origin. `LoadDuel()` returning is not confirmation. The departing
OpenWorld interactable cannot cancel an already requested load. A failed or
destroyed root, another loaded scene, or 20 seconds without `Ready` retires
the attempt, retains the conservative save block and clears only the session
still owned by that attempt. Late callbacks cannot release another token.
If the origin OpenWorld remains active, its interactable drops its local
pending state and explains that a menu recovery is required; it does not
pretend a second attempt can bypass the conservative gate. The existing
SceneController clears its static `Instance` on destruction, including after
test-owned controller teardown.
If the persistent SceneController itself disappears before its readiness
watcher runs, the unreleased gate remains fail-closed; explicit fresh-start or
successful-load recovery is then required.

`DuelBootstrap` captures whether its accepted context came from an active
session and the revision at acceptance. The runtime builder passes this
provenance to the result manager without reading a competing Legacy context.
Before applying a result, the result manager checks that the captured session
still owns the accepted payload. The save gate brackets result application,
confirms only the matching result rival/context/revision after effects have
been applied, and checks the same identity again on save or retry. A changed
session leaves the provisional result unpublished even after leaving Duel;
successful publication consumes the authorization. The direct-scene fallback
can still build and play, but cannot apply Victory/Defeat/Abandon progression:
it shows trial-mode notice and an exit to MainMenu without saving. Its
non-persistable Legacy projection stays blocked until a valid load or complete
New Game reset. This is not durable recovery of an unsaved result.

## Tests and validation

`ApplicationStateTransitionEditModeTests` adds isolated cases for stable
publication, rejection before capture while a transition is pending, stale
ownership uncertainty, failed-load recovery and rejection of a Legacy-only
completed flag. The PlayMode result test exercises the actual accepted-result
authorization.
Its setup/teardown uses isolated deletion, reset and internal fresh-start
confirmation, then restores prior GameState, session and achievements. The
internal call is exercised through reflection because the test assembly cannot
reference predefined Assembly-CSharp directly; it is not a public test seam.
`DuelResultPersistenceTests` is a PlayMode Persistence-category fixture with a
per-test child root under the wrapper's process isolation root. It forces a
temporary-write failure, verifies the live retry UI, retries only the save,
checks the result counter remains singular, and adds direct-fallback and
production-scene readiness/failure/destruction/timeout/late-callback checks.
The new EditMode cases check result identity, unconfirmed publication,
replacement with the same payload, and absence of a session. Every fixture
clears its isolated gate before restoring prior GameState, session and
achievement snapshots. No test accesses the real profile directly. The new
PlayMode scene cases are **NOT RUN** until separate Unity authorization;
their injected failures are test-local and do not change production scenes.

Static validation on the implementation branch: **PASS**, 30 PASS / 2 WARN /
0 FAIL. `STATIC-020` flags the explicit private transient gate fields (and
pre-existing baseline drift); `STATIC-021` flags the required visible retry
and failure messages (and pre-existing baseline drift). Neither baseline was
silently expanded. `git diff --check`: PASS. Unity compile, EditMode, PlayMode,
Persistence, Shutdown and Smoke remain **NOT RUN** in this authorization.
Independent Unity CI remains blocked. Runtime UI, portal collisions and
post-quit behavior require subsequent authorized Unity/Owner verification.
No Unity process, gameplay test, real save/settings access, worktree cleanup,
backup mutation or `.vscode/` operation was performed for this implementation.

## Limitations and non-guarantees

This is not a durable transaction manager, write queue, SaveSystem redesign or
new application owner. It does not promise recovery after a crash/power loss,
repair an orphan `.pending`, or prove cross-producer ownership when the same
Legacy value is rewritten. `DuelResultManager` can retry storage failures but
cannot solve unavailable storage without external intervention. Validating
compile/runtime behavior requires a separate Unity authorization. REC-008 is
still partially migrated/stabilized; REC-007 and F1.6 remain open.
