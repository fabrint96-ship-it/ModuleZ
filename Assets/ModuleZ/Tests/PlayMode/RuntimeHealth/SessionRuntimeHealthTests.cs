using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ModuleZ.Tests.PlayMode.Support;

namespace ModuleZ.Tests.PlayMode.RuntimeHealth
{
    internal sealed class SessionRuntimeHealthTests : RuntimeHealthTestBase
    {
        [Test]
        public void RH_SESSION_001_ClearIsIdempotentAndFullyInactive()
        {
            ClearSession();
            ClearSession();
            AssertSessionInactive();
        }

        [Test]
        public void RH_SESSION_002_StartThenClearPublishesAndResetsPayload()
        {
            Type rivalType = RuntimeContractProbe.RequireType(RivalTypeName);
            object rival = Enum.GetValues(rivalType).GetValue(0);
            Vector3 returnPosition = new Vector3(7f, 1.5f, -3f);

            RuntimeContractProbe.InvokePublicStatic(
                SessionType,
                "StartDuel",
                rival,
                true,
                returnPosition
            );

            Assert.That(
                RuntimeContractProbe.GetPublicStaticMember(SessionType, "HasActiveDuel"),
                Is.True
            );
            Assert.That(
                RuntimeContractProbe.GetPublicStaticMember(SessionType, "RivalId"),
                Is.EqualTo(rival)
            );
            Assert.That(
                RuntimeContractProbe.GetPublicStaticMember(SessionType, "IsRematch"),
                Is.True
            );
            Assert.That(
                RuntimeContractProbe.GetPublicStaticMember(SessionType, "ReturnPosition"),
                Is.EqualTo(returnPosition)
            );

            ClearSession();
            AssertSessionInactive();
        }

        [UnityTest]
        public IEnumerator RH_SESSION_003_DirectFallbackRemainsInactiveThroughTeardown()
        {
            ClearSession();
            yield return LoadDuelReady();
            AssertSessionInactive();
            yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
            yield return null;
            AssertSessionInactive();
        }
    }
}
