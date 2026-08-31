using ModuleZ.OpenWorld.Encounters;
using ModuleZ.OpenWorld.Runtime;
using UnityEngine;

namespace ModuleZ.Core.Managers
{
    public static class ModuleZGameState
    {
        public static Vector3 OpenWorldReturnPosition = new Vector3(0f, 0.1f, -4f);
        public static bool ReturningFromDuel = false;

        public static ModuleZRivalId CurrentDuelRival = ModuleZRivalId.Madrid;

        public static bool RivalMadridDefeated = false;
        public static bool RivalBarcelonaDefeated = false;
        public static bool RivalValenciaDefeated = false;
        public static bool RivalAndaluciaDefeated = false;

        public static bool AndaluciaUnlocked = false;

        public static int DuelsWon = 0;
        public static int DuelsLost = 0;
        public static int DuelsAbandoned = 0;

        public static bool DuelWasCancelled = false;
        public static bool DuelCompleted = false;
        public static bool DuelWasLost = false;
        public static bool DuelWasAbandoned = false;

        public static string LastDuelResultMessage = "";
        public static string PendingOpenWorldMessage = "";

        public static bool IsPaused = false;

        public static bool AudioEnabled = true;
        public static bool FullscreenEnabled = true;

        public static OpenWorldThemeId CurrentOpenWorldTheme =
            OpenWorldThemeId.Madrid70s;

        public static bool MainProgressionCompleted = false;

        public static bool CurrentDuelIsRematch = false;

        public static int RematchesWon = 0;
        public static int RematchesLost = 0;
        public static int RematchesAbandoned = 0;

        public static bool IsOverlayOpen = false;

        public static bool RivalMadridPersonalityCompleted;
        public static bool RivalBarcelonaPersonalityCompleted;
        public static bool RivalValenciaPersonalityCompleted;
        public static bool RivalAndaluciaPersonalityCompleted;

        public static ModuleZRivalId PendingDuelRival = ModuleZRivalId.Madrid;

        public static void ResetGameState()
        {
            OpenWorldReturnPosition = new Vector3(0f, 0.1f, -4f);
            ReturningFromDuel = false;

            CurrentDuelRival = ModuleZRivalId.Madrid;

            RivalMadridDefeated = false;
            RivalBarcelonaDefeated = false;
            RivalValenciaDefeated = false;
            RivalAndaluciaDefeated = false;

            AndaluciaUnlocked = false;

            DuelsWon = 0;
            DuelsLost = 0;
            DuelsAbandoned = 0;

            DuelWasCancelled = false;
            DuelCompleted = false;
            DuelWasLost = false;
            DuelWasAbandoned = false;

            LastDuelResultMessage = "";
            PendingOpenWorldMessage = "";

            IsPaused = false;

            CurrentOpenWorldTheme = OpenWorldThemeId.Madrid70s;

            MainProgressionCompleted = false;

            CurrentDuelIsRematch = false;

            RematchesWon = 0;
            RematchesLost = 0;
            RematchesAbandoned = 0;

            IsOverlayOpen = false;

            RivalMadridPersonalityCompleted = false;
            RivalBarcelonaPersonalityCompleted = false;
            RivalValenciaPersonalityCompleted = false;
            RivalAndaluciaPersonalityCompleted = false;

            PendingDuelRival = ModuleZRivalId.Madrid;
        }
    }
}