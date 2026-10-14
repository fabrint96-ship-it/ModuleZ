# EP-02-02 — Integration and validation record

## Identity and disposition

EP-02-02 was implemented in [PR #35](https://github.com/fabrint96-ship-it/ModuleZ/pull/35), validated at `f459de5993dc7fa9b804a3c1add5c08a2781362d`, and merged into `develop` as `58690f483db2ae643b738859e3297ec68cb50182`. GitHub reports PR #35 as **MERGED**; local `develop` and `origin/develop` were verified at the merge commit with no tracked changes. Integration verification: **PASS**.

Current status: **IMPLEMENTED / VERIFIED / REVIEWED / MERGED / SYNCHRONIZED; RECORD IN PROGRESS; CLOSE NOT AUTHORIZED**. This record does not close EP-02-02. EP-01, F1.6, and REC-007 remain open. REC-008 remains **PARTIALLY MIGRATED / STABILIZED**, not a completed application-state migration.

## Delivered behavior and compatibility

The merged [implementation characterization](EP-02-02_DUEL_TRANSITION_SESSION_LIFECYCLE.md) describes the code-level contracts. `DuelTransitionBridge` now gives the production caller an `Attempt` that owns a delayed transition. Cancellation checks attempt identity and the `ModuleZDuelSessionState` revision, restores only Legacy projections that still have the attempt's published value, and cannot let an obsolete attempt undo its successor. If a foreign Legacy value prevents full rollback, it preserves that foreign value, releases the other attempt-owned projections, and clears the owned session rather than restoring an ambiguous previous active session. A fully owned cancellation can restore the previous session.

The session revision changes when `StartDuel` or `Clear` runs. The production interactable retains and cancels its attempt on disable/destroy or rejected completion. A missing scene controller and a synchronous load exception in the origin scene can trigger bounded rollback. The existing two-argument `Begin` remains a compatibility entry point; production uses the attempt-returning overload. `DuelContext`, `DuelResult`, scene assets, gameplay rules, and the nine Legacy projection fields were not redefined by this slice.

`ModuleZSaveManager.TryLoadGame()` returns `Success`, `NotFound`, `InvalidData`, `ReadError`, or `ApplyError`, while `LoadGame()` remains a void compatibility wrapper. The reader checks complete JSON grammar, required root counters, known root field types, and deserialized semantic values before applying a candidate save. Unknown fields and absent optional fields remain compatible with the current `modulez_save.json` format. A snapshot restores affected in-memory GameState and achievement values if application fails. The save filename, serialization format, and schema are unchanged; this is not migration support for the historical `module_z_save.json` filename.

New Game retains its two-click confirmation: successful deletion precedes GameState reset, duel-session clear, and OpenWorld navigation; deletion failure leaves the menu state in place. Continue clears a stale duel session and navigates only after `TryLoadGame()` succeeds. Missing, invalid, unreadable, or unappliable saves leave the session and menu flow unchanged. These are current functional ownership policies, not a new global application-state owner.

## Validation evidence

Validation used Unity **6000.3.13f1**, the official isolated wrapper, and a detached worktree at the exact implementation SHA:

```text
C:\Users\fabri\Documents\Proyectos\Unity\ModuleZ-EP02-02-PR35-f459de5-VERIFY01-20261014
```

Local logs and XML were retained outside that worktree under:

```text
C:\Users\fabri\Documents\Proyectos\Unity\ModuleZ\artifacts\validation\pr35-verify01
```

The table records actual execution, not inferred coverage. Persistence's five directed cases are a **subset** of ordinary PlayMode, not five additional independent tests.

| Stage | Result | Tests / evidence |
| --- | --- | --- |
| GitHub required static validation | **PASS** | [Static architecture and wrapper guards](https://github.com/fabrint96-ship-it/ModuleZ/actions/runs/38550368760/job/115806567170) succeeded for PR #35. |
| Local static validation | **PASS with WARN** | 32 checks: 30 PASS, 2 WARN, 0 FAIL; `artifacts/validation/static-results.json`. |
| Unity Compile | **PASS** | Exit 0, zero compilation errors; `compile.log`. |
| Unity EditMode | **PASS** | Exit 0; 45/45 global, including 44/44 ModuleZ; 0 failed or skipped; `editmode-results.xml`, `editmode.log`. |
| Unity PlayMode | **PASS** | Exit 0; 28/28, 0 failed or skipped; `playmode-results.xml`, `playmode.log`. |
| Unity Persistence | **PASS** | Exit 0; 5/5 directed PlayMode subset; `persistence-results.xml`, `persistence.log`. |
| Unity Shutdown | **PASS** | Exit 0; 1/1; exit-time autosave reached only the isolated root; `shutdown-results.xml`, `shutdown.log`. |
| Canonical Smoke | **NOT RUN** | Not authorized for this validation. |
| Independent Unity CI | **BLOCKED** | Licensing/security prerequisites remain unresolved; GitHub's static check does not execute Unity. |

EditMode includes transition cancellation, stale attempts, prior active-session restoration, partial Legacy rollback, JSON shape/numeric/compatibility cases, and failed load-application rollback. PlayMode includes New Game and Continue lifecycle cases alongside existing runtime-health and persistence tests. This does not assert that the unrun canonical smoke passed. The local static warnings are heuristic `STATIC-020` (session `Revision` and bridge `pendingAttempt`) and `STATIC-021` (the menu's visible delete-failure message); neither is an exact-rule failure or an expanded allowlist. Compile logged C# warnings but no compilation errors. Unity logs also contained a transient license-token update error followed by Unity Personal entitlement resolution and shutdown-thread messages; the wrapper stages exited 0 and their required XML passed.

## Real-profile integrity and Git recovery

The wrapper created an isolated storage root per Unity process, tracked process exit, and checked the real save and settings **after each Unity process closed**. Independent before/after checks found existence, byte length, UTC modification time, and SHA-256 unchanged:

| Real-profile file | Size | LastWriteTimeUtc | SHA-256 before and after |
| --- | ---: | --- | --- |
| `modulez_save.json` | 3744 bytes | `2026-10-11T18:44:32.9357444Z` | `FAAD98E4BC84E89D79A31770A9921FC1A633782377B2DCF397B48AC2528F0B6B` |
| `module_z_settings.json` | 59 bytes | `2026-09-02T03:07:45.1036943Z` | `8522EB940EAC8C52A1AAA66122DF1410BC3A3AF6E20737DF033FFA4A8F076FCA` |

Unity repeatedly left `ProjectSettings/Packages/com.unity.testtools.codecoverage/Settings.json` marked `.M` in the validation worktree. Before PlayMode and after each of PlayMode, Persistence, and Shutdown, the Project Owner's narrow recovery exception was applied **four times**: the file was the sole marked tracked path, no change was staged, and HEAD, index, and normalized working-tree blobs all equaled `3c7b4c18bb0d72369422dce1472bb7d54e7024f2`. Only `git restore --worktree --` for that exact file was used. Each recovery was followed by a clean tracked-status and unchanged-blob check. These were controlled worktree line-ending/metadata recoveries, **not production or ProjectSettings changes in PR #35**. The unrelated untracked `.vscode/` and generated `.slnx` remained untouched.

## Known limitations and remaining obligations

- A scene-loading failure reported asynchronously after the synchronous load invocation returns is not rolled back by the attempt bridge.
- Legacy fields have no per-field ownership revision; a foreign write of the *same value* cannot be distinguished from the attempt's own projection.
- The in-memory load snapshot cannot reverse side effects performed by an arbitrary external achievement subscriber.
- Canonical Smoke was not executed in this verification.
- Independent Unity CI remains blocked by licensing/security and is not replaced by local Unity validation or the GitHub static check.

The final architectural review and any EP-02-02 closure decision remain separate. This documentation record neither changes the original REC-007 closure criteria nor resolves REC-008's remaining Legacy state.
