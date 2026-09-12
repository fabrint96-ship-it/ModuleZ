using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ModuleZ.Tests.PlayMode.Support
{
    internal abstract class RuntimeHealthTestBase
    {
        protected const string SessionTypeName =
            "ModuleZ.Core.Managers.ModuleZDuelSessionState";
        protected const string RivalTypeName =
            "ModuleZ.OpenWorld.Encounters.ModuleZRivalId";
        protected static readonly string DirectFallbackWarning =
            "[ModuleZ] DuelBootstrap using transitional direct-scene fallback.";

        [UnitySetUp]
        public IEnumerator SetUpRuntimeHealthTest()
        {
            Time.timeScale = 1f;
            ClearSession();
            yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
            AssertSessionInactive();
            AssertResultManagerInstanceCleared();
        }

        [UnityTearDown]
        public IEnumerator TearDownRuntimeHealthTest()
        {
            Time.timeScale = 1f;
            ClearSession();
            yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
            AssertSessionInactive();
            AssertResultManagerInstanceCleared();
        }

        protected static Type SessionType =>
            RuntimeContractProbe.RequireType(SessionTypeName);

        protected static void ClearSession()
        {
            RuntimeContractProbe.InvokePublicStatic(SessionType, "Clear");
        }

        protected static void AssertSessionInactive()
        {
            Assert.That(
                RuntimeContractProbe.GetPublicStaticMember(SessionType, "HasActiveDuel"),
                Is.False
            );
            object rival = RuntimeContractProbe.GetPublicStaticMember(
                SessionType,
                "RivalId"
            );
            Assert.That(Convert.ToInt64(rival), Is.Zero);
            Assert.That(
                RuntimeContractProbe.GetPublicStaticMember(SessionType, "IsRematch"),
                Is.False
            );
            Assert.That(
                RuntimeContractProbe.GetPublicStaticMember(SessionType, "ReturnPosition"),
                Is.EqualTo(Vector3.zero)
            );
        }

        protected static void AssertResultManagerInstanceCleared()
        {
            Type managerType = RuntimeContractProbe.RequireType(
                RuntimeHealthAssertions.ResultManagerType
            );
            object instance = RuntimeContractProbe.GetPublicStaticMember(
                managerType,
                "Instance"
            );
            Assert.That(instance, Is.Null);
        }

        protected static void ExpectDirectFallbackWarning()
        {
            LogAssert.Expect(LogType.Warning, DirectFallbackWarning);
        }

        protected static IEnumerator LoadOpenWorldReady()
        {
            yield return SceneTestDriver.LoadSingle(SceneTestDriver.OpenWorldPath);
            yield return SceneTestDriver.WaitForReady(
                RuntimeHealthAssertions.OpenWorldRootType,
                RuntimeHealthAssertions.OpenWorldDiagnostics
            );
        }

        protected static IEnumerator LoadDuelReady()
        {
            ExpectDirectFallbackWarning();
            yield return SceneTestDriver.LoadSingle(SceneTestDriver.DuelPath);
            yield return SceneTestDriver.WaitForReady(
                RuntimeHealthAssertions.DuelRootType,
                RuntimeHealthAssertions.DuelDiagnostics
            );
        }
    }
}
