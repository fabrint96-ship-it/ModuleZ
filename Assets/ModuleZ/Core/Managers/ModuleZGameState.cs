using ModuleZ.OpenWorld.Encounters;
using UnityEngine;
using ModuleZ.Duel.Runtime;
using ModuleZ.OpenWorld.Runtime;

namespace ModuleZ.Core.Managers
{
    public static class ModuleZGameState
    {
        public static Vector3 OpenWorldReturnPosition = new Vector3(0f, 0.1f, -4f);
        public static bool ReturningFromDuel = false;

        public static ModuleZRivalId CurrentDuelRival = ModuleZRivalId.Madrid;
        public static ModuleZDuelThemeId CurrentDuelTheme = ModuleZDuelThemeId.Madrid70s;

        public static bool RivalMadridDefeated = false;
        public static bool RivalBarcelonaDefeated = false;
        public static bool RivalValenciaDefeated = false;

        public static int DuelsWon = 0;
        public static int DuelsLost = 0;
        public static int DuelsAbandoned = 0;

        public static bool DuelWasCancelled = false;
        public static bool DuelCompleted = false;
        public static bool DuelWasLost = false;
        public static bool DuelWasAbandoned = false;

        public static bool AndaluciaUnlocked = false;
        public static bool RivalAndaluciaDefeated = false;

        public static string LastDuelResultMessage = "";

        public static bool IsPaused = false;

        public static bool AudioEnabled = true;
        public static bool FullscreenEnabled = true;

        public static OpenWorldThemeId CurrentOpenWorldTheme = OpenWorldThemeId.Madrid70s;
        public static string PendingOpenWorldMessage = "";
    }
}