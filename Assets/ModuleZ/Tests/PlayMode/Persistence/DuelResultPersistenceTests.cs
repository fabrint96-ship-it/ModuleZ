using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ModuleZ.Tests.PlayMode.Support;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace ModuleZ.Tests.PlayMode.Persistence
{
    [Category("Persistence")]
    internal sealed class DuelResultPersistenceTests
    {
        private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
        private const string PathsName = "ModuleZ.Core.SaveSystem.ModuleZPersistencePaths";
        private const string GameName = "ModuleZ.Core.Managers.ModuleZGameState";
        private const string SessionName = "ModuleZ.Core.Managers.ModuleZDuelSessionState";
        private const string RivalName = "ModuleZ.OpenWorld.Encounters.ModuleZRivalId";
        private const string ContextName = "ModuleZ.Game.DuelTransition.DuelContext";
        private const string ResultManagerName = "ModuleZ.Duel3D.Rules.Duel3DResultManager";
        private const string SaveManagerName = "ModuleZ.Core.SaveSystem.ModuleZSaveManager";
        private string root;
        private GameObject managerObject;
        private object transitionAttempt;
        private Dictionary<FieldInfo, object> gameBefore;
        private object[] sessionBefore;
        private readonly Dictionary<object, object[]> achievementsBefore =
            new Dictionary<object, object[]>();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            transitionAttempt = null;
            Type paths = RuntimeContractProbe.RequireType(PathsName);
            string processRoot = (string)RuntimeContractProbe.GetPublicStaticMember(paths, "RootPath");
            Assert.That(Environment.GetEnvironmentVariable("MODULEZ_VALIDATION_ISOLATED"),
                Is.EqualTo("1"), "Run through the isolated validation wrapper.");
            Assert.That(processRoot, Is.Not.EqualTo(Application.persistentDataPath));
            Assert.That(Path.GetFullPath(processRoot),
                Is.EqualTo(Path.GetFullPath(Environment.GetEnvironmentVariable(
                    "MODULEZ_PERSISTENCE_ROOT"))));
            root = Path.Combine(processRoot, "duel-result-" + Guid.NewGuid().ToString("N"));
            Assert.That(Directory.Exists(root), Is.False);
            RuntimeContractProbe.InvokePublicStatic(paths, "SetRootOverride", root);

            Type game = RuntimeContractProbe.RequireType(GameName);
            gameBefore = game.GetFields(PublicStatic).ToDictionary(field => field,
                field => field.GetValue(null));
            Type session = RuntimeContractProbe.RequireType(SessionName);
            sessionBefore = new[] {
                RuntimeContractProbe.GetPublicStaticMember(session, "HasActiveDuel"),
                RuntimeContractProbe.GetPublicStaticMember(session, "RivalId"),
                RuntimeContractProbe.GetPublicStaticMember(session, "IsRematch"),
                RuntimeContractProbe.GetPublicStaticMember(session, "ReturnPosition")
            };
            Type achievements = RuntimeContractProbe.RequireType(
                "ModuleZ.Core.Achievements.ModuleZAchievementManager");
            foreach (object state in (IEnumerable)RuntimeContractProbe.InvokePublicStatic(
                achievements, "GetAll"))
                achievementsBefore.Add(state, new[] {
                    state.GetType().GetField("unlocked").GetValue(state),
                    state.GetType().GetField("unlockedDate").GetValue(state)
                });
            RuntimeContractProbe.InvokePublicStatic(session, "Clear");
            RuntimeContractProbe.InvokePublicStatic(game, "ResetGameState");
            MethodInfo freshStart = RuntimeContractProbe.RequireType(SaveManagerName)
                .GetMethod("ConfirmFreshStart", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(freshStart?.Invoke(null, null), Is.EqualTo(true));
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                RuntimeContractProbe.RequireType(ResultManagerName), "Instance"), Is.Null);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            try
            {
                if (transitionAttempt != null)
                {
                    Type bridge = RuntimeContractProbe.RequireType(
                        "ModuleZ.Game.DuelTransition.DuelTransitionBridge");
                    RuntimeContractProbe.InvokePublicStatic(bridge, "Cancel",
                        transitionAttempt);
                }
                yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
                if (transitionAttempt != null)
                {
                    float deadline = Time.realtimeSinceStartup + 22f;
                    while ((bool)RuntimeContractProbe.GetPublicInstanceProperty(
                            transitionAttempt, "IsLoadRequested") &&
                           Time.realtimeSinceStartup < deadline)
                        yield return null;
                    Assert.That(RuntimeContractProbe.GetPublicInstanceProperty(
                        transitionAttempt, "IsLoadRequested"), Is.False,
                        "The in-flight transition must terminate before test teardown.");
                }
                if (managerObject != null)
                {
                    managerObject.GetComponent<MonoBehaviour>()?.CancelInvoke();
                    UnityEngine.Object.Destroy(managerObject);
                }
                yield return null;
                Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                    RuntimeContractProbe.RequireType(ResultManagerName), "Instance"), Is.Null);
            }
            finally
            {
                try
                {
                    // Clear the test's isolated publication gate even when an
                    // assertion above fails, before restoring prior state.
                    Type saveManager = RuntimeContractProbe.RequireType(SaveManagerName);
                    Assert.That(RuntimeContractProbe.InvokePublicStatic(
                        saveManager, "TryDeleteSave"), Is.EqualTo(true));
                    RuntimeContractProbe.InvokePublicStatic(
                        RuntimeContractProbe.RequireType(SessionName), "Clear");
                    MethodInfo freshStart = saveManager.GetMethod("ConfirmFreshStart",
                        BindingFlags.NonPublic | BindingFlags.Static);
                    Assert.That(freshStart?.Invoke(null, null), Is.EqualTo(true));
                }
                finally
                {
                    try
                    {
                        if (gameBefore != null)
                            foreach (var entry in gameBefore)
                                entry.Key.SetValue(null, entry.Value);
                        if (sessionBefore != null)
                        {
                            Type session = RuntimeContractProbe.RequireType(SessionName);
                            if ((bool)sessionBefore[0])
                                RuntimeContractProbe.InvokePublicStatic(session, "StartDuel",
                                    sessionBefore[1], sessionBefore[2], sessionBefore[3]);
                            else
                                RuntimeContractProbe.InvokePublicStatic(session, "Clear");
                        }
                        foreach (var entry in achievementsBefore)
                        {
                            entry.Key.GetType().GetField("unlocked").SetValue(entry.Key,
                                entry.Value[0]);
                            entry.Key.GetType().GetField("unlockedDate").SetValue(entry.Key,
                                entry.Value[1]);
                        }
                    }
                    finally
                    {
                        RuntimeContractProbe.InvokePublicStatic(
                            RuntimeContractProbe.RequireType(PathsName), "ClearRootOverride");
                        if (!string.IsNullOrEmpty(root) && Directory.Exists(root))
                        {
                            string processRoot = Path.GetFullPath(
                                Environment.GetEnvironmentVariable("MODULEZ_PERSISTENCE_ROOT"))
                                .TrimEnd(Path.DirectorySeparatorChar);
                            Assert.That(Path.GetFullPath(root).StartsWith(
                                processRoot + Path.DirectorySeparatorChar,
                                StringComparison.OrdinalIgnoreCase), Is.True);
                            Directory.Delete(root, true);
                        }
                    }
                }
            }
        }

        [UnityTest]
        public IEnumerator LIFE_006_007_ResultSaveFailureRetriesWithoutReapplyingAbandon()
        {
            Type game = RuntimeContractProbe.RequireType(GameName);
            Type session = RuntimeContractProbe.RequireType(SessionName);
            Type rival = RuntimeContractProbe.RequireType(RivalName);
            Type context = RuntimeContractProbe.RequireType(ContextName);
            object madrid = Enum.ToObject(rival, 0);
            RuntimeContractProbe.InvokePublicStatic(session, "StartDuel", madrid,
                false, Vector3.one);
            object accepted = Activator.CreateInstance(context, madrid, false, Vector3.one);
            int before = (int)game.GetField("DuelsAbandoned", PublicStatic).GetValue(null);

            Directory.CreateDirectory(root);
            string pending = Path.Combine(root, "modulez_save.json.pending");
            File.WriteAllText(pending, "unconfirmed test temporary");
            managerObject = new GameObject("Persist02_ResultManagerTest");
            Component manager = (Component)managerObject.AddComponent(
                RuntimeContractProbe.RequireType(ResultManagerName));
            Assert.That(RuntimeContractProbe.InvokePublicInstance(manager, "Initialize", accepted),
                Is.EqualTo(true));

            LogAssert.Expect(LogType.Warning,
                new System.Text.RegularExpressions.Regex("Resultado aplicado, no guardado"));
            RuntimeContractProbe.InvokePublicInstance(manager, "AbandonDuel");
            Assert.That((int)game.GetField("DuelsAbandoned", PublicStatic).GetValue(null),
                Is.EqualTo(before + 1));
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(session, "HasActiveDuel"),
                Is.True);
            Assert.That(File.Exists(Path.Combine(root, "modulez_save.json")), Is.False);
            Assert.That(GameObject.Find("DuelResult_SaveRetry"), Is.Not.Null);

            File.Delete(pending);
            Button retryButton = GameObject.Find("RetrySaveButton")?.GetComponent<Button>();
            Assert.That(retryButton, Is.Not.Null, "Player retry action must be reachable.");
            retryButton.onClick.Invoke();
            Assert.That(File.Exists(Path.Combine(root, "modulez_save.json")), Is.True);
            Assert.That((int)game.GetField("DuelsAbandoned", PublicStatic).GetValue(null),
                Is.EqualTo(before + 1), "Retry must never reapply the result.");
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(session, "HasActiveDuel"),
                Is.False);
            Assert.That(RuntimeContractProbe.InvokePublicInstance(manager, "RetryPendingSave"),
                Is.EqualTo(false));
            RuntimeContractProbe.InvokePublicStatic(session, "StartDuel", madrid,
                false, Vector3.one);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(SaveManagerName), "TrySaveGame").ToString(),
                Is.EqualTo("LifecycleRejected"),
                "The previous result authorization must not authorize a new session.");
            yield break;
        }

        [UnityTest]
        public IEnumerator LIFE_005_ReplacedSessionCannotApplyAcceptedResult()
        {
            Type game = RuntimeContractProbe.RequireType(GameName);
            Type session = RuntimeContractProbe.RequireType(SessionName);
            Type rival = RuntimeContractProbe.RequireType(RivalName);
            Type context = RuntimeContractProbe.RequireType(ContextName);
            object madrid = Enum.ToObject(rival, 0);
            RuntimeContractProbe.InvokePublicStatic(session, "StartDuel",
                madrid, false, Vector3.one);
            object accepted = Activator.CreateInstance(context, madrid, false, Vector3.one);
            managerObject = new GameObject("Persist02_ObsoleteResultManager");
            Component manager = (Component)managerObject.AddComponent(
                RuntimeContractProbe.RequireType(ResultManagerName));
            Assert.That(RuntimeContractProbe.InvokePublicInstance(manager, "Initialize", accepted),
                Is.EqualTo(true));
            RuntimeContractProbe.InvokePublicStatic(session, "StartDuel",
                madrid, false, Vector3.one);
            int before = (int)game.GetField("DuelsAbandoned", PublicStatic).GetValue(null);
            LogAssert.Expect(LogType.Error,
                "[ModuleZ] Duel result belongs to an obsolete session.");
            RuntimeContractProbe.InvokePublicInstance(manager, "AbandonDuel");
            Assert.That((int)game.GetField("DuelsAbandoned", PublicStatic).GetValue(null),
                Is.EqualTo(before));
            Assert.That(File.Exists(Path.Combine(root, "modulez_save.json")), Is.False);
            yield break;
        }

        [UnityTest]
        public IEnumerator LIFE_005_DirectDuelTrialRejectsResultAndExitsWithoutSaving()
        {
            Type game = RuntimeContractProbe.RequireType(GameName);
            int abandonedBefore = (int)game.GetField("DuelsAbandoned", PublicStatic)
                .GetValue(null);
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                RuntimeContractProbe.RequireType(
                    "ModuleZ.Core.SceneLoading.ModuleZSceneController"), "Instance"),
                Is.Null, "A pre-existing SceneController would contaminate this test.");
            managerObject = new GameObject("Persist02_TrialSceneController");
            managerObject.AddComponent(RuntimeContractProbe.RequireType(
                "ModuleZ.Core.SceneLoading.ModuleZSceneController"));

            LogAssert.Expect(LogType.Warning,
                "[ModuleZ] DuelBootstrap using transitional direct-scene fallback.");
            yield return SceneTestDriver.LoadSingle(SceneTestDriver.DuelPath);
            yield return SceneTestDriver.WaitForReady(
                "ModuleZ.Duel3D.Runtime.DuelSceneRoot",
                () => RuntimeHealthAssertions.DuelDiagnostics());

            object manager = RuntimeContractProbe.GetPublicStaticMember(
                RuntimeContractProbe.RequireType(ResultManagerName), "Instance");
            Assert.That(manager, Is.Not.Null);
            RuntimeContractProbe.InvokePublicInstance(manager, "AbandonDuel");
            Assert.That((int)game.GetField("DuelsAbandoned", PublicStatic).GetValue(null),
                Is.EqualTo(abandonedBefore));
            Assert.That(File.Exists(Path.Combine(root, "modulez_save.json")), Is.False);
            Assert.That(GameObject.Find("RetrySaveButton"), Is.Null);
            Button exit = GameObject.Find("ExitTrialModeButton")?.GetComponent<Button>();
            Assert.That(exit, Is.Not.Null);
            exit.onClick.Invoke();
            yield return SceneTestDriver.WaitForActiveScene(
                SceneTestDriver.BootPath,
                () => "Trial exit did not reach Boot/MainMenu.");
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo("Boot"));
            Assert.That(File.Exists(Path.Combine(root, "modulez_save.json")), Is.False);
        }

        [UnityTest]
        public IEnumerator LIFE_003_DuelLoadRequestConfirmsOnlyAtMatchingSceneRootReady()
        {
            yield return PrepareOpenWorldForTransition();
            object token = BeginNormalTransition();
            Assert.That(RuntimeContractProbe.GetPublicInstanceProperty(
                token, "IsReadyConfirmed"), Is.False);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(SaveManagerName), "TrySaveGame").ToString(),
                Is.EqualTo("LifecycleRejected"));

            RequestDuel(token);
            yield return SceneTestDriver.WaitForActiveScene(
                SceneTestDriver.DuelPath,
                () => "Requested Duel scene did not become active.");
            yield return SceneTestDriver.WaitForReady(
                "ModuleZ.Duel3D.Runtime.DuelSceneRoot",
                () => RuntimeHealthAssertions.DuelDiagnostics());
            Assert.That(RuntimeContractProbe.GetPublicInstanceProperty(
                token, "IsReadyConfirmed"), Is.True);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(SaveManagerName), "TrySaveGame").ToString(),
                Is.EqualTo("LifecycleRejected"),
                "Ready does not authorize an unresolved Duel result.");

            RuntimeContractProbe.InvokePublicInstance(managerObject.GetComponent<MonoBehaviour>(),
                "ReturnToMainMenu");
            yield return SceneTestDriver.WaitForActiveScene(SceneTestDriver.BootPath,
                () => "Duel test cleanup did not return to Boot.");
        }

        [UnityTest]
        public IEnumerator LIFE_003_FailedSceneRootCannotReleasePublicationGate()
        {
            yield return PrepareOpenWorldForTransition();
            object token = BeginNormalTransition();
            Component disabledRoot = null;
            void DisableRoot(Scene scene, LoadSceneMode mode)
            {
                if (scene.name != "Duel") return;
                disabledRoot = RuntimeContractProbe.FindSceneComponents(
                    "ModuleZ.Duel3D.Runtime.DuelSceneRoot").Single();
                ((MonoBehaviour)disabledRoot).enabled = false;
            }
            SceneManager.sceneLoaded += DisableRoot;
            try { RequestDuel(token); }
            finally { SceneManager.sceneLoaded -= DisableRoot; }
            yield return SceneTestDriver.WaitForActiveScene(SceneTestDriver.DuelPath,
                () => "Duel did not load for failure injection.");
            Assert.That(disabledRoot, Is.Not.Null);
            LogAssert.Expect(LogType.Error,
                "[ModuleZ] Duel scene did not confirm Ready; save remains blocked.");
            LogAssert.Expect(LogType.Error,
                "[ModuleZ] DuelSceneRoot failed: DuelBootstrap is required to initialize the Duel scene.");
            Assert.That(RuntimeContractProbe.InvokePublicInstance(disabledRoot,
                "Initialize", new object[] { null }), Is.EqualTo(false));
            Assert.That(RuntimeContractProbe.GetStateName(disabledRoot), Is.EqualTo("Failed"));
            Assert.That(RuntimeContractProbe.GetPublicInstanceProperty(token,
                "IsReadyConfirmed"), Is.False);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(SaveManagerName), "TrySaveGame").ToString(),
                Is.EqualTo("LifecycleRejected"));
        }

        [UnityTest]
        public IEnumerator LIFE_003_TimeoutAndLateReadyCannotConfirmObsoleteAttempt()
        {
            yield return PrepareOpenWorldForTransition();
            object token = BeginNormalTransition();
            Component disabledRoot = null;
            void DisableRoot(Scene scene, LoadSceneMode mode)
            {
                if (scene.name != "Duel") return;
                disabledRoot = RuntimeContractProbe.FindSceneComponents(
                    "ModuleZ.Duel3D.Runtime.DuelSceneRoot").Single();
                ((MonoBehaviour)disabledRoot).enabled = false;
            }
            SceneManager.sceneLoaded += DisableRoot;
            try { RequestDuel(token); }
            finally { SceneManager.sceneLoaded -= DisableRoot; }
            yield return SceneTestDriver.WaitForActiveScene(SceneTestDriver.DuelPath,
                () => "Duel did not load for timeout injection.");
            Assert.That(disabledRoot, Is.Not.Null);
            LogAssert.Expect(LogType.Error,
                "[ModuleZ] Duel scene did not confirm Ready; save remains blocked.");
            float deadline = Time.realtimeSinceStartup + 22f;
            while ((bool)RuntimeContractProbe.GetPublicInstanceProperty(
                    token, "IsLoadRequested") && Time.realtimeSinceStartup < deadline)
                yield return null;
            Assert.That(RuntimeContractProbe.GetPublicInstanceProperty(token,
                "IsLoadRequested"), Is.False, "Readiness timeout did not expire.");
            Assert.That(RuntimeContractProbe.GetPublicInstanceProperty(token,
                "IsReadyConfirmed"), Is.False);
            LogAssert.Expect(LogType.Warning,
                "[ModuleZ] DuelBootstrap using transitional direct-scene fallback.");
            ((MonoBehaviour)disabledRoot).enabled = true;
            yield return SceneTestDriver.WaitForReady(
                "ModuleZ.Duel3D.Runtime.DuelSceneRoot",
                () => RuntimeHealthAssertions.DuelDiagnostics());
            Assert.That(RuntimeContractProbe.GetPublicInstanceProperty(token,
                "IsReadyConfirmed"), Is.False,
                "A late Ready callback cannot confirm an expired attempt.");
            Assert.That(RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(SaveManagerName), "TrySaveGame").ToString(),
                Is.EqualTo("LifecycleRejected"));
        }

        [UnityTest]
        public IEnumerator LIFE_003_DestroyedRootCannotConfirmTransition()
        {
            yield return PrepareOpenWorldForTransition();
            object token = BeginNormalTransition();
            Component disabledRoot = null;
            void DisableRoot(Scene scene, LoadSceneMode mode)
            {
                if (scene.name != "Duel") return;
                disabledRoot = RuntimeContractProbe.FindSceneComponents(
                    "ModuleZ.Duel3D.Runtime.DuelSceneRoot").Single();
                ((MonoBehaviour)disabledRoot).enabled = false;
            }
            SceneManager.sceneLoaded += DisableRoot;
            try { RequestDuel(token); }
            finally { SceneManager.sceneLoaded -= DisableRoot; }
            yield return SceneTestDriver.WaitForActiveScene(SceneTestDriver.DuelPath,
                () => "Duel did not load for root-destruction injection.");
            Assert.That(disabledRoot, Is.Not.Null);
            LogAssert.Expect(LogType.Error,
                "[ModuleZ] Duel scene did not confirm Ready; save remains blocked.");
            UnityEngine.Object.Destroy(disabledRoot.gameObject);
            yield return null;
            Assert.That(RuntimeContractProbe.GetPublicInstanceProperty(token,
                "IsReadyConfirmed"), Is.False);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(SaveManagerName), "TrySaveGame").ToString(),
                Is.EqualTo("LifecycleRejected"));
        }

        [UnityTest]
        public IEnumerator LIFE_003_SceneChangeInvalidatesUnconfirmedAttempt()
        {
            yield return PrepareOpenWorldForTransition();
            object token = BeginNormalTransition();
            void DisableRoot(Scene scene, LoadSceneMode mode)
            {
                if (scene.name != "Duel") return;
                Component root = RuntimeContractProbe.FindSceneComponents(
                    "ModuleZ.Duel3D.Runtime.DuelSceneRoot").Single();
                ((MonoBehaviour)root).enabled = false;
            }
            SceneManager.sceneLoaded += DisableRoot;
            try { RequestDuel(token); }
            finally { SceneManager.sceneLoaded -= DisableRoot; }
            yield return SceneTestDriver.WaitForActiveScene(SceneTestDriver.DuelPath,
                () => "Duel did not load for scene-change injection.");
            LogAssert.Expect(LogType.Error,
                "[ModuleZ] Duel scene did not confirm Ready; save remains blocked.");
            RuntimeContractProbe.InvokePublicInstance(managerObject.GetComponent<MonoBehaviour>(),
                "ReturnToMainMenu");
            yield return SceneTestDriver.WaitForActiveScene(SceneTestDriver.BootPath,
                () => "Boot did not load after unconfirmed Duel.");
            Assert.That(RuntimeContractProbe.GetPublicInstanceProperty(token,
                "IsReadyConfirmed"), Is.False);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(SaveManagerName), "TrySaveGame").ToString(),
                Is.EqualTo("LifecycleRejected"));
        }

        private IEnumerator PrepareOpenWorldForTransition()
        {
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                RuntimeContractProbe.RequireType(
                    "ModuleZ.Core.SceneLoading.ModuleZSceneController"), "Instance"),
                Is.Null, "A pre-existing SceneController would contaminate this test.");
            managerObject = new GameObject("Persist02_TransitionSceneController");
            managerObject.AddComponent(RuntimeContractProbe.RequireType(
                "ModuleZ.Core.SceneLoading.ModuleZSceneController"));
            yield return SceneTestDriver.LoadSingle(SceneTestDriver.OpenWorldPath);
            yield return SceneTestDriver.WaitForReady(
                "ModuleZ.OpenWorld.Runtime.OpenWorldSceneRoot",
                () => "OpenWorld did not become Ready before Duel transition.");
        }

        private object BeginNormalTransition()
        {
            Type bridge = RuntimeContractProbe.RequireType(
                "ModuleZ.Game.DuelTransition.DuelTransitionBridge");
            Type contextType = RuntimeContractProbe.RequireType(ContextName);
            Type attemptType = RuntimeContractProbe.RequireType(
                "ModuleZ.Game.DuelTransition.DuelTransitionBridge+Attempt");
            object rivalValue = Enum.ToObject(RuntimeContractProbe.RequireType(RivalName), 0);
            object accepted = Activator.CreateInstance(contextType, rivalValue, false,
                new Vector3(3f, 0f, 4f));
            MethodInfo begin = bridge.GetMethod("Begin", new[] {
                contextType, attemptType.MakeByRefType(), typeof(string).MakeByRefType()
            });
            Assert.That(begin, Is.Not.Null);
            object[] arguments = { accepted, null, null };
            Assert.That(begin.Invoke(null, arguments), Is.EqualTo(true),
                arguments[2]?.ToString());
            transitionAttempt = arguments[1];
            return transitionAttempt;
        }

        private static void RequestDuel(object token)
        {
            Type bridge = RuntimeContractProbe.RequireType(
                "ModuleZ.Game.DuelTransition.DuelTransitionBridge");
            object[] arguments = { token, null };
            Assert.That(bridge.GetMethod("CompleteAfterDelay", PublicStatic)
                .Invoke(null, arguments), Is.EqualTo(true), arguments[1]?.ToString());
        }
    }
}
