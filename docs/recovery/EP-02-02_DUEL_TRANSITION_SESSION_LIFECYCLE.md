# EP-02-02 — Duel transition rollback and session lifecycle

## Scope and baseline

Implementation branch `fix/ep-02-02-duel-transition-session-lifecycle` starts at
`develop` commit `0a143dcf7f23285a85bf347bcdda23f6e907748a`.
This is a D2 functional change. It does not change the save schema, DuelContext,
DuelResult, scene assets, gameplay rules, or the persistence root boundary.

## Existing contracts and compatibility

`DuelStarterInteractable` is the sole production caller of
`DuelTransitionBridge.Begin`; the bridge publishes nine Legacy projections and
an active session before delayed scene loading. `DuelBootstrap` prefers that
session. The Arena, HUD, AI configuration, return spawn and result compatibility
consumers still read the Legacy projections. Their publication timing therefore
remains unchanged. The earlier two-argument `Begin` remains for existing public
consumers and the EP-02-01 contract test, but production retains the attempt
returned by the new overload.

Git history shows that `modulez_save.json` and the current `ModuleZSaveData`
fields were introduced together in the recovered Legacy snapshot (`caa726e`).
An older `a6b0d89` implementation used a **different filename**,
`module_z_save.json`, and a string theme; that file was already outside the
current loader's path at this baseline. No historical save fixture is checked
in. The new reader accepts the existing current-filename core counters and
allows later progression, achievement, personality and return fields to be
absent with their established `JsonUtility` defaults. This is not a migration
of the older filename or schema.

## Transition ownership and rollback

The production Begin overload returns a distinct `Attempt`, capturing only the
nine projected Legacy values and four session values. The session's read-only
revision changes on `StartDuel` and `Clear`. Rollback requires the same pending
attempt and its published session revision. It restores each Legacy projection
only while that field still carries the attempt's published value. A changed
field is left to its external writer; the other eight cannot retain this
attempt's provisional values. If any field changed externally, cancellation
reports incomplete rollback and clears the attempt-owned session instead of
re-publishing a previous active session against foreign Legacy state. A fully
owned cancellation restores the previous session, including a previous active
one. A stale callback cannot undo a successor. A new Begin first cancels an
owned pending attempt. Invalid contexts do not publish anything.

Legacy fields have no per-field revision. A same-value external write cannot
be distinguished from the attempt's own value; compare-and-restore is the
bounded ownership guarantee available without a new Legacy state authority.

The interactable retains the attempt across its 1.5-second delay. A rejected
completion releases `duelStarting`; disable/destroy cancels the delayed invoke
and asks the bridge to restore owned state. Missing SceneController causes
rollback. A synchronous exception from `LoadDuel` is reverted only while the
origin scene remains active. Once scene loading commits, the bridge does not
claim rollback; the existing DuelSceneRoot initialization failure cleanup
remains responsible for an active session on fatal Duel startup.

## Save loading and menu policy

`TryLoadGame` reports `Success`, `NotFound`, `InvalidData`, `ReadError`, or
`ApplyError` (a validated save whose application fails).
`LoadGame()` remains a void compatibility wrapper for SaveBootstrap. The shared
reader validates full JSON grammar, root-level required counters, known root
field types and deserialized values before applying GameState or achievement
state. Malformed JSON, nested-only core counters, incompatible types, integer
overflow, negative counts, invalid achievement IDs, duplicate achievements,
non-finite return coordinates and undefined themes are rejected. Unknown fields
and absent optional fields remain compatible with the current save format.
`GetSaveData()` uses the same safe reader for MainMenu presentation. No invalid
file is rewritten or deleted. Before application, the loader snapshots affected
GameState and achievement objects. If application or achievement notification
throws, it restores those values and reports `ApplyError`; successful-load
ordering and serialization remain unchanged. Side effects performed by an
arbitrary external achievement subscriber cannot be rolled back by this local
snapshot and are not claimed as protected state.

New Game retains two-click confirmation. Its second click uses `TryDeleteSave`;
only a successful delete resets GameState, clears DuelSession, and navigates.
Continue uses `TryLoadGame`, clears a stale session only on success, and remains
on the menu with the existing info text on failure. Failed Continue leaves
GameState, session and achievements unchanged. `DeleteSave()` remains a void
compatibility wrapper; the separate Delete Save menu operation is unchanged.

## Test contracts and isolation

EditMode `ApplicationStateTransitionEditModeTests` covers
`STATE-TRANSITION-001..006`, a previous active session, partial Legacy rollback,
malformed/nested/mistyped saves, and an injected achievement notification
failure during load. It restores public GameState, achievement object identity
and values, the notification delegate, and the persistence override in teardown.
Session restoration uses public `StartDuel`/`Clear`, keeping its revision
monotonic so stale attempts cannot regain ownership. The existing
EP-02-01 `Begin` test now selects its original overload explicitly and retains
its nine-projection assertions.

PlayMode `ApplicationStateMenuTests` covers `STATE-NEW-001..003` and
`STATE-CONTINUE-001..005` through Boot's real button actions. It requires the
wrapper's process-level isolated persistence marker before loading Boot, uses
one unique per-test root beneath that process root, and tears down its Boot
runtime. The EditMode fixture refuses a root overlapping the real profile or
resolving through a reparse-point ancestor; its cleanup targets only its unique
temporary directory. Both fixtures restore sessions through the public API and
achievement entries through `LoadStates`. The wrapper's external post-exit
real-profile fingerprint guard remains
the final safety check. These tests have **not** been run in this slice.

## Validation and known limits

The static validator and `git diff --check` are the only executable validations
authorized for this PR. At implementation review, the static validator reports
32 checks with 0 FAIL (30 PASS, 2 WARN), and `git diff --check` passes. Unity
compile, EditMode, PlayMode and persistence validation remain `NOT RUN` pending
Project Owner approval for the final SHA. Static warnings for a narrowly owned
pending attempt and a menu failure message are heuristic findings, not
allowlist changes.

The bridge cannot detect a scene-loading failure reported only after
`LoadDuel()` returns. The static transition checks are structural, not runtime
proof. The older `module_z_save.json` compatibility question predates this
slice. Historic real-profile compatibility, compile and runtime behavior must
be confirmed in the separately authorized isolated Unity validation before
merge. REC-008 remains partially migrated; REC-007, F1.6 and EP-01 remain open.

For the final PR SHA, the proposed authorized follow-up is one isolated
checkout with Unity `6000.3.13f1`, followed by the wrapper's `compile`,
`editmode`, `playmode`, `persistence`, and `shutdown` stages as explicitly
approved by the Project Owner. Inspect each exit code, XML discovery/pass/fail
counts and log; compare real save/settings fingerprints after **each Unity
process exits**. Do not run `smoke -EnableSmoke` without its own authorization.
No Unity result is inferred from the source-level static PASS.
