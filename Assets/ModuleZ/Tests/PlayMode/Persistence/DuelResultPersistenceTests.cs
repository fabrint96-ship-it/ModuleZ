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
        private Dictionary<FieldInfo, object> gameBefore;
        private object[] sessionBefore;
        private readonly Dictionary<object, object[]> achievementsBefore =
            new Dictionary<object, object[]>();

        [UnitySetUp]
        public IEnumerator SetUp()
        {
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
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                RuntimeContractProbe.RequireType(ResultManagerName), "Instance"), Is.Null);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            try
            {
                if (managerObject != null)
                {
                    managerObject.GetComponent<MonoBehaviour>()?.CancelInvoke();
                    UnityEngine.Object.Destroy(managerObject);
                }
                yield return null;
                Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                    RuntimeContractProbe.RequireType(ResultManagerName), "Instance"), Is.Null);
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
                    entry.Key.GetType().GetField("unlocked").SetValue(entry.Key, entry.Value[0]);
                    entry.Key.GetType().GetField("unlockedDate").SetValue(entry.Key, entry.Value[1]);
                }
            }
            finally
            {
                RuntimeContractProbe.InvokePublicStatic(
                    RuntimeContractProbe.RequireType(PathsName), "ClearRootOverride");
                if (!string.IsNullOrEmpty(root) && Directory.Exists(root))
                {
                    string processRoot = Path.GetFullPath(Environment.GetEnvironmentVariable(
                        "MODULEZ_PERSISTENCE_ROOT")).TrimEnd(Path.DirectorySeparatorChar);
                    Assert.That(Path.GetFullPath(root).StartsWith(
                        processRoot + Path.DirectorySeparatorChar,
                        StringComparison.OrdinalIgnoreCase), Is.True);
                    Directory.Delete(root, true);
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
    }
}
