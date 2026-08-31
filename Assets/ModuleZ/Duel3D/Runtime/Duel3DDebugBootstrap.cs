using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Duel3D.Runtime
{
    public class Duel3DDebugBootstrap : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool autoStartDuel3D = true;

        [Header("Debug Rival")]
        [SerializeField]
        private ModuleZRivalId debugRival =
            ModuleZRivalId.Madrid;

        [Header("Scene Cleanup")]
        [SerializeField] private bool destroyLegacyDuelObjects = true;

        private void Awake()
        {
            if (!autoStartDuel3D)
                return;

            PrepareGameState();

            if (destroyLegacyDuelObjects)
                CleanupLegacyDuelScene();

            CreateRuntime();
        }

        private void PrepareGameState()
        {
            ModuleZGameState.CurrentDuelRival = debugRival;

            ModuleZGameState.DuelCompleted = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.DuelWasCancelled = false;

            Debug.Log(
                $"[ModuleZ] Duel3D Debug iniciado con rival {debugRival}"
            );
        }

        private void CleanupLegacyDuelScene()
        {
            string[] namesToRemove =
            {
                "DuelRuntimeBuilder",
                "DuelArenaBuilder",
                "DuelCameraBuilder",
                "DuelOpponentBuilder",
                "DuelHUDController",
                "DuelBoard",
                "DuelArena",
                "DuelRoot"
            };

            for (int i = 0; i < namesToRemove.Length; i++)
            {
                GameObject obj =
                    GameObject.Find(namesToRemove[i]);

                if (obj != null)
                    Destroy(obj);
            }
        }

        private void CreateRuntime()
        {
            GameObject runtime =
                new GameObject("Duel3D_Runtime");

            runtime.AddComponent<Duel3DRuntimeBuilder>();

            Debug.Log(
                "[ModuleZ] Duel3D Runtime creado automáticamente."
            );
        }
    }
}