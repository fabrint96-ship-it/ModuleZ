using System;
using ModuleZ.Game.Camera;
using UnityEngine;

namespace ModuleZ.OpenWorld.Runtime
{
    public static class OpenWorldGameplayCameraBuilder
    {
        private static readonly Vector3 InitialOffset =
            new Vector3(0f, 3.2f, -4.5f);

        public static Camera Create(
            Transform target,
            out AudioListener audioListener,
            out ModuleZThirdPersonCamera cameraController)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";

            Camera gameplayCamera = cameraObject.AddComponent<Camera>();
            audioListener = cameraObject.AddComponent<AudioListener>();
            gameplayCamera.fieldOfView = 50f;
            gameplayCamera.clearFlags = CameraClearFlags.Skybox;

            cameraController =
                cameraObject.AddComponent<ModuleZThirdPersonCamera>();

            cameraController.SetTarget(target);

            cameraObject.transform.position =
                target.position + target.rotation * InitialOffset;

            cameraObject.transform.LookAt(
                target.position + Vector3.up * 1.6f
            );

            return gameplayCamera;
        }
    }
}
