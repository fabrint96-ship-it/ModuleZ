using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace ModuleZ.Tests.PlayMode.Support
{
    internal static class RuntimeHealthAssertions
    {
        public const string OpenWorldRootType =
            "ModuleZ.OpenWorld.Runtime.OpenWorldSceneRoot";
        public const string ThirdPersonCameraType =
            "ModuleZ.Game.Camera.ModuleZThirdPersonCamera";
        public const string DuelRootType =
            "ModuleZ.Duel3D.Runtime.DuelSceneRoot";
        public const string ResultManagerType =
            "ModuleZ.Duel3D.Rules.Duel3DResultManager";

        public static string OpenWorldDiagnostics()
        {
            Component[] roots = RuntimeContractProbe.FindSceneComponents(
                OpenWorldRootType
            );
            string states = string.Join(",", roots.Select(
                RuntimeContractProbe.GetStateName
            ));
            return $"scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().path}; " +
                $"roots={roots.Length}; states=[{states}]; " +
                $"activeCameras={ActiveCameras().Length}; " +
                $"mainCameraObjects={ActiveMainCameraObjects().Length}; " +
                $"activeListeners={ActiveListeners().Length}.";
        }

        public static string DuelDiagnostics()
        {
            Component[] roots = RuntimeContractProbe.FindSceneComponents(DuelRootType);
            string states = string.Join(",", roots.Select(
                RuntimeContractProbe.GetStateName
            ));
            int runtimeRoots = Resources.FindObjectsOfTypeAll<GameObject>()
                .Count(gameObject =>
                    gameObject != null &&
                    gameObject.name == "Duel3D_Runtime" &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded);
            return $"scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().path}; " +
                $"roots={roots.Length}; states=[{states}]; " +
                $"runtimeRoots={runtimeRoots}; " +
                $"resultManagers={RuntimeContractProbe.FindSceneComponents(ResultManagerType).Length}.";
        }

        public static Camera[] ActiveCameras()
        {
            return Object.FindObjectsByType<Camera>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None
                )
                .Where(camera => camera.enabled && camera.gameObject.activeInHierarchy)
                .ToArray();
        }

        public static GameObject[] ActiveMainCameraObjects()
        {
            return GameObject.FindGameObjectsWithTag("MainCamera")
                .Where(gameObject => gameObject.activeInHierarchy)
                .ToArray();
        }

        public static AudioListener[] ActiveListeners()
        {
            return Object.FindObjectsByType<AudioListener>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None
                )
                .Where(listener => listener.enabled && listener.gameObject.activeInHierarchy)
                .ToArray();
        }

        public static void AssertOpenWorldOwnership()
        {
            Component[] roots = RuntimeContractProbe.FindSceneComponents(
                OpenWorldRootType
            );
            Assert.That(roots, Has.Length.EqualTo(1), OpenWorldDiagnostics());
            Assert.That(RuntimeContractProbe.GetStateName(roots[0]), Is.EqualTo("Ready"));

            Camera[] cameras = ActiveCameras();
            GameObject[] mainCameraObjects = ActiveMainCameraObjects();
            AudioListener[] listeners = ActiveListeners();
            Component[] controllers = RuntimeContractProbe.FindSceneComponents(
                ThirdPersonCameraType
            );

            Assert.That(cameras, Has.Length.EqualTo(1), OpenWorldDiagnostics());
            Assert.That(mainCameraObjects, Has.Length.EqualTo(1), OpenWorldDiagnostics());
            Assert.That(mainCameraObjects[0].GetComponent<Camera>(), Is.SameAs(cameras[0]));
            Assert.That(cameras[0].enabled, Is.True);
            Assert.That(listeners, Has.Length.EqualTo(1), OpenWorldDiagnostics());
            Assert.That(listeners[0].gameObject, Is.SameAs(cameras[0].gameObject));
            Assert.That(controllers, Has.Length.EqualTo(1), OpenWorldDiagnostics());
            Assert.That(controllers[0].gameObject, Is.SameAs(cameras[0].gameObject));
            Assert.That(
                RuntimeContractProbe.GetPublicInstanceProperty(roots[0], "GameplayCamera"),
                Is.SameAs(cameras[0])
            );
            Assert.That(
                RuntimeContractProbe.GetPublicInstanceProperty(roots[0], "GameplayAudioListener"),
                Is.SameAs(listeners[0])
            );
            Assert.That(
                RuntimeContractProbe.GetPublicInstanceProperty(roots[0], "CameraController"),
                Is.SameAs(controllers[0])
            );
        }
    }
}
