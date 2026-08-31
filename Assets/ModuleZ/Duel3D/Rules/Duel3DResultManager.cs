using ModuleZ.Core.Managers;
using ModuleZ.Core.SaveSystem;
using ModuleZ.Core.SceneLoading;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Duel3D.Rules
{
    public class Duel3DResultManager : MonoBehaviour
    {
        public static Duel3DResultManager Instance { get; private set; }

        private bool resultResolved;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void WinDuel()
        {
            if (resultResolved)
                return;

            resultResolved = true;

            ModuleZRivalId defeatedRival =
                ModuleZDuelSessionState.HasActiveDuel
                    ? ModuleZDuelSessionState.RivalId
                    : ModuleZGameState.CurrentDuelRival;

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.DuelsWon++;

            string victoryMessage;

            if (ModuleZGameState.CurrentDuelIsRematch)
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
                ModuleZGameState.CurrentDuelRival
            );
        }

        public void LoseDuel()
        {
            if (resultResolved)
                return;

            resultResolved = true;

            ModuleZRivalId defeatedRival =
                ModuleZDuelSessionState.HasActiveDuel
                    ? ModuleZDuelSessionState.RivalId
                    : ModuleZGameState.CurrentDuelRival;

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = true;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.DuelsLost++;

            if (ModuleZGameState.CurrentDuelIsRematch)
                ModuleZGameState.RematchesLost++;

            string defeatMessage = ModuleZGameState.CurrentDuelIsRematch
                ? "Derrota en rematch contra " + GetRivalName(ModuleZGameState.CurrentDuelRival)
                : "Derrota contra " + GetRivalName(ModuleZGameState.CurrentDuelRival);

            ModuleZGameState.LastDuelResultMessage = defeatMessage;
            ModuleZGameState.PendingOpenWorldMessage = defeatMessage;

            ModuleZGameState.CurrentDuelIsRematch = false;

            ModuleZSaveManager.SaveGame();

            ModuleZDuelSessionState.Clear();

            Debug.Log("[ModuleZ] Duel3D resultado: Derrota.");

            Invoke(nameof(ReturnToOpenWorld), 1.5f);
        }

        public void AbandonDuel()
        {
            if (resultResolved)
                return;

            resultResolved = true;

            ModuleZRivalId defeatedRival =
                ModuleZDuelSessionState.HasActiveDuel
                    ? ModuleZDuelSessionState.RivalId
                    : ModuleZGameState.CurrentDuelRival;

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = true;
            ModuleZGameState.DuelsAbandoned++;

            if (ModuleZGameState.CurrentDuelIsRematch)
                ModuleZGameState.RematchesAbandoned++;

            string abandonMessage = ModuleZGameState.CurrentDuelIsRematch
                ? "Rematch abandonado contra " + GetRivalName(ModuleZGameState.CurrentDuelRival)
                : "Duelo abandonado contra " + GetRivalName(ModuleZGameState.CurrentDuelRival);

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