using System;
using ModuleZ.Core.Managers;
using ModuleZ.Core.SceneLoading;
using ModuleZ.OpenWorld.Encounters;
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

            internal void Restore()
            {
                ModuleZGameState.PendingDuelRival = PendingRival;
                ModuleZGameState.CurrentDuelRival = CurrentRival;
                ModuleZGameState.CurrentDuelIsRematch = IsRematch;
                ModuleZGameState.OpenWorldReturnPosition = ReturnPosition;
                ModuleZGameState.ReturningFromDuel = Returning;
                ModuleZGameState.DuelCompleted = Completed;
                ModuleZGameState.DuelWasCancelled = Cancelled;
                ModuleZGameState.DuelWasLost = Lost;
                ModuleZGameState.DuelWasAbandoned = Abandoned;
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
        }

        private static Attempt pendingAttempt;

        // Compatibility entry point: callers that do not retain an attempt
        // cannot later request a rollback. Production uses the overload below.
        public static bool Begin(DuelContext context, out string failureReason)
        {
            if (!IsValid(context, out failureReason))
                return false;

            Publish(context);
            return true;
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
            Publish(context);
            attempt.PublishedRevision = ModuleZDuelSessionState.Revision;
            pendingAttempt = attempt;
            return true;
        }

        public static bool Cancel(Attempt attempt)
        {
            if (attempt == null || attempt.Committed ||
                !ReferenceEquals(pendingAttempt, attempt))
                return false;

            // Never restore a snapshot over another writer's session or
            // Legacy projection.
            if (ModuleZDuelSessionState.Revision != attempt.PublishedRevision)
            {
                pendingAttempt = null;
                return false;
            }

            if (!attempt.Previous.EqualsPublished(attempt.Context))
            {
                // The session is still ours, but another writer owns at least
                // one Legacy field. Retire our session without touching theirs.
                RestoreSession(attempt);
                pendingAttempt = null;
                return false;
            }

            attempt.Previous.Restore();
            RestoreSession(attempt);
            pendingAttempt = null;
            return true;
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
            attempt.Committed = true;
            pendingAttempt = null;
            try
            {
                ModuleZSceneController.Instance.LoadDuel();
                failureReason = null;
                return true;
            }
            catch (Exception exception)
            {
                failureReason = "Duel scene load failed: " + exception.Message;
                // Undo only a synchronous failure in the original scene.
                if (SceneManager.GetActiveScene().handle == attempt.OriginSceneHandle)
                {
                    attempt.Committed = false;
                    pendingAttempt = attempt;
                    if (Cancel(attempt))
                        ModuleZGameState.IsPaused = wasPaused;
                }
                return false;
            }
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
