using System;
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
        private bool transitioning;

        private void OnTriggerEnter(Collider other)
        {
            if (transitioning || !other.GetComponent<CharacterController>())
                return;

            if (!Enum.IsDefined(typeof(OpenWorldThemeId), targetTheme) ||
                float.IsNaN(spawnPosition.x) || float.IsInfinity(spawnPosition.x) ||
                float.IsNaN(spawnPosition.y) || float.IsInfinity(spawnPosition.y) ||
                float.IsNaN(spawnPosition.z) || float.IsInfinity(spawnPosition.z) ||
                ModuleZSceneController.Instance == null)
            {
                Debug.LogError("[ModuleZ] Destino del portal no disponible o inválido.");
                return;
            }

            transitioning = true;
            OpenWorldThemeId previousTheme = ModuleZGameState.CurrentOpenWorldTheme;
            Vector3 previousPosition = ModuleZGameState.OpenWorldReturnPosition;
            bool previousReturning = ModuleZGameState.ReturningFromDuel;

            ModuleZGameState.CurrentOpenWorldTheme = targetTheme;
            ModuleZGameState.OpenWorldReturnPosition = spawnPosition;
            ModuleZGameState.ReturningFromDuel = false;
            ModuleZSaveGameStatus status = ModuleZSaveManager.TrySaveGame();
            if (status != ModuleZSaveGameStatus.Success)
            {
                // Roll back only values still carrying this attempt's publication.
                if (ModuleZGameState.CurrentOpenWorldTheme == targetTheme)
                    ModuleZGameState.CurrentOpenWorldTheme = previousTheme;
                if (ModuleZGameState.OpenWorldReturnPosition == spawnPosition)
                    ModuleZGameState.OpenWorldReturnPosition = previousPosition;
                if (!ModuleZGameState.ReturningFromDuel)
                    ModuleZGameState.ReturningFromDuel = previousReturning;
                transitioning = false;
                Debug.LogWarning("[ModuleZ] Portal detenido: no se guardó el destino (" + status + ").");
                return;
            }

            try
            {
                ModuleZSceneController.Instance.LoadOpenWorld();
            }
            catch (Exception exception)
            {
                // The destination is already durable. Do not roll back only
                // the in-memory projection and contradict the saved state.
                transitioning = false;
                Debug.LogError("[ModuleZ] Destino guardado, pero la escena no cargó. " +
                    "Permanece aquí; vuelve a activar el portal para reintentar la carga. " +
                    exception.Message);
            }
        }
    }
}
