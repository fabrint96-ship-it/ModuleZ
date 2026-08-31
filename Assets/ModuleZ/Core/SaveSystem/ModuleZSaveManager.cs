using System.IO;
using ModuleZ.Core.Managers;
using ModuleZ.Core.Achievements;
using UnityEngine;

namespace ModuleZ.Core.SaveSystem
{
    public static class ModuleZSaveManager
    {
        private const string SaveFileName = "modulez_save.json";

        private static string SavePath
        {
            get
            {
                return Path.Combine(
                    Application.persistentDataPath,
                    SaveFileName
                );
            }
        }

        public static void SaveGame()
        {
            ModuleZSaveData data = new ModuleZSaveData();

            data.returnX = ModuleZGameState.OpenWorldReturnPosition.x;
            data.returnY = ModuleZGameState.OpenWorldReturnPosition.y;
            data.returnZ = ModuleZGameState.OpenWorldReturnPosition.z;

            data.rivalMadridDefeated = ModuleZGameState.RivalMadridDefeated;
            data.rivalBarcelonaDefeated = ModuleZGameState.RivalBarcelonaDefeated;
            data.rivalValenciaDefeated = ModuleZGameState.RivalValenciaDefeated;
            data.rivalAndaluciaDefeated = ModuleZGameState.RivalAndaluciaDefeated;

            data.andaluciaUnlocked = ModuleZGameState.AndaluciaUnlocked;

            data.duelsWon = ModuleZGameState.DuelsWon;
            data.duelsLost = ModuleZGameState.DuelsLost;
            data.duelsAbandoned = ModuleZGameState.DuelsAbandoned;

            data.currentOpenWorldTheme = ModuleZGameState.CurrentOpenWorldTheme;

            data.mainProgressionCompleted = ModuleZGameState.MainProgressionCompleted;

            data.rematchesWon = ModuleZGameState.RematchesWon;
            data.rematchesLost = ModuleZGameState.RematchesLost;
            data.rematchesAbandoned = ModuleZGameState.RematchesAbandoned;

            data.rivalMadridPersonalityCompleted = ModuleZGameState.RivalMadridPersonalityCompleted;

            data.rivalBarcelonaPersonalityCompleted = ModuleZGameState.RivalBarcelonaPersonalityCompleted;

            data.rivalValenciaPersonalityCompleted = ModuleZGameState.RivalValenciaPersonalityCompleted;

            data.rivalAndaluciaPersonalityCompleted = ModuleZGameState.RivalAndaluciaPersonalityCompleted;

            ModuleZAchievementManager.EvaluateAll();

            data.achievements = ModuleZAchievementManager.GetAll();

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);

            Debug.Log("[ModuleZ] Partida guardada: " + SavePath);
        }

        public static void LoadGame()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("[ModuleZ] No existe partida guardada.");
                return;
            }

            string json = File.ReadAllText(SavePath);
            ModuleZSaveData data = JsonUtility.FromJson<ModuleZSaveData>(json);

            if (data == null)
            {
                Debug.LogWarning("[ModuleZ] SaveData inválido.");
                return;
            }

            ModuleZGameState.OpenWorldReturnPosition =
                new Vector3(data.returnX, data.returnY, data.returnZ);

            ModuleZGameState.RivalMadridDefeated = data.rivalMadridDefeated;
            ModuleZGameState.RivalBarcelonaDefeated = data.rivalBarcelonaDefeated;
            ModuleZGameState.RivalValenciaDefeated = data.rivalValenciaDefeated;
            ModuleZGameState.RivalAndaluciaDefeated = data.rivalAndaluciaDefeated;

            ModuleZGameState.AndaluciaUnlocked = data.andaluciaUnlocked;

            ModuleZGameState.DuelsWon = data.duelsWon;
            ModuleZGameState.DuelsLost = data.duelsLost;
            ModuleZGameState.DuelsAbandoned = data.duelsAbandoned;

            ModuleZGameState.CurrentOpenWorldTheme = data.currentOpenWorldTheme;

            ModuleZGameState.MainProgressionCompleted = data.mainProgressionCompleted;

            ModuleZGameState.RematchesWon = data.rematchesWon;
            ModuleZGameState.RematchesLost = data.rematchesLost;
            ModuleZGameState.RematchesAbandoned = data.rematchesAbandoned;

            ModuleZGameState.RivalMadridPersonalityCompleted = data.rivalMadridPersonalityCompleted;

            ModuleZGameState.RivalBarcelonaPersonalityCompleted = data.rivalBarcelonaPersonalityCompleted;

            ModuleZGameState.RivalValenciaPersonalityCompleted = data.rivalValenciaPersonalityCompleted;

            ModuleZGameState.RivalAndaluciaPersonalityCompleted = data.rivalAndaluciaPersonalityCompleted;

            ModuleZ.OpenWorld.Encounters.ModuleZRivalProgression.UpdateUnlocks();

            // Opcional
            ModuleZGameState.CurrentDuelIsRematch = false;
            ModuleZGameState.DuelCompleted = false;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.IsPaused = false;

            ModuleZAchievementManager.LoadStates(data.achievements);
            ModuleZAchievementManager.EvaluateAll();

            Debug.Log("[ModuleZ] Partida cargada.");
        }

        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);

            ModuleZAchievementManager.ResetAll();

            Debug.Log("[ModuleZ] Partida eliminada.");
        }

        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public static ModuleZSaveData GetSaveData()
        {
            if (!File.Exists(SavePath))
                return null;

            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<ModuleZSaveData>(json);
        }
    }
}