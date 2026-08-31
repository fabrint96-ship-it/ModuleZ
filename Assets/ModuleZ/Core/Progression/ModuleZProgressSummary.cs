using ModuleZ.Core.Managers;
using ModuleZ.Duel3D.AI;

namespace ModuleZ.Core.Progression
{
    public static class ModuleZProgressSummary
    {
        public static string GetSummary()
        {
            float rivalProgress =
                Duel3DAIDifficultyScaler.GetProgressByRivalsDefeated(
                    ModuleZGameState.RivalMadridDefeated,
                    ModuleZGameState.RivalBarcelonaDefeated,
                    ModuleZGameState.RivalValenciaDefeated,
                    ModuleZGameState.RivalAndaluciaDefeated
                );

            float duelWinProgress =
                Duel3DAIDifficultyScaler.GetProgressByDuelWins(
                    ModuleZGameState.DuelsWon
                );

            float rematchProgress =
                Duel3DAIDifficultyScaler.GetProgressByRematchesWon(
                    ModuleZGameState.RematchesWon
                );

            float aiProgress =
                Duel3DAIDifficultyScaler.CombineProgress(
                    rivalProgress,
                    duelWinProgress,
                    rematchProgress
                );

            return
                "=== MODULE Z ===\n" +
                "\n" +
                "Madrid: " + GetState(ModuleZGameState.RivalMadridDefeated) + "\n" +
                "Barcelona: " + GetState(ModuleZGameState.RivalBarcelonaDefeated) + "\n" +
                "Valencia: " + GetState(ModuleZGameState.RivalValenciaDefeated) + "\n" +
                "Andalucía: " + GetState(ModuleZGameState.RivalAndaluciaDefeated) + "\n" +
                "\n" +
                "Duelos ganados: " + ModuleZGameState.DuelsWon + "\n" +
                "Duelos perdidos: " + ModuleZGameState.DuelsLost + "\n" +
                "Duelos abandonados: " + ModuleZGameState.DuelsAbandoned + "\n" +
                "\n" +
                "Rematches ganados: " + ModuleZGameState.RematchesWon + "\n" +
                "Rematches perdidos: " + ModuleZGameState.RematchesLost + "\n" +
                "Rematches abandonados: " + ModuleZGameState.RematchesAbandoned + "\n" +
                "\n" +
                "IA: " + (int)(aiProgress * 100f) + "%\n" +
                "\n" +
                "Logros: " +
                ModuleZ.Core.Achievements.ModuleZAchievementManager.GetUnlockedCount() +
                " / " +
                ModuleZ.Core.Achievements.ModuleZAchievementManager.GetTotalCount() +
                "\n" +
                "Campaña: " +
                (ModuleZGameState.MainProgressionCompleted
                    ? "COMPLETADA"
                    : "EN PROGRESO");
        }

        public static int GetCompletionPercent()
        {
            int completed = 0;

            if (ModuleZGameState.RivalMadridDefeated)
                completed++;

            if (ModuleZGameState.RivalBarcelonaDefeated)
                completed++;

            if (ModuleZGameState.RivalValenciaDefeated)
                completed++;

            if (ModuleZGameState.RivalAndaluciaDefeated)
                completed++;

            return completed * 25;
        }

        public static int GetDefeatedRivalsCount()
        {
            int count = 0;

            if (ModuleZGameState.RivalMadridDefeated)
                count++;

            if (ModuleZGameState.RivalBarcelonaDefeated)
                count++;

            if (ModuleZGameState.RivalValenciaDefeated)
                count++;

            if (ModuleZGameState.RivalAndaluciaDefeated)
                count++;

            return count;
        }

        public static string GetCurrentObjective()
        {
            if (!ModuleZGameState.RivalMadridDefeated)
                return "Derrota al rival de Madrid";

            if (!ModuleZGameState.RivalBarcelonaDefeated)
                return "Derrota al rival de Barcelona";

            if (!ModuleZGameState.RivalValenciaDefeated)
                return "Derrota al rival de Valencia";

            if (!ModuleZGameState.RivalAndaluciaDefeated)
                return "Derrota al rival de Andalucía";

            return "Progresión principal completada";
        }

        private static string GetState(bool defeated)
        {
            return defeated
                ? "DERROTADO"
                : "PENDIENTE";
        }
    }
}