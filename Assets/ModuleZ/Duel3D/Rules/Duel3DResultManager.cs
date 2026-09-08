using ModuleZ.Core.Managers;
using ModuleZ.Core.SaveSystem;
using ModuleZ.Core.SceneLoading;
using ModuleZ.Game.DuelTransition;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Duel3D.Rules
{
    public class Duel3DResultManager : MonoBehaviour
    {
        public static Duel3DResultManager Instance { get; private set; }

        private bool resultResolved;
        private DuelContext acceptedContext;
        private bool hasAcceptedContext;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public bool Initialize(DuelContext context)
        {
            if (hasAcceptedContext)
                return false;

            acceptedContext = context;
            hasAcceptedContext = true;
            return true;
        }

        public void Complete(DuelResult result)
        {
            if (resultResolved)
                return;

            if (!hasAcceptedContext)
            {
                Debug.LogError(
                    "[ModuleZ] Duel result has no accepted DuelContext."
                );
                return;
            }

            if (!DuelResultBridge.Complete(
                    acceptedContext,
                    result,
                    ApplyCompatibilityResult,
                    out string failureReason))
            {
                Debug.LogError(
                    "[ModuleZ] Duel result rejected: " + failureReason
                );
            }
        }

        public void AbandonDuel()
        {
            if (!hasAcceptedContext)
            {
                Debug.LogError(
                    "[ModuleZ] Duel abandon has no accepted DuelContext."
                );
                return;
            }

            Complete(new DuelResult(
                DuelOutcome.Abandoned,
                acceptedContext.RivalId
            ));
        }

        private void ApplyCompatibilityResult(
            DuelContext context,
            DuelResult result)
        {
            if (resultResolved)
                return;

            resultResolved = true;

            switch (result.Outcome)
            {
                case DuelOutcome.Victory:
                    ApplyVictory(context, result);
                    break;

                case DuelOutcome.Defeat:
                    ApplyDefeat(context, result);
                    break;

                case DuelOutcome.Abandoned:
                    ApplyAbandon(context, result);
                    break;
            }
        }

        private void ApplyVictory(
            DuelContext context,
            DuelResult result)
        {
            ModuleZRivalId defeatedRival = result.RivalId;

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.DuelsWon++;

            string victoryMessage;

            if (context.IsRematch)
            {
                ModuleZGameState.RematchesWon++;

                victoryMessage =
                    "Victoria en rematch contra " +
                    GetRivalName(defeatedRival);
            }
            else
            {
                ModuleZRivalProgression.MarkRivalDefeated(defeatedRival);

                victoryMessage =
                    "Victoria contra " +
                    GetRivalName(defeatedRival);

                string unlockMessage =
                    ModuleZRivalProgression.GetUnlockMessage(defeatedRival);

                if (!string.IsNullOrEmpty(unlockMessage))
                    victoryMessage += "\n" + unlockMessage;
            }

            ModuleZGameState.LastDuelResultMessage = victoryMessage;
            ModuleZGameState.PendingOpenWorldMessage = victoryMessage;

            ModuleZGameState.CurrentDuelIsRematch = false;

            ModuleZSaveManager.SaveGame();

            ModuleZDuelSessionState.Clear();

            Debug.Log("[ModuleZ] Duel3D resultado: Victoria.");

            Invoke(nameof(ReturnToOpenWorld), 1.5f);

            Debug.Log(
                "[ModuleZ DEBUG] CurrentDuelRival = " +
                defeatedRival
            );
        }

        private void ApplyDefeat(
            DuelContext context,
            DuelResult result)
        {
            ModuleZRivalId defeatedRival = result.RivalId;

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = true;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.DuelsLost++;

            if (context.IsRematch)
                ModuleZGameState.RematchesLost++;

            string defeatMessage = context.IsRematch
                ? "Derrota en rematch contra " + GetRivalName(defeatedRival)
                : "Derrota contra " + GetRivalName(defeatedRival);

            ModuleZGameState.LastDuelResultMessage = defeatMessage;
            ModuleZGameState.PendingOpenWorldMessage = defeatMessage;

            ModuleZGameState.CurrentDuelIsRematch = false;

            ModuleZSaveManager.SaveGame();

            ModuleZDuelSessionState.Clear();

            Debug.Log("[ModuleZ] Duel3D resultado: Derrota.");

            Invoke(nameof(ReturnToOpenWorld), 1.5f);
        }

        private void ApplyAbandon(
            DuelContext context,
            DuelResult result)
        {
            ModuleZRivalId defeatedRival = result.RivalId;

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = true;
            ModuleZGameState.DuelsAbandoned++;

            if (context.IsRematch)
                ModuleZGameState.RematchesAbandoned++;

            string abandonMessage = context.IsRematch
                ? "Rematch abandonado contra " + GetRivalName(defeatedRival)
                : "Duelo abandonado contra " + GetRivalName(defeatedRival);

            ModuleZGameState.LastDuelResultMessage = abandonMessage;
            ModuleZGameState.PendingOpenWorldMessage = abandonMessage;

            ModuleZGameState.CurrentDuelIsRematch = false;

            ModuleZSaveManager.SaveGame();

            ModuleZDuelSessionState.Clear();

            Debug.Log("[ModuleZ] Duel3D resultado: Abandono.");

            Invoke(nameof(ReturnToOpenWorld), 0.8f);
        }

        private void ReturnToOpenWorld()
        {
            ModuleZGameState.ReturningFromDuel = true;
            ModuleZGameState.IsPaused = false;

            if (ModuleZSceneController.Instance != null)
                ModuleZSceneController.Instance.LoadOpenWorld();
            else
                Debug.LogError("[ModuleZ] No existe ModuleZSceneController.");
        }

        private string GetRivalName(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Rival Madrid";

                case ModuleZRivalId.Barcelona:
                    return "Rival Barcelona";

                case ModuleZRivalId.Valencia:
                    return "Rival Valencia";

                case ModuleZRivalId.Andalucia:
                    return "Rival Andalucía";

                default:
                    return "Rival";
            }
        }
    }
}
