using UnityEngine;

namespace ModuleZ.Core.SaveSystem
{
    public class ModuleZSaveBootstrap : MonoBehaviour
    {
        private static bool loaded;

        private void Awake()
        {
            if (loaded)
                return;

            ModuleZSaveManager.LoadGame();
            loaded = true;

            DontDestroyOnLoad(gameObject);

            Debug.Log("[ModuleZ] SaveBootstrap inicializado.");
        }
    }
}