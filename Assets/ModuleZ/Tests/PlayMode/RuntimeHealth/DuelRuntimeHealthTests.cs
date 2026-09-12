using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using ModuleZ.Tests.PlayMode.Support;

namespace ModuleZ.Tests.PlayMode.RuntimeHealth
{
    internal sealed class DuelRuntimeHealthTests : RuntimeHealthTestBase
    {
        [UnityTest]
        public IEnumerator RH_DUEL_001_002_003_004_DirectFallbackBuildsOneReadyRootWithoutSession()
        {
            AssertSessionInactive();
            yield return LoadDuelReady();

            Scene scene = SceneManager.GetSceneByPath(SceneTestDriver.DuelPath);
            Assert.That(scene.IsValid() && scene.isLoaded, Is.True);
            Component[] roots = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.DuelRootType
            );
            Assert.That(roots, Has.Length.EqualTo(1), RuntimeHealthAssertions.DuelDiagnostics());
            Assert.That(RuntimeContractProbe.GetStateName(roots[0]), Is.EqualTo("Ready"));
            AssertSessionInactive();
        }

        [UnityTest]
        public IEnumerator RH_DUEL_005_006_RuntimeAndResultManagerAreSingular()
        {
            yield return LoadDuelReady();

            GameObject[] runtimeRoots = Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(gameObject =>
                    gameObject != null &&
                    gameObject.name == "Duel3D_Runtime" &&
                    gameObject.scene.IsValid() &&
                    gameObject.scene.isLoaded &&
                    gameObject.activeInHierarchy)
                .ToArray();
            Component[] managers = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.ResultManagerType
            );

            Assert.That(runtimeRoots, Has.Length.EqualTo(1), RuntimeHealthAssertions.DuelDiagnostics());
            Assert.That(managers, Has.Length.EqualTo(1), RuntimeHealthAssertions.DuelDiagnostics());
            Assert.That(managers[0].transform.IsChildOf(runtimeRoots[0].transform), Is.True);
        }

        [UnityTest]
        public IEnumerator RH_DUEL_007_ResultManagerInstanceReferencesLiveSceneInstance()
        {
            yield return LoadDuelReady();

            Component[] managers = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.ResultManagerType
            );
            Type managerType = RuntimeContractProbe.RequireType(
                RuntimeHealthAssertions.ResultManagerType
            );
            object instance = RuntimeContractProbe.GetPublicStaticMember(
                managerType,
                "Instance"
            );

            Assert.That(managers, Has.Length.EqualTo(1));
            Assert.That(instance, Is.SameAs(managers[0]));
            Assert.That(managers[0].gameObject.activeInHierarchy, Is.True);
        }

        [UnityTest]
        public IEnumerator RH_DUEL_008_ResultManagerInstanceClearsAfterSceneTeardown()
        {
            yield return LoadDuelReady();
            yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
            yield return null;

            Assert.That(
                RuntimeContractProbe.FindSceneComponents(
                    RuntimeHealthAssertions.ResultManagerType
                ),
                Is.Empty
            );
            AssertResultManagerInstanceCleared();
        }
    }
}
