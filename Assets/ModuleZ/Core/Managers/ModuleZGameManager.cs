using UnityEngine;
using ModuleZ.Core.Settings;
using ModuleZ.Core.Managers;

namespace ModuleZ.Core.Managers
{
    public class ModuleZGameManager : MonoBehaviour
    {
        public static ModuleZGameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadSettingsOnBoot();

            Debug.Log("[Module Z] GameManager iniciado correctamente.");
        }

        private void LoadSettingsOnBoot()
        {
            ModuleZSettingsManager.LoadSettings();
        }
    }
}