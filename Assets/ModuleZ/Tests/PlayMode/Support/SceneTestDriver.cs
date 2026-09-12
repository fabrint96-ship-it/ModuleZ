using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModuleZ.Tests.PlayMode.Support
{
    internal static class SceneTestDriver
    {
        public const string OpenWorldPath = "Assets/ModuleZ/Scenes/OpenWorld.unity";
        public const string DuelPath = "Assets/ModuleZ/Scenes/Duel.unity";
        private const float TimeoutSeconds = 20f;

        public static IEnumerator LoadSingle(string scenePath)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(
                scenePath,
                LoadSceneMode.Single
            );
            Assert.That(operation, Is.Not.Null, $"Scene load did not start: {scenePath}");

            float deadline = Time.realtimeSinceStartup + TimeoutSeconds;
            while (!operation.isDone && Time.realtimeSinceStartup < deadline)
                yield return null;

            Assert.That(operation.isDone, Is.True,
                $"Timed out loading {scenePath} after {TimeoutSeconds} seconds.");

            Scene scene = SceneManager.GetSceneByPath(scenePath);
            Assert.That(scene.IsValid(), Is.True, $"Loaded scene is invalid: {scenePath}");
            Assert.That(scene.isLoaded, Is.True, $"Scene is not loaded: {scenePath}");
            Assert.That(SceneManager.GetActiveScene(), Is.EqualTo(scene));
        }

        public static IEnumerator WaitForReady(
            string rootTypeName,
            Func<string> diagnostics)
        {
            float deadline = Time.realtimeSinceStartup + TimeoutSeconds;
            while (Time.realtimeSinceStartup < deadline)
            {
                Component[] roots = RuntimeContractProbe.FindSceneComponents(
                    rootTypeName
                );
                if (roots.Length == 1 &&
                    RuntimeContractProbe.GetStateName(roots[0]) == "Ready")
                    yield break;

                yield return null;
            }

            Assert.Fail(
                $"Timed out waiting for {rootTypeName} Ready/R3 after " +
                $"{TimeoutSeconds} seconds. {diagnostics()}"
            );
        }

        public static IEnumerator Stabilize(int frames = 5)
        {
            for (int index = 0; index < frames; index++)
                yield return null;
        }

        public static IEnumerator DestroyLoadedProductionSceneRoots()
        {
            for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (scene.path != OpenWorldPath && scene.path != DuelPath)
                    continue;

                foreach (GameObject root in scene.GetRootGameObjects())
                    UnityEngine.Object.Destroy(root);
            }

            yield return null;
            yield return null;
        }
    }
}
