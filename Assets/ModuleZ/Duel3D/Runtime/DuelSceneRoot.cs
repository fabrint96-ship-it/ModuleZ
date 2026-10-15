using ModuleZ.Core.Managers;
using ModuleZ.Game.DuelTransition;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Duel3D.Runtime
{
    [DisallowMultipleComponent]
    public sealed class DuelSceneRoot : MonoBehaviour
    {
        public enum LifecycleState
        {
            Uninitialized,
            Initializing,
            Ready,
            Failed
        }

        [SerializeField] private DuelBootstrap bootstrap;
        private ulong initializationRevision;
        private bool initializationSessionActive;
        private ModuleZRivalId initializationRival;
        private bool initializationRematch;
        private Vector3 initializationReturnPosition;

        public LifecycleState State { get; private set; } =
            LifecycleState.Uninitialized;

        public GameObject RuntimeObject { get; private set; }
        public Duel3DRuntimeBuilder RuntimeBuilder { get; private set; }

        private void Awake()
        {
            initializationRevision = ModuleZDuelSessionState.Revision;
            initializationSessionActive = ModuleZDuelSessionState.HasActiveDuel;
            initializationRival = ModuleZDuelSessionState.RivalId;
            initializationRematch = ModuleZDuelSessionState.IsRematch;
            initializationReturnPosition = ModuleZDuelSessionState.ReturnPosition;
        }

        private void Start()
        {
            if (State != LifecycleState.Uninitialized)
                return;

            if (bootstrap == null)
                bootstrap = GetComponent<DuelBootstrap>();

            Initialize(bootstrap);
        }

        public bool Initialize(DuelBootstrap productionBootstrap)
        {
            if (State == LifecycleState.Ready)
                return true;

            if (State != LifecycleState.Uninitialized)
                return false;

            State = LifecycleState.Initializing;

            if (productionBootstrap == null)
            {
                return Fail(
                    "DuelBootstrap is required to initialize the Duel scene."
                );
            }

            bootstrap = productionBootstrap;

            if (!bootstrap.TryCreateRuntime(
                    transform,
                    out GameObject runtimeObject,
                    out Duel3DRuntimeBuilder runtimeBuilder,
                    out DuelContext acceptedContext,
                    out ulong sessionRevision,
                    out bool fromSession,
                    out string failureReason))
            {
                return Fail(failureReason);
            }

            if (!runtimeBuilder.TryBuild(out failureReason))
            {
                CleanUpFailedRuntime(runtimeObject);
                return Fail(failureReason);
            }

            if (fromSession &&
                ModuleZDuelSessionState.Revision != sessionRevision)
            {
                CleanUpFailedRuntime(runtimeObject);
                return Fail("Duel session changed during initialization.");
            }

            RuntimeObject = runtimeObject;
            RuntimeBuilder = runtimeBuilder;
            State = LifecycleState.Ready;

            bool expectedByBridge = DuelTransitionBridge.IsAwaitingReadiness(
                gameObject.scene);
            if (!DuelTransitionBridge.ConfirmReady(this, acceptedContext,
                    sessionRevision, fromSession) && expectedByBridge)
            {
                CleanUpFailedRuntime(runtimeObject);
                RuntimeObject = null;
                RuntimeBuilder = null;
                return Fail("Duel readiness did not match transition ownership.");
            }

            Debug.Log("[ModuleZ] DuelSceneRoot ready.");
            return true;
        }

        private static void CleanUpFailedRuntime(GameObject runtimeObject)
        {
            if (runtimeObject == null)
                return;

            runtimeObject.SetActive(false);
            Destroy(runtimeObject);
        }

        private bool Fail(string failureReason)
        {
            DuelTransitionBridge.ReportSceneFailure(this);
            ClearOwnedSession();
            State = LifecycleState.Failed;
            Debug.LogError("[ModuleZ] DuelSceneRoot failed: " + failureReason);
            return false;
        }

        private void ClearOwnedSession()
        {
            if (initializationSessionActive &&
                ModuleZDuelSessionState.HasActiveDuel &&
                ModuleZDuelSessionState.Revision == initializationRevision &&
                ModuleZDuelSessionState.RivalId == initializationRival &&
                ModuleZDuelSessionState.IsRematch == initializationRematch &&
                ModuleZDuelSessionState.ReturnPosition == initializationReturnPosition)
                ModuleZDuelSessionState.Clear();
        }

        private void OnDestroy()
        {
            if (State != LifecycleState.Ready)
            {
                DuelTransitionBridge.ReportSceneFailure(this);
                ClearOwnedSession();
            }
        }
    }
}
