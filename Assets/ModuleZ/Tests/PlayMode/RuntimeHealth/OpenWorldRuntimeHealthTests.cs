using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using ModuleZ.Tests.PlayMode.Support;

namespace ModuleZ.Tests.PlayMode.RuntimeHealth
{
    internal sealed class OpenWorldRuntimeHealthTests : RuntimeHealthTestBase
    {
        [UnityTest]
        public IEnumerator RH_OW_001_002_003_SceneLoadsWithOneReadyR3Root()
        {
            yield return LoadOpenWorldReady();

            Scene scene = SceneManager.GetSceneByPath(SceneTestDriver.OpenWorldPath);
            Assert.That(scene.IsValid() && scene.isLoaded, Is.True);
            Component[] roots = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.OpenWorldRootType
            );
            Assert.That(roots, Has.Length.EqualTo(1));
            Assert.That(RuntimeContractProbe.GetStateName(roots[0]), Is.EqualTo("Ready"));
        }

        [UnityTest]
        public IEnumerator RH_OW_004_005_006_007_OwnershipIsSingularAndRootOwned()
        {
            yield return LoadOpenWorldReady();
            RuntimeHealthAssertions.AssertOpenWorldOwnership();
        }

        [UnityTest]
        public IEnumerator RH_OW_008_OwnershipRemainsSingularAfterAdditionalFrames()
        {
            yield return LoadOpenWorldReady();
            RuntimeHealthAssertions.AssertOpenWorldOwnership();
            yield return SceneTestDriver.Stabilize(10);
            RuntimeHealthAssertions.AssertOpenWorldOwnership();
        }
    }
}
