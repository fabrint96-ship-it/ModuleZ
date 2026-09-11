using System;
using ModuleZ.Core.Managers;
using ModuleZ.Core.SceneLoading;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Game.DuelTransition
{
    public static class DuelTransitionBridge
    {
        public static bool Begin(
            DuelContext context,
            out string failureReason)
        {
            if (!IsValid(context, out failureReason))
                return false;

            ModuleZGameState.PendingDuelRival = context.RivalId;

            ModuleZDuelSessionState.StartDuel(
                context.RivalId,
                context.IsRematch,
                context.ReturnPosition
            );

            ModuleZGameState.CurrentDuelRival = context.RivalId;
            ModuleZGameState.CurrentDuelIsRematch = context.IsRematch;
            ModuleZGameState.OpenWorldReturnPosition =
                context.ReturnPosition;

            // Legacy compatibility: this flag is already set before the
            // Duel scene loads and is consumed only when OpenWorld returns.
            ModuleZGameState.ReturningFromDuel = true;

            ModuleZGameState.DuelCompleted = false;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;

            return true;
        }

        public static void CompleteAfterDelay(DuelContext context)
        {
            if (!IsValid(context, out string failureReason))
            {
                ModuleZDuelSessionState.Clear();
                Debug.LogError(
                    "[ModuleZ] Duel transition failed: " + failureReason
                );
                return;
            }

            ModuleZGameState.IsPaused = false;

            if (ModuleZSceneController.Instance != null)
            {
                ModuleZSceneController.Instance.LoadDuel();
            }
            else
            {
                ModuleZDuelSessionState.Clear();
                Debug.LogError("[Module Z] No existe ModuleZSceneController.");
            }
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
