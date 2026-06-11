using System.IO;
using ModuleZ.Core.Managers;
using UnityEngine;

namespace ModuleZ.Core.Settings
{
    public static class ModuleZSettingsManager
    {
        private static string SettingsPath =>
            Path.Combine(Application.persistentDataPath, "module_z_settings.json");

        public static void SaveSettings()
        {
            ModuleZSettingsData data = new ModuleZSettingsData
            {
                audioEnabled = ModuleZGameState.AudioEnabled,
                fullscreenEnabled = ModuleZGameState.FullscreenEnabled
            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SettingsPath, json);

            Debug.Log("[Module Z] Opciones guardadas.");
        }

        public static void LoadSettings()
        {
            if (!File.Exists(SettingsPath))
            {
                ApplyDefaultSettings();
                return;
            }

            string json = File.ReadAllText(SettingsPath);
            ModuleZSettingsData data = JsonUtility.FromJson<ModuleZSettingsData>(json);

            ModuleZGameState.AudioEnabled = data.audioEnabled;
            ModuleZGameState.FullscreenEnabled = data.fullscreenEnabled;

            ApplySettings();
        }

        private static void ApplyDefaultSettings()
        {
            ModuleZGameState.AudioEnabled = true;
            ModuleZGameState.FullscreenEnabled = true;

            ApplySettings();
        }

        public static void ApplySettings()
        {
            AudioListener.volume = ModuleZGameState.AudioEnabled ? 1f : 0f;
            Screen.fullScreen = ModuleZGameState.FullscreenEnabled;
        }

        public static void ResetSettings()
        {
            ModuleZGameState.AudioEnabled = true;
            ModuleZGameState.FullscreenEnabled = true;

            ApplySettings();
            SaveSettings();

            Debug.Log("[Module Z] Opciones restauradas.");
        }
    }
}