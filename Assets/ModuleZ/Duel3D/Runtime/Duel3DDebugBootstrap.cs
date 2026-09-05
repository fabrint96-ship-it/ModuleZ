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

            bool hasProductionSession =
                ModuleZDuelSessionState.HasActiveDuel;

            if (!hasProductionSession)
                PrepareDebugGameState();

            if (destroyLegacyDuelObjects && !hasProductionSession)
                CleanupLegacyDuelScene();

            CreateProductionBoundary();
        }

        private void PrepareDebugGameState()
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

        private void CreateProductionBoundary()
        {
            GameObject sceneRootObject =
                new GameObject("DuelSceneRoot");

            DuelBootstrap bootstrap =
                sceneRootObject.AddComponent<DuelBootstrap>();

            DuelSceneRoot sceneRoot =
                sceneRootObject.AddComponent<DuelSceneRoot>();

            sceneRoot.Initialize(bootstrap);
        }
    }
}
