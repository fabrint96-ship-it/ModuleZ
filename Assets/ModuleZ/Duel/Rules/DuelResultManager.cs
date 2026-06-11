using ModuleZ.Core.Managers;
using ModuleZ.Core.SceneLoading;
using ModuleZ.Duel.Builders;
using ModuleZ.OpenWorld.Encounters;
using ModuleZ.Duel.Runtime;
using ModuleZ.Core.SaveSystem;
using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelResultManager : MonoBehaviour
    {
        public static DuelResultManager Instance { get; private set; }

        private bool resultResolved;

        private void Awake()
        {
            Instance = this;
        }

        public void WinDuel()
        {
            if (resultResolved)
                return;

            resultResolved = true;

            LockZPiece();

            if (DuelFeedbackController.Instance != null)
                DuelFeedbackController.Instance.PlayVictory();

            if (DuelAudioFeedbackController.Instance != null)
                DuelAudioFeedbackController.Instance.PlayVictorySound();

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.DuelsWon++;

            DuelThemeData themeData = GetCurrentThemeData();

            ModuleZGameState.LastDuelResultMessage =
                themeData.victoryMessage + " contra " + GetCurrentRivalName();

            ModuleZGameState.PendingOpenWorldMessage =
                ModuleZGameState.LastDuelResultMessage;

            MarkCurrentRivalDefeated();

            CheckUnlocks();

            ModuleZSaveManager.SaveGame();

            Debug.Log("[Module Z] Resultado duelo: Victoria.");

            Invoke(nameof(ReturnToOpenWorld), 1.5f);
        }

        public void LoseDuel()
        {
            if (resultResolved)
                return;

            resultResolved = true;

            LockZPiece();

            if (DuelFeedbackController.Instance != null)
                DuelFeedbackController.Instance.PlayDefeat();

            if (DuelAudioFeedbackController.Instance != null)
                DuelAudioFeedbackController.Instance.PlayDefeatSound();

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = true;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.DuelsLost++;

            DuelTimerController timer = FindObjectOfType<DuelTimerController>();
            if (timer != null)
                timer.StopTimer();

            DuelThemeData themeData = GetCurrentThemeData();

            ModuleZGameState.LastDuelResultMessage =
                themeData.defeatMessage + " contra " + GetCurrentRivalName();

            ModuleZGameState.PendingOpenWorldMessage =
                ModuleZGameState.LastDuelResultMessage;

            if (DuelHUDController.Instance != null)
                DuelHUDController.Instance.ShowMessage(themeData.defeatMessage);

            ModuleZSaveManager.SaveGame();

            Debug.Log("[Module Z] Resultado duelo: Derrota.");

            Invoke(nameof(ReturnToOpenWorld), 1.5f);
        }

        private void MarkCurrentRivalDefeated()
        {
            switch (ModuleZGameState.CurrentDuelRival)
            {
                case ModuleZRivalId.Madrid:
                    ModuleZGameState.RivalMadridDefeated = true;
                    break;

                case ModuleZRivalId.Barcelona:
                    ModuleZGameState.RivalBarcelonaDefeated = true;
                    break;

                case ModuleZRivalId.Valencia:
                    ModuleZGameState.RivalValenciaDefeated = true;
                    break;

                case ModuleZRivalId.Andalucia:
                    ModuleZGameState.RivalAndaluciaDefeated = true;
                    break;
            }
        }

        private void ReturnToOpenWorld()
        {
            ModuleZGameState.ReturningFromDuel = true;
            ModuleZSceneController.Instance.LoadOpenWorld();
        }

        public void AbandonDuel()
        {
            if (resultResolved)
                return;

            resultResolved = true;

            LockZPiece();

            if (DuelFeedbackController.Instance != null)
                DuelFeedbackController.Instance.PlayAbandon();

            if (DuelAudioFeedbackController.Instance != null)
                DuelAudioFeedbackController.Instance.PlayAbandonSound();

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = true;
            ModuleZGameState.DuelsAbandoned++;

            if (DuelHUDController.Instance != null)
                DuelHUDController.Instance.ShowMessage("Duelo abandonado");

            ModuleZGameState.LastDuelResultMessage =
                "Duelo abandonado contra " + GetCurrentRivalName();

            ModuleZSaveManager.SaveGame();

            Debug.Log("[Module Z] Resultado duelo: Abandonado.");

            ModuleZGameState.ReturningFromDuel = true;

            ModuleZGameState.PendingOpenWorldMessage =
                ModuleZGameState.LastDuelResultMessage;
            Invoke(nameof(ReturnToOpenWorld), 0.8f);
        }

        private void LockZPiece()
        {
            DuelZPieceController zPiece = FindObjectOfType<DuelZPieceController>();

            if (zPiece != null)
                zPiece.LockPiece();
        }

        private string GetCurrentRivalName()
        {
            switch (ModuleZGameState.CurrentDuelRival)
            {
                case ModuleZ.OpenWorld.Encounters.ModuleZRivalId.Madrid:
                    return "Rival Madrid";

                case ModuleZ.OpenWorld.Encounters.ModuleZRivalId.Barcelona:
                    return "Rival Barcelona";

                case ModuleZ.OpenWorld.Encounters.ModuleZRivalId.Valencia:
                    return "Rival Valencia";

                case ModuleZRivalId.Andalucia:
                    return "Rival Andalucía";

                default:
                    return "Rival";
            }
        }

        private void CheckUnlocks()
        {
            if (ModuleZGameState.AndaluciaUnlocked)
                return;

            if (
                ModuleZGameState.RivalMadridDefeated &&
                ModuleZGameState.RivalBarcelonaDefeated &&
                ModuleZGameState.RivalValenciaDefeated
            )
            {
                ModuleZGameState.AndaluciaUnlocked = true;

                ModuleZGameState.LastDuelResultMessage =
                    "Nueva zona desbloqueada: Andalucía";

                ModuleZGameState.PendingOpenWorldMessage =
                    "Nueva zona desbloqueada: Andalucía";

                Debug.Log("[Module Z] Nueva zona desbloqueada: Andalucía");
            }
        }

        private DuelThemeData GetCurrentThemeData()
        {
            return DuelThemeDatabase.GetThemeData(ModuleZGameState.CurrentDuelTheme);
        }
    }
}