using System.IO;
using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Runtime;
using UnityEngine;

namespace ModuleZ.Core.SaveSystem
{
    public static class ModuleZSaveManager
    {
        private static string SavePath =>
            Path.Combine(Application.persistentDataPath, "module_z_save.json");

        public static void SaveGame()
        {
            ModuleZSaveData data = new ModuleZSaveData
            {
                duelsWon = ModuleZGameState.DuelsWon,
                duelsLost = ModuleZGameState.DuelsLost,
                duelsAbandoned = ModuleZGameState.DuelsAbandoned,

                rivalMadridDefeated = ModuleZGameState.RivalMadridDefeated,
                rivalBarcelonaDefeated = ModuleZGameState.RivalBarcelonaDefeated,
                rivalValenciaDefeated = ModuleZGameState.RivalValenciaDefeated,
                rivalAndaluciaDefeated = ModuleZGameState.RivalAndaluciaDefeated,

                andaluciaUnlocked = ModuleZGameState.AndaluciaUnlocked,

                currentOpenWorldTheme = ModuleZGameState.CurrentOpenWorldTheme.ToString(),
            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);

            Debug.Log("[Module Z] Partida guardada: " + SavePath);
        }

        public static void LoadGame()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log("[Module Z] No hay partida guardada.");
                return;
            }

            string json = File.ReadAllText(SavePath);
            ModuleZSaveData data = JsonUtility.FromJson<ModuleZSaveData>(json);

            ModuleZGameState.DuelsWon = data.duelsWon;
            ModuleZGameState.DuelsLost = data.duelsLost;
            ModuleZGameState.DuelsAbandoned = data.duelsAbandoned;

            ModuleZGameState.RivalMadridDefeated = data.rivalMadridDefeated;
            ModuleZGameState.RivalBarcelonaDefeated = data.rivalBarcelonaDefeated;
            ModuleZGameState.RivalValenciaDefeated = data.rivalValenciaDefeated;
            ModuleZGameState.RivalAndaluciaDefeated = data.rivalAndaluciaDefeated;

            ModuleZGameState.AndaluciaUnlocked = data.andaluciaUnlocked;

            if (System.Enum.TryParse(data.currentOpenWorldTheme, out OpenWorldThemeId theme))
                ModuleZGameState.CurrentOpenWorldTheme = theme;

            Debug.Log("[Module Z] Partida cargada.");
        }

        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("[Module Z] Partida eliminada.");
            }
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