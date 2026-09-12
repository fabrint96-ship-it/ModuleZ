using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ModuleZ.Tests.PlayMode.Support;

namespace ModuleZ.Tests.PlayMode.RuntimeHealth
{
    internal sealed class RuntimeIsolationTests : RuntimeHealthTestBase
    {
        [UnityTest]
        public IEnumerator RH_ISO_001_RepeatedOpenWorldLoadHasNoOwnershipAccumulation()
        {
            yield return LoadOpenWorldReady();
            RuntimeHealthAssertions.AssertOpenWorldOwnership();
            yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
            yield return LoadOpenWorldReady();
            RuntimeHealthAssertions.AssertOpenWorldOwnership();
        }

        [UnityTest]
        public IEnumerator RH_ISO_002_RepeatedDuelLoadHasFreshRuntimeAndResultManager()
        {
            yield return LoadDuelReady();
            Component firstManager = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.ResultManagerType
            ).Single();
            int firstInstanceId = firstManager.GetInstanceID();

            yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
            AssertResultManagerInstanceCleared();
            AssertSessionInactive();

            yield return LoadDuelReady();
            Component[] managers = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.ResultManagerType
            );
            GameObject[] runtimeRoots = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(gameObject =>
                    gameObject != null &&
                    gameObject.name == "Duel3D_Runtime" &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded &&
                    gameObject.activeInHierarchy)
                .ToArray();

            Assert.That(managers, Has.Length.EqualTo(1));
            Assert.That(runtimeRoots, Has.Length.EqualTo(1));
            Assert.That(managers[0].GetInstanceID(), Is.Not.EqualTo(firstInstanceId));
            AssertSessionInactive();
        }
    }
}
