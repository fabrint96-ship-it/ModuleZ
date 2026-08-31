using UnityEngine;

namespace ModuleZ.Core.SaveSystem
{
    public class ModuleZAutoSaveController : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            ModuleZSaveManager.SaveGame();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                ModuleZSaveManager.SaveGame();
        }
    }
}