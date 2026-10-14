# EP-02-03 / PERSIST-01 — Save Integrity & Safe Commit

## Scope and baseline

Implementation baseline: `develop @ 18b3e31d52deed21f8fd321a4dee70934bce0aec`.
This slice changes only save-file publication and recovery in `ModuleZSaveManager`, plus isolated PlayMode regression tests. It does not implement PERSIST-02, alter the JSON schema, or change Application State ownership.

## Existing contracts and compatibility

`SaveGame()` remains a `void` public entry point for autosave, pause menu, duel result, and portal callers. It delegates to the new `TrySaveGame()` result API and logs a warning on failure. `TryLoadGame()` retains its existing `ModuleZLoadGameStatus` values and application rollback. `GetSaveData()`, `HasSave()`, `DeleteSave()`, and `TryDeleteSave()` retain their signatures. The filename remains `modulez_save.json`; the existing root-aware JSON validation and `ModuleZSaveData` serialization format are unchanged. Missing optional fields in current-name historical saves remain accepted.

## Publication contract

1. Capture the same GameState and achievement payload previously written by `SaveGame()`.
2. Serialize and validate the JSON and in-memory data.
3. Create `modulez_save.json.pending` in the selected persistence root using create-new semantics. Write and flush it, then parse and validate it through the existing load validator.
4. If there is no main or recovery copy, move the verified pending file to the main filename without overwrite. If a valid main exists, replace it while placing its prior contents at `modulez_save.json.bak`.
5. If the main is invalid or unreadable, or the main is absent while a recovery copy exists, fail closed. The caller receives `ExistingSaveInvalid`. No file is promoted automatically in an ambiguous state.

`TrySaveGame()` reports `Success`, `CaptureError`, `SerializationError`, `TemporaryWriteError`, `ExistingSaveInvalid`, `PublicationError`, or `UnsupportedPlatform`. A failed capture/serialization does not touch disk. A failed temporary write removes only a pending file created by this call when deletion succeeds; an existing pending file is never truncated or deleted. A verified pending file is retained after failed publication for diagnosis and blocks later writes until explicitly resolved.

This avoids the former direct truncation of the main file. It does **not** claim power-loss durability, multi-process transactionality, or identical atomicity across operating systems. `File.Replace` is used only for a valid existing main; if unavailable, the operation fails without a copy/delete or overwrite-move fallback. Microsoft's API documentation describes backup replacement and platform limitations for [`File.Replace`](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.replace?view=net-7.0), while [`File.Move`](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.move?view=netstandard-2.0) does not justify treating copy/delete fallback as atomic. Unity/platform runtime behavior still requires authorized validation.

## Recovery and deletion policy

| Disk state | Load/read | Save |
| --- | --- | --- |
| Valid main, with or without older backup | Use main | Replace valid main, retain previous main as backup |
| No main and no backup | `NotFound` | First creation allowed |
| No main but backup exists | `NotFound`; no automatic resurrection | Fail closed |
| Pending incomplete or valid, unconfirmed | Ignore pending on load | Create-new refuses to overwrite it |
| Invalid main and valid backup | Read backup in memory with explicit warning; do not rewrite main | Fail closed until reviewed |
| Invalid main without valid backup | `InvalidData` | Fail closed |
| Unreadable main | `ReadError` | Fail closed; do not assume backup is newer |

`HasSave()` continues to indicate main-file presence only. Explicit `TryDeleteSave()` removes the main, then the pending file, then the backup; the backup is left until last so an interrupted deletion does not discard a recovery copy prematurely. If a later deletion step fails, the operation reports failure and may leave a partial file set for review. No recovery copy or pending file is ever automatically promoted. Manual repair of an ambiguous file set is deliberately outside this slice.

## Tests and isolation

`SaveIntegrityTests` belongs to the existing PlayMode `Persistence` category. It requires the process isolation marker and root, creates a unique child directory, and uses only the public production API via the established Assembly-CSharp contract probe. The fixture restores achievement state and clears its root override in teardown. It never addresses the real save/settings filenames.

| Test | Contract |
| --- | --- |
| PERSIST-SAFE-001 | First save, legacy `SaveGame()` signature, `HasSave`/`GetSaveData` |
| PERSIST-SAFE-002 | Replacement leaves the previous valid JSON in backup |
| PERSIST-SAFE-003 | Failed pending write leaves valid main and foreign pending untouched |
| PERSIST-SAFE-004 | Failed publication leaves valid main and verified pending |
| PERSIST-SAFE-005 | Corrupt main reads valid backup without promotion; new save fails closed |
| PERSIST-SAFE-006 | Valid main outranks older backup |
| PERSIST-SAFE-007 | Unconfirmed pending is not promoted or overwritten |
| PERSIST-SAFE-008 | Orphaned backup blocks ambiguous first publication |
| PERSIST-SAFE-009 | Explicit deletion clears isolated main and sidecars |

These tests also exercise the existing historical current-name JSON shape with omitted optional fields. Unity compile and PlayMode execution are **NOT RUN** in this implementation authorization; no test is claimed PASS until the Project Owner separately authorizes Unity validation. The official wrapper's process-root and post-exit real-profile guards remain required for that later run.

## Validation and limitations

Static validation and `git diff --check` are the only authorized local executions for this slice. A future controlled Unity run must verify Editor compilation, PlayMode tests, actual `File.Replace` behavior on the target platform, wrapper exit codes/XML, and real-profile fingerprints after the Unity process exits. A rejected save can still have updated in-memory achievement evaluation, matching the former capture behavior; no claim of an application-state transaction is made. Concurrent independent save writers and manual recovery/repair are deferred.
