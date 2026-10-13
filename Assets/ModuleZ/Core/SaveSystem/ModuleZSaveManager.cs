using System;
using System.IO;
using System.Text.RegularExpressions;
using ModuleZ.Core.Managers;
using ModuleZ.Core.Achievements;
using UnityEngine;

namespace ModuleZ.Core.SaveSystem
{
    public enum ModuleZLoadGameStatus
    {
        Success,
        NotFound,
        InvalidData,
        ReadError
    }

    public static class ModuleZSaveManager
    {
        private const string SaveFileName = "modulez_save.json";

        private static string SavePath
        {
            get
            {
                return Path.Combine(
                    ModuleZPersistencePaths.RootPath,
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
            Directory.CreateDirectory(ModuleZPersistencePaths.RootPath);
            File.WriteAllText(SavePath, json);

            Debug.Log("[ModuleZ] Partida guardada: " + SavePath);
        }

        public static void LoadGame()
        {
            TryLoadGame();
        }

        public static ModuleZLoadGameStatus TryLoadGame()
        {
            ModuleZLoadGameStatus status = TryReadSaveData(out ModuleZSaveData data);
            if (status != ModuleZLoadGameStatus.Success)
                return status;

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
            return ModuleZLoadGameStatus.Success;
        }

        private static ModuleZLoadGameStatus TryReadSaveData(out ModuleZSaveData data)
        {
            data = null;
            string json;
            try
            {
                json = File.ReadAllText(SavePath);
            }
            catch (FileNotFoundException)
            {
                Debug.Log("[ModuleZ] No existe partida guardada.");
                return ModuleZLoadGameStatus.NotFound;
            }
            catch (DirectoryNotFoundException)
            {
                Debug.Log("[ModuleZ] No existe partida guardada.");
                return ModuleZLoadGameStatus.NotFound;
            }
            catch (Exception exception) when (exception is IOException ||
                exception is UnauthorizedAccessException ||
                exception is ArgumentException ||
                exception is System.Security.SecurityException ||
                exception is NotSupportedException)
            {
                Debug.LogWarning("[ModuleZ] No se pudo leer la partida: " + exception.Message);
                return ModuleZLoadGameStatus.ReadError;
            }

            try
            {
                string trimmed = json.Trim();
                // All saves written to modulez_save.json have these core
                // fields. Later progression and achievement fields are optional.
                if (!trimmed.StartsWith("{", StringComparison.Ordinal) ||
                    !trimmed.EndsWith("}", StringComparison.Ordinal) ||
                    !Regex.IsMatch(json, "\\\"duelsWon\\\"\\s*:\\s*-?\\d+") ||
                    !Regex.IsMatch(json, "\\\"duelsLost\\\"\\s*:\\s*-?\\d+") ||
                    !Regex.IsMatch(json, "\\\"duelsAbandoned\\\"\\s*:\\s*-?\\d+"))
                    return ModuleZLoadGameStatus.InvalidData;

                data = JsonUtility.FromJson<ModuleZSaveData>(json);
            }
            catch (Exception exception) when (exception is ArgumentException ||
                exception is FormatException)
            {
                Debug.LogWarning("[ModuleZ] SaveData inválido: " + exception.Message);
                return ModuleZLoadGameStatus.InvalidData;
            }

            if (!IsValid(data))
            {
                Debug.LogWarning("[ModuleZ] SaveData inválido.");
                return ModuleZLoadGameStatus.InvalidData;
            }

            return ModuleZLoadGameStatus.Success;
        }

        private static bool IsValid(ModuleZSaveData data)
        {
            return data != null &&
                !float.IsNaN(data.returnX) && !float.IsInfinity(data.returnX) &&
                !float.IsNaN(data.returnY) && !float.IsInfinity(data.returnY) &&
                !float.IsNaN(data.returnZ) && !float.IsInfinity(data.returnZ) &&
                data.duelsWon >= 0 && data.duelsLost >= 0 &&
                data.duelsAbandoned >= 0 && data.rematchesWon >= 0 &&
                data.rematchesLost >= 0 && data.rematchesAbandoned >= 0 &&
                Enum.IsDefined(data.currentOpenWorldTheme.GetType(),
                    data.currentOpenWorldTheme);
        }

        public static void DeleteSave()
        {
            TryDeleteSave();
        }

        public static bool TryDeleteSave()
        {
            try
            {
                // File.Delete is a no-op for a missing file, but unlike a
                // File.Exists guard it reports access and I/O failures.
                File.Delete(SavePath);
                ModuleZAchievementManager.ResetAll();
                Debug.Log("[ModuleZ] Partida eliminada.");
                return true;
            }
            catch (Exception exception) when (exception is IOException ||
                exception is UnauthorizedAccessException ||
                exception is ArgumentException ||
                exception is System.Security.SecurityException ||
                exception is NotSupportedException)
            {
                Debug.LogWarning("[ModuleZ] No se pudo borrar la partida: " + exception.Message);
                return false;
            }
        }

        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public static ModuleZSaveData GetSaveData()
        {
            return TryReadSaveData(out ModuleZSaveData data) ==
                ModuleZLoadGameStatus.Success ? data : null;
        }
    }
}
