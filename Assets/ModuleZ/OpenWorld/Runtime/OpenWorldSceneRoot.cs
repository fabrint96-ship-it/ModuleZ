using ModuleZ.Game.Camera;
using UnityEngine;

namespace ModuleZ.OpenWorld.Runtime
{
    [DisallowMultipleComponent]
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

        public bool Initialize(
            GameObject player,
            Camera gameplayCamera,
            AudioListener gameplayAudioListener,
            ModuleZThirdPersonCamera cameraController)
        {
            if (State == LifecycleState.Ready)
                return true;

            if (State != LifecycleState.Uninitialized)
                return false;

            State = LifecycleState.Initializing;

            if (player == null)
                return Fail("Player ownership is required.");

            if (gameplayCamera == null)
                return Fail("Gameplay Camera ownership is required.");

            if (gameplayAudioListener == null ||
                gameplayAudioListener.gameObject != gameplayCamera.gameObject)
            {
                return Fail(
                    "The gameplay AudioListener must belong to the gameplay Camera."
                );
            }

            if (cameraController == null ||
                cameraController.gameObject != gameplayCamera.gameObject)
            {
                return Fail(
                    "The third-person controller must belong to the gameplay Camera."
                );
            }

            Player = player;
            GameplayCamera = gameplayCamera;
            GameplayAudioListener = gameplayAudioListener;
            CameraController = cameraController;
            State = LifecycleState.Ready;

            Debug.Log("[ModuleZ] OpenWorldSceneRoot ready.");
            return true;
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
