using System.Collections.Generic;
using ModuleZ.Core.Managers;
using ModuleZ.Duel3D.AI;
using ModuleZ.OpenWorld.Runtime;
using UnityEngine;

namespace ModuleZ.Core.Achievements
{
    public static class ModuleZAchievementManager
    {
        private static readonly Dictionary<ModuleZAchievementId, ModuleZAchievementState> achievements =
            new Dictionary<ModuleZAchievementId, ModuleZAchievementState>();

        private static bool initialized;

        public static System.Action<ModuleZAchievementId> OnAchievementUnlocked;

        public static void Initialize()
        {
            if (initialized)
                return;

            achievements.Clear();

            foreach (ModuleZAchievementId id in System.Enum.GetValues(typeof(ModuleZAchievementId)))
                achievements[id] = new ModuleZAchievementState(id);

            initialized = true;
        }

        public static bool IsUnlocked(ModuleZAchievementId id)
        {
            Initialize();
            return achievements.ContainsKey(id) && achievements[id].unlocked;
        }

        public static void Unlock(ModuleZAchievementId id)
        {
            Initialize();

            if (!achievements.ContainsKey(id))
                achievements[id] = new ModuleZAchievementState(id);

            if (achievements[id].unlocked)
                return;

            achievements[id].Unlock();

            Debug.Log("[ModuleZ] Logro desbloqueado: " + id);

            OnAchievementUnlocked?.Invoke(id);
        }

        public static void EvaluateAll()
        {
            Initialize();

            if (ModuleZGameState.DuelsWon + ModuleZGameState.DuelsLost + ModuleZGameState.DuelsAbandoned > 0)
                Unlock(ModuleZAchievementId.FirstDuel);

            if (ModuleZGameState.DuelsWon > 0)
                Unlock(ModuleZAchievementId.FirstVictory);

            if (ModuleZGameState.RivalMadridDefeated)
                Unlock(ModuleZAchievementId.ConquerorMadrid);

            if (ModuleZGameState.RivalBarcelonaDefeated)
                Unlock(ModuleZAchievementId.ConquerorBarcelona);

            if (ModuleZGameState.RivalValenciaDefeated)
                Unlock(ModuleZAchievementId.ConquerorValencia);

            if (ModuleZGameState.RivalAndaluciaDefeated)
                Unlock(ModuleZAchievementId.ConquerorAndalucia);

            if (ModuleZGameState.MainProgressionCompleted)
                Unlock(ModuleZAchievementId.MainCampaignCompleted);

            if (ModuleZGameState.DuelsWon >= 10)
                Unlock(ModuleZAchievementId.TenVictories);

            if (ModuleZGameState.DuelsWon >= 25)
                Unlock(ModuleZAchievementId.TwentyFiveVictories);

            if (ModuleZGameState.DuelsWon >= 50)
                Unlock(ModuleZAchievementId.FiftyVictories);

            if (ModuleZGameState.RematchesWon + ModuleZGameState.RematchesLost + ModuleZGameState.RematchesAbandoned > 0)
                Unlock(ModuleZAchievementId.FirstRematch);

            if (ModuleZGameState.RematchesWon >= 10)
                Unlock(ModuleZAchievementId.TenRematchesWon);

            if (ModuleZGameState.RematchesWon >= 25)
                Unlock(ModuleZAchievementId.TwentyFiveRematchesWon);

            EvaluateAIAchievements();
            EvaluateVisitAchievements();
            EvaluatePersonalityAchievements();
            EvaluateMasterAchievements();
        }

        private static void EvaluatePersonalityAchievements()
        {
            if (ModuleZGameState.RivalMadridPersonalityCompleted)
                Unlock(ModuleZAchievementId.MetMiguel);

            if (ModuleZGameState.RivalBarcelonaPersonalityCompleted)
                Unlock(ModuleZAchievementId.MetJordi);

            if (ModuleZGameState.RivalValenciaPersonalityCompleted)
                Unlock(ModuleZAchievementId.MetVicent);

            if (ModuleZGameState.RivalAndaluciaPersonalityCompleted)
                Unlock(ModuleZAchievementId.MetAntonio);

            bool allPersonalitiesCompleted =
                ModuleZGameState.RivalMadridPersonalityCompleted &&
                ModuleZGameState.RivalBarcelonaPersonalityCompleted &&
                ModuleZGameState.RivalValenciaPersonalityCompleted &&
                ModuleZGameState.RivalAndaluciaPersonalityCompleted;

            if (allPersonalitiesCompleted)
                Unlock(ModuleZAchievementId.AllPersonalitiesDiscovered);
        }

        private static void EvaluateAIAchievements()
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

            if (aiProgress >= 0.5f)
                Unlock(ModuleZAchievementId.AILevel50);

            if (aiProgress >= 1f)
                Unlock(ModuleZAchievementId.AILevel100);
        }

        private static void EvaluateVisitAchievements()
        {
            switch (ModuleZGameState.CurrentOpenWorldTheme)
            {
                case OpenWorldThemeId.Madrid70s:
                    Unlock(ModuleZAchievementId.VisitMadrid);
                    break;

                case OpenWorldThemeId.Barcelona70s:
                    Unlock(ModuleZAchievementId.VisitBarcelona);
                    break;

                case OpenWorldThemeId.Valencia70s:
                    Unlock(ModuleZAchievementId.VisitValencia);
                    break;

                case OpenWorldThemeId.Andalucia70s:
                    Unlock(ModuleZAchievementId.VisitAndalucia);
                    break;
            }
        }

        private static void EvaluateMasterAchievements()
        {
            bool allRivalsDefeated =
                ModuleZGameState.RivalMadridDefeated &&
                ModuleZGameState.RivalBarcelonaDefeated &&
                ModuleZGameState.RivalValenciaDefeated &&
                ModuleZGameState.RivalAndaluciaDefeated;

            if (allRivalsDefeated)
                Unlock(ModuleZAchievementId.PuzzleMaster);

            if (allRivalsDefeated &&
                ModuleZGameState.MainProgressionCompleted &&
                ModuleZGameState.DuelsWon >= 10)
            {
                Unlock(ModuleZAchievementId.ModuleZMaster);
            }
        }

        public static List<ModuleZAchievementState> GetAll()
        {
            Initialize();
            return new List<ModuleZAchievementState>(achievements.Values);
        }

        public static void ResetAll()
        {
            Initialize();

            foreach (ModuleZAchievementState achievement in achievements.Values)
                achievement.Reset();
        }

        public static void LoadStates(List<ModuleZAchievementState> loadedStates)
        {
            Initialize();

            if (loadedStates == null)
                return;

            for (int i = 0; i < loadedStates.Count; i++)
            {
                ModuleZAchievementState state = loadedStates[i];

                if (state == null)
                    continue;

                achievements[state.achievementId] = state;
            }
        }

        public static int GetUnlockedCount()
        {
            Initialize();

            int count = 0;

            foreach (ModuleZAchievementState achievement in achievements.Values)
            {
                if (achievement.unlocked)
                    count++;
            }

            return count;
        }

        public static int GetTotalCount()
        {
            Initialize();
            return achievements.Count;
        }
    }
}