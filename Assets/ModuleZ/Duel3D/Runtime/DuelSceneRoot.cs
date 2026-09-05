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

        public LifecycleState State { get; private set; } =
            LifecycleState.Uninitialized;

        public GameObject RuntimeObject { get; private set; }
        public Duel3DRuntimeBuilder RuntimeBuilder { get; private set; }

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
                    out string failureReason))
            {
                return Fail(failureReason);
            }

            RuntimeObject = runtimeObject;
            RuntimeBuilder = runtimeBuilder;
            State = LifecycleState.Ready;

            Debug.Log("[ModuleZ] DuelSceneRoot ready.");
            return true;
        }

        private bool Fail(string failureReason)
        {
            State = LifecycleState.Failed;
            Debug.LogError("[ModuleZ] DuelSceneRoot failed: " + failureReason);
            return false;
        }
    }
}
