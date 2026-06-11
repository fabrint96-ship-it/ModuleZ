using ModuleZ.Core.Managers;
using ModuleZ.Core.SceneLoading;
using ModuleZ.Core.SaveSystem;
using UnityEngine;

namespace ModuleZ.OpenWorld.Runtime
{
    public class OpenWorldZonePortal : MonoBehaviour
    {
        public OpenWorldThemeId targetTheme = OpenWorldThemeId.Madrid70s;
        public Vector3 spawnPosition = new Vector3(0f, 0.1f, -4f);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<CharacterController>())
                return;

            ModuleZGameState.CurrentOpenWorldTheme = targetTheme;
            ModuleZSaveManager.SaveGame();
            ModuleZGameState.OpenWorldReturnPosition = spawnPosition;
            ModuleZGameState.ReturningFromDuel = false;

            ModuleZSceneController.Instance.LoadOpenWorld();
        }
    }
}