using System;
using System.Collections;
using ModuleZ.Core.Managers;
using ModuleZ.Core.SceneLoading;
using ModuleZ.Core.SaveSystem;
using ModuleZ.OpenWorld.Encounters;
using ModuleZ.Duel3D.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModuleZ.Game.DuelTransition
{
    public static class DuelTransitionBridge
    {
        public sealed class Attempt
        {
            internal readonly DuelContext Context;
            internal readonly LegacyState Previous;
            internal readonly bool PreviousSessionActive;
            internal readonly ModuleZRivalId PreviousRival;
            internal readonly bool PreviousRematch;
            internal readonly Vector3 PreviousReturnPosition;
            internal readonly int OriginSceneHandle;
            internal ulong PublishedRevision;
            internal bool Committed;
            internal bool LoadRequested;
            internal int DuelSceneHandle = -1;
            public bool IsLoadRequested => LoadRequested;
            public bool IsReadyConfirmed => Committed;

            internal Attempt(DuelContext context)
            {
                Context = context;
                Previous = LegacyState.Capture();
                PreviousSessionActive = ModuleZDuelSessionState.HasActiveDuel;
                PreviousRival = ModuleZDuelSessionState.RivalId;
                PreviousRematch = ModuleZDuelSessionState.IsRematch;
                PreviousReturnPosition = ModuleZDuelSessionState.ReturnPosition;
                OriginSceneHandle = SceneManager.GetActiveScene().handle;
            }
        }

        internal struct LegacyState
        {
            internal ModuleZRivalId PendingRival;
            internal ModuleZRivalId CurrentRival;
            internal bool IsRematch;
            internal Vector3 ReturnPosition;
            internal bool Returning;
            internal bool Completed;
            internal bool Cancelled;
            internal bool Lost;
            internal bool Abandoned;

            internal static LegacyState Capture()
            {
                return new LegacyState {
                    PendingRival = ModuleZGameState.PendingDuelRival,
                    CurrentRival = ModuleZGameState.CurrentDuelRival,
                    IsRematch = ModuleZGameState.CurrentDuelIsRematch,
                    ReturnPosition = ModuleZGameState.OpenWorldReturnPosition,
                    Returning = ModuleZGameState.ReturningFromDuel,
                    Completed = ModuleZGameState.DuelCompleted,
                    Cancelled = ModuleZGameState.DuelWasCancelled,
                    Lost = ModuleZGameState.DuelWasLost,
                    Abandoned = ModuleZGameState.DuelWasAbandoned
                };
            }

            internal bool EqualsPublished(DuelContext context)
            {
                return ModuleZGameState.PendingDuelRival == context.RivalId &&
                    ModuleZGameState.CurrentDuelRival == context.RivalId &&
                    ModuleZGameState.CurrentDuelIsRematch == context.IsRematch &&
                    ModuleZGameState.OpenWorldReturnPosition == context.ReturnPosition &&
                    ModuleZGameState.ReturningFromDuel &&
                    !ModuleZGameState.DuelCompleted &&
                    !ModuleZGameState.DuelWasCancelled &&
                    !ModuleZGameState.DuelWasLost &&
                    !ModuleZGameState.DuelWasAbandoned;
            }

            // Release only projections still carrying this attempt's value.
            // A different value belongs to another writer and is never replaced.
            internal bool RestoreOwned(DuelContext context)
            {
                bool complete = true;
                if (ModuleZGameState.PendingDuelRival == context.RivalId)
                    ModuleZGameState.PendingDuelRival = PendingRival;
                else complete = false;
                if (ModuleZGameState.CurrentDuelRival == context.RivalId)
                    ModuleZGameState.CurrentDuelRival = CurrentRival;
                else complete = false;
                if (ModuleZGameState.CurrentDuelIsRematch == context.IsRematch)
                    ModuleZGameState.CurrentDuelIsRematch = IsRematch;
                else complete = false;
                if (ModuleZGameState.OpenWorldReturnPosition == context.ReturnPosition)
                    ModuleZGameState.OpenWorldReturnPosition = ReturnPosition;
                else complete = false;
                if (ModuleZGameState.ReturningFromDuel)
                    ModuleZGameState.ReturningFromDuel = Returning;
                else complete = false;
                if (!ModuleZGameState.DuelCompleted)
                    ModuleZGameState.DuelCompleted = Completed;
                else complete = false;
                if (!ModuleZGameState.DuelWasCancelled)
                    ModuleZGameState.DuelWasCancelled = Cancelled;
                else complete = false;
                if (!ModuleZGameState.DuelWasLost)
                    ModuleZGameState.DuelWasLost = Lost;
                else complete = false;
                if (!ModuleZGameState.DuelWasAbandoned)
                    ModuleZGameState.DuelWasAbandoned = Abandoned;
                else complete = false;
                return complete;
            }
        }

        private static Attempt pendingAttempt;

        // Compatibility entry point: callers that do not retain an attempt
        // cannot later request a rollback. Production uses the overload below.
        public static bool Begin(DuelContext context, out string failureReason)
        {
            // Retain the compatibility signature, but never publish an
            // unguarded provisional projection.
            return Begin(context, out Attempt _, out failureReason);
        }

        public static bool Begin(
            DuelContext context,
            out Attempt attempt,
            out string failureReason)
        {
            attempt = null;
            if (!IsValid(context, out failureReason))
                return false;

            if (pendingAttempt != null && !Cancel(pendingAttempt))
            {
                failureReason = "Previous duel transition no longer owns its state.";
                return false;
            }

            attempt = new Attempt(context);
            if (!ModuleZSaveManager.BeginProvisionalTransition(attempt))
            {
                attempt = null;
                failureReason = "Save lifecycle is not stable for a duel transition.";
                return false;
            }
            try
            {
                Publish(context);
            }
            catch
            {
                ModuleZSaveManager.EndProvisionalTransition(attempt, false);
                pendingAttempt = null;
                throw;
            }
            attempt.PublishedRevision = ModuleZDuelSessionState.Revision;
            pendingAttempt = attempt;
            return true;
        }

        public static bool Cancel(Attempt attempt)
        {
            if (attempt == null || attempt.Committed ||
                attempt.LoadRequested ||
                !ReferenceEquals(pendingAttempt, attempt))
                return false;

            // Never restore a snapshot over another writer's session or
            // Legacy projection.
            if (ModuleZDuelSessionState.Revision != attempt.PublishedRevision)
            {
                pendingAttempt = null;
                ModuleZSaveManager.EndProvisionalTransition(attempt, false);
                return false;
            }

            bool fullyOwned = attempt.Previous.RestoreOwned(attempt.Context);
            // A foreign Legacy projection makes a former active session
            // ambiguous. Do not re-publish it against that foreign value.
            if (fullyOwned)
                RestoreSession(attempt);
            else
                ModuleZDuelSessionState.Clear();
            pendingAttempt = null;
            ModuleZSaveManager.EndProvisionalTransition(attempt, fullyOwned);
            return fullyOwned;
        }

        private static void RestoreSession(Attempt attempt)
        {
            if (attempt.PreviousSessionActive)
                ModuleZDuelSessionState.StartDuel(
                    attempt.PreviousRival,
                    attempt.PreviousRematch,
                    attempt.PreviousReturnPosition);
            else
                ModuleZDuelSessionState.Clear();
        }

        public static bool CompleteAfterDelay(
            Attempt attempt,
            out string failureReason)
        {
            if (attempt == null || attempt.Committed ||
                !ReferenceEquals(pendingAttempt, attempt))
            {
                failureReason = "Duel transition attempt is obsolete.";
                return false;
            }

            if (!IsValid(attempt.Context, out failureReason) ||
                ModuleZDuelSessionState.Revision != attempt.PublishedRevision ||
                !attempt.Previous.EqualsPublished(attempt.Context))
            {
                if (failureReason == null)
                    failureReason = "Duel transition state changed before scene load.";
                Cancel(attempt);
                return false;
            }

            if (ModuleZSceneController.Instance == null)
            {
                failureReason = "ModuleZSceneController is unavailable.";
                Cancel(attempt);
                return false;
            }

            bool wasPaused = ModuleZGameState.IsPaused;
            ModuleZGameState.IsPaused = false;
            attempt.LoadRequested = true;
            SceneManager.sceneLoaded += OnSceneLoaded;
            try
            {
                ModuleZSceneController.Instance.LoadDuel();
                if (ReferenceEquals(pendingAttempt, attempt))
                    ModuleZSceneController.Instance.StartCoroutine(WaitForReady(attempt));
                if (!ReferenceEquals(pendingAttempt, attempt) && !attempt.Committed)
                {
                    failureReason = "Duel scene rejected initialization.";
                    return false;
                }
                failureReason = null;
                return true;
            }
            catch (Exception exception)
            {
                failureReason = "Duel scene load failed: " + exception.Message;
                // Undo only a synchronous failure in the original scene.
                if (SceneManager.GetActiveScene().handle == attempt.OriginSceneHandle)
                {
                    attempt.LoadRequested = false;
                    SceneManager.sceneLoaded -= OnSceneLoaded;
                    if (Cancel(attempt))
                        ModuleZGameState.IsPaused = wasPaused;
                }
                else
                    FailReadiness(attempt);
                return false;
            }
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Attempt attempt = pendingAttempt;
            if (attempt == null || !attempt.LoadRequested)
                return;
            if (scene.name != ModuleZSceneController.DuelScene ||
                mode != LoadSceneMode.Single || attempt.DuelSceneHandle != -1)
            {
                FailReadiness(attempt);
                return;
            }
            attempt.DuelSceneHandle = scene.handle;
        }

        private static IEnumerator WaitForReady(Attempt attempt)
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (ReferenceEquals(pendingAttempt, attempt) && attempt.LoadRequested)
            {
                Scene scene = SceneManager.GetActiveScene();
                bool stillAtOrigin = attempt.DuelSceneHandle == -1 &&
                    scene.handle == attempt.OriginSceneHandle;
                if ((!stillAtOrigin &&
                     (scene.name != ModuleZSceneController.DuelScene ||
                      attempt.DuelSceneHandle == -1 ||
                      scene.handle != attempt.DuelSceneHandle)) ||
                    Time.realtimeSinceStartup >= deadline)
                {
                    FailReadiness(attempt);
                    yield break;
                }
                yield return null;
            }
        }

        // Called by the scene owner, never by LoadDuel's return path.
        public static bool IsAwaitingReadiness(Scene scene)
        {
            return pendingAttempt != null && pendingAttempt.LoadRequested &&
                scene.name == ModuleZSceneController.DuelScene;
        }

        public static bool ConfirmReady(DuelSceneRoot root, DuelContext context,
            ulong sessionRevision, bool fromSession)
        {
            Attempt attempt = pendingAttempt;
            if (attempt == null || !attempt.LoadRequested || !fromSession ||
                root == null || root.State != DuelSceneRoot.LifecycleState.Ready ||
                attempt.DuelSceneHandle == -1 ||
                root.gameObject.scene.handle != attempt.DuelSceneHandle ||
                SceneManager.GetActiveScene().handle != attempt.DuelSceneHandle ||
                !OwnsSession(attempt, context, sessionRevision))
                return false;

            attempt.Committed = true;
            attempt.LoadRequested = false;
            pendingAttempt = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            ModuleZSaveManager.EndProvisionalTransition(attempt, true);
            return true;
        }

        public static void ReportSceneFailure(DuelSceneRoot root)
        {
            Attempt attempt = pendingAttempt;
            if (attempt == null || !attempt.LoadRequested || root == null ||
                root.gameObject.scene.name != ModuleZSceneController.DuelScene ||
                root.gameObject.scene.handle != SceneManager.GetActiveScene().handle ||
                (attempt.DuelSceneHandle != -1 &&
                 root.gameObject.scene.handle != attempt.DuelSceneHandle))
                return;
            FailReadiness(attempt);
        }

        private static bool OwnsSession(Attempt attempt, DuelContext context,
            ulong revision)
        {
            return ModuleZDuelSessionState.HasActiveDuel &&
                revision == attempt.PublishedRevision &&
                ModuleZDuelSessionState.Revision == revision &&
                context.RivalId == attempt.Context.RivalId &&
                context.IsRematch == attempt.Context.IsRematch &&
                context.ReturnPosition == attempt.Context.ReturnPosition &&
                ModuleZDuelSessionState.RivalId == context.RivalId &&
                ModuleZDuelSessionState.IsRematch == context.IsRematch &&
                ModuleZDuelSessionState.ReturnPosition == context.ReturnPosition;
        }

        private static void FailReadiness(Attempt attempt)
        {
            if (!ReferenceEquals(pendingAttempt, attempt))
                return;
            pendingAttempt = null;
            attempt.LoadRequested = false;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            ModuleZSaveManager.EndProvisionalTransition(attempt, false);
            if (ModuleZDuelSessionState.HasActiveDuel &&
                ModuleZDuelSessionState.Revision == attempt.PublishedRevision &&
                ModuleZDuelSessionState.RivalId == attempt.Context.RivalId &&
                ModuleZDuelSessionState.IsRematch == attempt.Context.IsRematch &&
                ModuleZDuelSessionState.ReturnPosition == attempt.Context.ReturnPosition)
                ModuleZDuelSessionState.Clear();
            Debug.LogError("[ModuleZ] Duel scene did not confirm Ready; save remains blocked.");
        }

        private static void Publish(DuelContext context)
        {
            ModuleZGameState.PendingDuelRival = context.RivalId;
            ModuleZDuelSessionState.StartDuel(
                context.RivalId, context.IsRematch, context.ReturnPosition);
            ModuleZGameState.CurrentDuelRival = context.RivalId;
            ModuleZGameState.CurrentDuelIsRematch = context.IsRematch;
            ModuleZGameState.OpenWorldReturnPosition = context.ReturnPosition;
            ModuleZGameState.ReturningFromDuel = true;
            ModuleZGameState.DuelCompleted = false;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;
        }

        private static bool IsValid(
            DuelContext context,
            out string failureReason)
        {
            if (!Enum.IsDefined(typeof(ModuleZRivalId), context.RivalId))
            {
                failureReason = "Duel rival is invalid.";
                return false;
            }

            failureReason = null;
            return true;
        }
    }
}
