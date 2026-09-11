using ModuleZ.Game.Camera;
using UnityEngine;

namespace ModuleZ.OpenWorld.Runtime
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(OpenWorldRuntimeBuilder))]
    public sealed class OpenWorldSceneRoot : MonoBehaviour
    {
        public enum LifecycleState
        {
            Uninitialized,
            Initializing,
            Ready,
            Failed
        }

        public LifecycleState State { get; private set; } =
            LifecycleState.Uninitialized;

        public GameObject Player { get; private set; }
        public Camera GameplayCamera { get; private set; }
        public AudioListener GameplayAudioListener { get; private set; }
        public ModuleZThirdPersonCamera CameraController { get; private set; }

        private void Start()
        {
            Initialize();
        }

        public bool Initialize()
        {
            if (State == LifecycleState.Ready)
                return true;

            if (State != LifecycleState.Uninitialized)
                return false;

            State = LifecycleState.Initializing;

            OpenWorldRuntimeBuilder runtimeBuilder =
                GetComponent<OpenWorldRuntimeBuilder>();

            if (runtimeBuilder == null)
                return Fail("OpenWorldRuntimeBuilder is required.");

            if (!runtimeBuilder.TryBuild(
                    out GameObject player,
                    out Camera gameplayCamera,
                    out AudioListener gameplayAudioListener,
                    out ModuleZThirdPersonCamera cameraController,
                    out string failureReason))
            {
                return Fail(failureReason);
            }

            if (player == null)
                return FailAndCleanUp(
                    runtimeBuilder,
                    "Player ownership is required."
                );

            if (gameplayCamera == null)
                return FailAndCleanUp(
                    runtimeBuilder,
                    "Gameplay Camera ownership is required."
                );

            if (gameplayAudioListener == null ||
                gameplayAudioListener.gameObject != gameplayCamera.gameObject)
            {
                return FailAndCleanUp(
                    runtimeBuilder,
                    "The gameplay AudioListener must belong to the gameplay Camera."
                );
            }

            if (cameraController == null ||
                cameraController.gameObject != gameplayCamera.gameObject)
            {
                return FailAndCleanUp(
                    runtimeBuilder,
                    "The third-person controller must belong to the gameplay Camera."
                );
            }

            Player = player;
            GameplayCamera = gameplayCamera;
            GameplayAudioListener = gameplayAudioListener;
            CameraController = cameraController;
            State = LifecycleState.Ready;

            Debug.Log("[ModuleZ] OpenWorldSceneRoot ready.");
            runtimeBuilder.BeginPostBuild();
            return true;
        }

        private bool FailAndCleanUp(
            OpenWorldRuntimeBuilder runtimeBuilder,
            string failureReason)
        {
            runtimeBuilder.CleanUpFailedBuild();
            return Fail(failureReason);
        }

        private bool Fail(string failureReason)
        {
            State = LifecycleState.Failed;
            Debug.LogError(
                "[ModuleZ] OpenWorldSceneRoot failed: " + failureReason
            );
            return false;
        }
    }
}
