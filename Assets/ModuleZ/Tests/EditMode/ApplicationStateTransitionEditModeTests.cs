using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ModuleZ.Tests.EditMode
{
    public sealed class ApplicationStateTransitionEditModeTests
    {
        private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;
        private Type bridge;
        private Type context;
        private Type attempt;
        private Type rival;
        private Type session;
        private Type gameState;
        private Type saveManager;
        private Type paths;
        private Dictionary<FieldInfo, object> priorGame;
        private Dictionary<FieldInfo, object> priorSession;
        private string originalRoot;
        private string isolatedRoot;
        private readonly List<object> attempts = new List<object>();
        private Dictionary<object, object[]> priorAchievements;
        private Type achievementManager;
        private FieldInfo unlockHandler;
        private object priorUnlockHandler;

        [SetUp]
        public void SetUp()
        {
            attempts.Clear();
            bridge = Require("ModuleZ.Game.DuelTransition.DuelTransitionBridge");
            context = Require("ModuleZ.Game.DuelTransition.DuelContext");
            attempt = Require("ModuleZ.Game.DuelTransition.DuelTransitionBridge+Attempt");
            rival = Require("ModuleZ.OpenWorld.Encounters.ModuleZRivalId");
            session = Require("ModuleZ.Core.Managers.ModuleZDuelSessionState");
            gameState = Require("ModuleZ.Core.Managers.ModuleZGameState");
            saveManager = Require("ModuleZ.Core.SaveSystem.ModuleZSaveManager");
            paths = Require("ModuleZ.Core.SaveSystem.ModuleZPersistencePaths");
            priorGame = Snapshot(gameState);
            priorSession = Snapshot(session);
            priorAchievements = new Dictionary<object, object[]>();
            achievementManager = Require("ModuleZ.Core.Achievements.ModuleZAchievementManager");
            unlockHandler = achievementManager.GetField("OnAchievementUnlocked", PublicStatic);
            priorUnlockHandler = unlockHandler.GetValue(null);
            foreach (object state in (IEnumerable)achievementManager.GetMethod("GetAll").Invoke(null, null))
            {
                Type stateType = state.GetType();
                priorAchievements.Add(state, new[] {
                    stateType.GetField("unlocked").GetValue(state),
                    stateType.GetField("unlockedDate").GetValue(state)
                });
            }
            originalRoot = (string)paths.GetProperty("RootPath").GetValue(null);
            isolatedRoot = Path.Combine(Path.GetTempPath(),
                "ModuleZ-EP02-02-" + Guid.NewGuid().ToString("N"));
            AssertSafeIsolatedRoot();
            paths.GetMethod("SetRootOverride").Invoke(null, new object[] { isolatedRoot });
            session.GetMethod("Clear").Invoke(null, null);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                foreach (object token in attempts)
                    Cancel(token);
                attempts.Clear();
                Restore(priorGame);
                RestoreSession(priorSession);
                foreach (var entry in priorAchievements)
                {
                    Type stateType = entry.Key.GetType();
                    stateType.GetField("unlocked").SetValue(entry.Key, entry.Value[0]);
                    stateType.GetField("unlockedDate").SetValue(entry.Key, entry.Value[1]);
                }
                achievementManager.GetMethod("LoadStates").Invoke(null,
                    new object[] { AchievementList(priorAchievements.Keys) });
                unlockHandler.SetValue(null, priorUnlockHandler);
            }
            finally
            {
                paths.GetMethod("ClearRootOverride").Invoke(null, null);
                Assert.That((string)paths.GetProperty("RootPath").GetValue(null),
                    Is.EqualTo(originalRoot));
                if (Directory.Exists(isolatedRoot))
                {
                    AssertSafeIsolatedRoot();
                    Directory.Delete(isolatedRoot, true);
                }
            }
        }

        [Test]
        public void STATE_TRANSITION_001_InvalidContextDoesNotPublish()
        {
            var before = Snapshot(gameState);
            object[] args = Begin((int)int.MaxValue);
            Assert.That(args[0], Is.False);
            Assert.That(args[1], Is.Null);
            AssertUnchanged(before);
            Assert.That(Session("HasActiveDuel"), Is.False);
        }

        [Test]
        public void STATE_TRANSITION_002_AcceptedAttemptPublishesThenCancelRestores()
        {
            var before = Snapshot(gameState);
            object[] args = Begin(1);
            Assert.That(args[0], Is.True);
            Assert.That(Session("HasActiveDuel"), Is.True);
            Assert.That(Cancel(args[1]), Is.True);
            AssertUnchanged(before);
            Assert.That(Session("HasActiveDuel"), Is.False);
        }

        [Test]
        public void STATE_TRANSITION_002B_OwnedCancelRestoresPreviousActiveSession()
        {
            Vector3 priorPosition = new Vector3(7f, 1f, 9f);
            session.GetMethod("StartDuel").Invoke(null,
                new[] { Enum.ToObject(rival, 2), true, (object)priorPosition });
            var before = Snapshot(gameState);
            object token = Begin(1)[1];
            Assert.That(Cancel(token), Is.True);
            AssertUnchanged(before);
            Assert.That(Session("HasActiveDuel"), Is.True);
            Assert.That(Session("RivalId"), Is.EqualTo(Enum.ToObject(rival, 2)));
            Assert.That(Session("IsRematch"), Is.True);
            Assert.That(Session("ReturnPosition"), Is.EqualTo(priorPosition));
        }

        [Test]
        public void STATE_TRANSITION_003_MissingControllerRollsBack()
        {
            var before = Snapshot(gameState);
            object token = Begin(2)[1];
            object[] result = Complete(token);
            Assert.That(result[0], Is.False);
            Assert.That(result[1], Does.Contain("ModuleZSceneController"));
            AssertUnchanged(before);
            Assert.That(Session("HasActiveDuel"), Is.False);
        }

        [Test]
        public void STATE_TRANSITION_004_SupersededAttemptCannotUndoNewerOne()
        {
            object old = Begin(1)[1];
            object current = Begin(2)[1];
            Assert.That(Cancel(old), Is.False);
            Assert.That(Complete(old)[0], Is.False);
            Assert.That(Session("RivalId"), Is.EqualTo(Enum.ToObject(rival, 2)));
            Assert.That(Cancel(current), Is.True);
        }

        [Test]
        public void STATE_TRANSITION_005_ExternalSessionChangePreventsRollback()
        {
            object token = Begin(1)[1];
            session.GetMethod("StartDuel").Invoke(null,
                new[] { Enum.ToObject(rival, 2), false, (object)Vector3.one });
            Assert.That(Cancel(token), Is.False);
            Assert.That(Session("RivalId"), Is.EqualTo(Enum.ToObject(rival, 2)));
            Assert.That(Complete(token)[0], Is.False);
        }

        [Test]
        public void STATE_TRANSITION_005B_ExternalLegacyWritePreservesItAndRetiresOwnedSession()
        {
            session.GetMethod("StartDuel").Invoke(null,
                new[] { Enum.ToObject(rival, 2), true, (object)new Vector3(9f, 0f, 8f) });
            var before = Snapshot(gameState);
            object token = Begin(1)[1];
            gameState.GetField("PendingDuelRival").SetValue(null,
                Enum.ToObject(rival, 3));
            Assert.That(Cancel(token), Is.False);
            Assert.That(gameState.GetField("PendingDuelRival").GetValue(null),
                Is.EqualTo(Enum.ToObject(rival, 3)));
            foreach (var field in before)
                if (field.Key.Name != "PendingDuelRival")
                    Assert.That(field.Key.GetValue(null), Is.EqualTo(field.Value),
                        field.Key.Name + " must not retain this attempt's projection");
            Assert.That(Session("HasActiveDuel"), Is.False,
                "A foreign Legacy write cannot coexist with a restored active session");
            Assert.That(Session("RivalId"), Is.EqualTo(Enum.ToObject(rival, 0)));
            Assert.That(Session("IsRematch"), Is.False);
            Assert.That(Session("ReturnPosition"), Is.EqualTo(Vector3.zero));
            Assert.That(Complete(token)[0], Is.False);
        }

        [Test]
        public void STATE_TRANSITION_006_CancellationIsIdempotent()
        {
            object token = Begin(1)[1];
            Assert.That(Cancel(token), Is.True);
            Assert.That(Cancel(token), Is.False);
            Assert.That(Complete(token)[0], Is.False);
        }

        [Test]
        public void STATE_LOAD_STATUS_001_MissingAndMalformedAreNonMutating()
        {
            var before = Snapshot(gameState);
            Assert.That(Load().ToString(), Is.EqualTo("NotFound"));
            AssertUnchanged(before);
            Directory.CreateDirectory(isolatedRoot);
            File.WriteAllText(SavePath(), "{malformed");
            Assert.That(Load().ToString(), Is.EqualTo("InvalidData"));
            AssertUnchanged(before);
        }

        [Test]
        public void STATE_LOAD_STATUS_002_SemanticInvalidityDoesNotApply()
        {
            var before = Snapshot(gameState);
            Directory.CreateDirectory(isolatedRoot);
            File.WriteAllText(SavePath(),
                "{\"duelsWon\":-1,\"duelsLost\":0,\"duelsAbandoned\":0}");
            LogAssert.Expect(LogType.Warning, new Regex("SaveData inválido"));
            Assert.That(Load().ToString(), Is.EqualTo("InvalidData"));
            AssertUnchanged(before);
        }

        [Test]
        public void STATE_LOAD_STATUS_003_ExistingFormatWithOptionalFieldsAbsentLoads()
        {
            Directory.CreateDirectory(isolatedRoot);
            File.WriteAllText(SavePath(),
                "{\"duelsWon\":2,\"duelsLost\":1,\"duelsAbandoned\":0," +
                "\"currentOpenWorldTheme\":0}");
            Assert.That(Load().ToString(), Is.EqualTo("Success"));
            Assert.That(gameState.GetField("DuelsWon").GetValue(null), Is.EqualTo(2));
        }

        [Test]
        public void STATE_LOAD_STATUS_003B_CurrentFormatWithAchievementPayloadReads()
        {
            Directory.CreateDirectory(isolatedRoot);
            File.WriteAllText(SavePath(),
                "{\"returnX\":1.5,\"returnY\":0.1,\"returnZ\":-4," +
                "\"rivalMadridDefeated\":true,\"duelsWon\":2," +
                "\"duelsLost\":1,\"duelsAbandoned\":0," +
                "\"currentOpenWorldTheme\":0,\"rematchesWon\":0," +
                "\"achievements\":[{\"achievementId\":1," +
                "\"unlocked\":true,\"unlockedDate\":\"2026-01-01 00:00:00\"}]}");
            object data = saveManager.GetMethod("GetSaveData", PublicStatic)
                .Invoke(null, null);
            Assert.That(data, Is.Not.Null);
            Assert.That(data.GetType().GetField("duelsWon").GetValue(data), Is.EqualTo(2));
            Assert.That(gameState.GetField("DuelsWon").GetValue(null),
                Is.EqualTo(priorGame[gameState.GetField("DuelsWon")]),
                "Read-only compatibility inspection cannot apply the save");
        }

        [TestCase("{\"nested\":{\"duelsWon\":1,\"duelsLost\":0,\"duelsAbandoned\":0}}")]
        [TestCase("{\"duelsWon\":1,\"duelsLost\":0,\"duelsAbandoned\":0,}")]
        [TestCase("{\"duelsWon\":\"1\",\"duelsLost\":0,\"duelsAbandoned\":0}")]
        [TestCase("{\"duelsWon\":1,\"duelsLost\":false,\"duelsAbandoned\":0}")]
        [TestCase("{\"duelsWon\":1,\"duelsLost\":0,\"duelsAbandoned\":0,\"rivalMadridDefeated\":1}")]
        [TestCase("{\"duelsWon\":1,\"duelsLost\":0,\"duelsAbandoned\":0,\"achievements\":[{\"achievementId\":\"1\"}]}")]
        [TestCase("{\"duelsWon\":2147483648,\"duelsLost\":0,\"duelsAbandoned\":0}")]
        [TestCase("{\"duelsWon\":-2147483649,\"duelsLost\":0,\"duelsAbandoned\":0}")]
        [TestCase("{\"duelsWon\":01,\"duelsLost\":0,\"duelsAbandoned\":0}")]
        [TestCase("{\"duelsWon\":+1,\"duelsLost\":0,\"duelsAbandoned\":0}")]
        [TestCase("{\"duelsWon\":1.,\"duelsLost\":0,\"duelsAbandoned\":0}")]
        [TestCase("{\"duelsWon\":1e0,\"duelsLost\":0,\"duelsAbandoned\":0}")]
        [TestCase("{\"duelsWon\":1,\"duelsLost\":0,\"duelsAbandoned\":0,\"returnX\":1e+}")]
        [TestCase("{\"duelsWon\":1,\"duelsLost\":0,\"duelsAbandoned\":0,\"returnX\":NaN}")]
        [TestCase("{\"duelsWon\":1,\"duelsLost\":0,\"duelsAbandoned\":0,\"returnX\":\"1.25\"}")]
        public void STATE_LOAD_STATUS_004_BadRootOrTypesCannotApply(string json)
        {
            var before = Snapshot(gameState);
            var achievementsBefore = AchievementValues();
            Directory.CreateDirectory(isolatedRoot);
            File.WriteAllText(SavePath(), json);
            Assert.That(Load().ToString(), Is.EqualTo("InvalidData"));
            AssertUnchanged(before);
            AssertAchievementsUnchanged(achievementsBefore);
        }

        [TestCase("0", "0", "0", 0, 0, 0)]
        [TestCase("2147483647", "0", "0", int.MaxValue, 0, 0)]
        [TestCase("0", "2147483647", "0", 0, int.MaxValue, 0)]
        [TestCase("0", "0", "2147483647", 0, 0, int.MaxValue)]
        public void STATE_LOAD_STATUS_004B_IntegerBoundariesReadWithoutApplying(
            string won, string lost, string abandoned, int expectedWon,
            int expectedLost, int expectedAbandoned)
        {
            var before = Snapshot(gameState);
            Directory.CreateDirectory(isolatedRoot);
            File.WriteAllText(SavePath(),
                "{\"duelsWon\":" + won + ",\"duelsLost\":" + lost +
                ",\"duelsAbandoned\":" + abandoned + "}");
            object data = saveManager.GetMethod("GetSaveData", PublicStatic)
                .Invoke(null, null);
            Assert.That(data, Is.Not.Null);
            Assert.That(data.GetType().GetField("duelsWon").GetValue(data),
                Is.EqualTo(expectedWon));
            Assert.That(data.GetType().GetField("duelsLost").GetValue(data),
                Is.EqualTo(expectedLost));
            Assert.That(data.GetType().GetField("duelsAbandoned").GetValue(data),
                Is.EqualTo(expectedAbandoned));
            AssertUnchanged(before);
        }

        [TestCase("0", "-0", "0.0", 0f, 0f, 0f)]
        [TestCase("-12.5", "1.25e2", "3E-2", -12.5f, 125f, 0.03f)]
        [TestCase("1e30", "-1e30", "0.125", 1e30f, -1e30f, 0.125f)]
        public void STATE_LOAD_STATUS_004C_ValidCoordinateNumbersRead(
            string x, string y, string z, float expectedX, float expectedY,
            float expectedZ)
        {
            var before = Snapshot(gameState);
            Directory.CreateDirectory(isolatedRoot);
            File.WriteAllText(SavePath(),
                "{\"duelsWon\":0,\"duelsLost\":0,\"duelsAbandoned\":0," +
                "\"returnX\":" + x + ",\"returnY\":" + y +
                ",\"returnZ\":" + z + "}");
            object data = saveManager.GetMethod("GetSaveData", PublicStatic)
                .Invoke(null, null);
            Assert.That(data, Is.Not.Null);
            Assert.That((float)data.GetType().GetField("returnX").GetValue(data),
                Is.EqualTo(expectedX).Within(Mathf.Max(0.0001f,
                    Mathf.Abs(expectedX) * 0.00001f)));
            Assert.That((float)data.GetType().GetField("returnY").GetValue(data),
                Is.EqualTo(expectedY).Within(Mathf.Max(0.0001f,
                    Mathf.Abs(expectedY) * 0.00001f)));
            Assert.That((float)data.GetType().GetField("returnZ").GetValue(data),
                Is.EqualTo(expectedZ).Within(Mathf.Max(0.0001f,
                    Mathf.Abs(expectedZ) * 0.00001f)));
            AssertUnchanged(before);
        }

        [Test]
        public void STATE_LOAD_STATUS_004D_SaveGameOutputIsReadableAndLoadable()
        {
            AssertSafeIsolatedRoot();
            Assert.That((string)paths.GetProperty("RootPath").GetValue(null),
                Is.EqualTo(isolatedRoot));
            gameState.GetMethod("ResetGameState").Invoke(null, null);
            gameState.GetField("DuelsWon").SetValue(null, 2);
            gameState.GetField("OpenWorldReturnPosition").SetValue(null,
                new Vector3(-12.5f, 0.125f, 125f));
            unlockHandler.SetValue(null, null);

            saveManager.GetMethod("SaveGame", PublicStatic).Invoke(null, null);
            Assert.That(File.Exists(SavePath()), Is.True);
            Assert.That(new FileInfo(SavePath()).Length, Is.GreaterThan(0));
            Assert.That(saveManager.GetMethod("GetSaveData", PublicStatic)
                .Invoke(null, null), Is.Not.Null);
            Assert.That(Load().ToString(), Is.EqualTo("Success"));
            Assert.That(gameState.GetField("DuelsWon").GetValue(null), Is.EqualTo(2));
            Assert.That(gameState.GetField("OpenWorldReturnPosition").GetValue(null),
                Is.EqualTo(new Vector3(-12.5f, 0.125f, 125f)));
        }

        [Test]
        public void STATE_LOAD_STATUS_005_ApplicationFailureRestoresStateAndAchievements()
        {
            achievementManager.GetMethod("ResetAll").Invoke(null, null);
            gameState.GetField("DuelsWon").SetValue(null, 0);
            gameState.GetField("DuelsLost").SetValue(null, 0);
            gameState.GetField("DuelsAbandoned").SetValue(null, 0);
            var before = Snapshot(gameState);
            var achievementsBefore = AchievementValues();
            Type idType = Require("ModuleZ.Core.Achievements.ModuleZAchievementId");
            MethodInfo reject = GetType().GetMethod(nameof(RejectAchievement),
                BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(idType);
            unlockHandler.SetValue(null, Delegate.CreateDelegate(
                unlockHandler.FieldType, this, reject));
            Directory.CreateDirectory(isolatedRoot);
            File.WriteAllText(SavePath(),
                "{\"duelsWon\":1,\"duelsLost\":0,\"duelsAbandoned\":0," +
                "\"achievements\":[{\"achievementId\":1,\"unlocked\":true," +
                "\"unlockedDate\":\"prior-date\"}]}");
            LogAssert.Expect(LogType.Warning, new Regex("No se pudo aplicar la partida"));
            Assert.That(Load().ToString(), Is.EqualTo("ApplyError"));
            AssertUnchanged(before);
            AssertAchievementsUnchanged(achievementsBefore);
        }

        public void RejectAchievement<T>(T id)
        {
            throw new InvalidOperationException("Injected achievement notification failure");
        }

        private object[] Begin(int rivalValue)
        {
            object accepted = Activator.CreateInstance(context,
                Enum.ToObject(rival, rivalValue), false, new Vector3(3f, 0f, 4f));
            object[] args = { accepted, null, null };
            MethodInfo method = bridge.GetMethod("Begin", new[] {
                context, attempt.MakeByRefType(), typeof(string).MakeByRefType() });
            bool ok = (bool)method.Invoke(null, args);
            if (ok)
                attempts.Add(args[1]);
            return new object[] { ok, args[1], args[2] };
        }

        private bool Cancel(object token) =>
            (bool)bridge.GetMethod("Cancel", PublicStatic).Invoke(null, new[] { token });

        private object[] Complete(object token)
        {
            object[] args = { token, null };
            bool ok = (bool)bridge.GetMethod("CompleteAfterDelay", PublicStatic)
                .Invoke(null, args);
            return new object[] { ok, args[1] };
        }

        private object Load() => saveManager.GetMethod("TryLoadGame", PublicStatic)
            .Invoke(null, null);

        private string SavePath() => Path.Combine(isolatedRoot, "modulez_save.json");

        private void AssertSafeIsolatedRoot()
        {
            string isolated = Path.GetFullPath(isolatedRoot).TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string real = Path.GetFullPath(Application.persistentDataPath).TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string temp = Path.GetFullPath(Path.GetTempPath()).TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            Assert.That(isolated.StartsWith(temp + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase), Is.True);
            Assert.That(isolated.Equals(real, StringComparison.OrdinalIgnoreCase) ||
                isolated.StartsWith(real + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase) ||
                real.StartsWith(isolated + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase), Is.False);
            for (string cursor = isolated; cursor != null;
                cursor = Directory.GetParent(cursor)?.FullName)
                if (Directory.Exists(cursor))
                    Assert.That((File.GetAttributes(cursor) & FileAttributes.ReparsePoint) == 0,
                        Is.True, "Isolated test root must not resolve through a reparse point");
        }

        private object Session(string name) => session.GetField(name, PublicStatic).GetValue(null);

        private static Type Require(string name)
        {
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(name, false))
                .FirstOrDefault(candidate => candidate != null);
            Assert.That(type, Is.Not.Null, name);
            return type;
        }

        private static Dictionary<FieldInfo, object> Snapshot(Type type) =>
            type.GetFields(PublicStatic).ToDictionary(field => field,
                field => field.GetValue(null));

        private static void Restore(Dictionary<FieldInfo, object> state)
        {
            foreach (var entry in state)
                entry.Key.SetValue(null, entry.Value);
        }

        private void RestoreSession(Dictionary<FieldInfo, object> state)
        {
            // Keep Revision monotonic: using the public API invalidates any
            // stale attempt instead of silently restoring fields behind it.
            if ((bool)state[session.GetField("HasActiveDuel")])
                session.GetMethod("StartDuel").Invoke(null, new[] {
                    state[session.GetField("RivalId")],
                    state[session.GetField("IsRematch")],
                    state[session.GetField("ReturnPosition")]
                });
            else session.GetMethod("Clear").Invoke(null, null);
        }

        private object AchievementList(IEnumerable<object> values)
        {
            Type stateType = Require("ModuleZ.Core.Achievements.ModuleZAchievementState");
            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(stateType));
            foreach (object value in values) list.Add(value);
            return list;
        }

        private Dictionary<object, object[]> AchievementValues()
        {
            var result = new Dictionary<object, object[]>();
            foreach (object state in (IEnumerable)achievementManager.GetMethod("GetAll").Invoke(null, null))
                result.Add(state, new[] {
                    state.GetType().GetField("unlocked").GetValue(state),
                    state.GetType().GetField("unlockedDate").GetValue(state)
                });
            return result;
        }

        private void AssertAchievementsUnchanged(Dictionary<object, object[]> expected)
        {
            var current = ((IEnumerable)achievementManager.GetMethod("GetAll").Invoke(null, null))
                .Cast<object>().ToList();
            CollectionAssert.AreEquivalent(expected.Keys, current);
            foreach (var entry in expected)
            {
                Assert.That(entry.Key.GetType().GetField("unlocked").GetValue(entry.Key),
                    Is.EqualTo(entry.Value[0]));
                Assert.That(entry.Key.GetType().GetField("unlockedDate").GetValue(entry.Key),
                    Is.EqualTo(entry.Value[1]));
            }
        }

        private static void AssertUnchanged(Dictionary<FieldInfo, object> state)
        {
            foreach (var entry in state)
                Assert.That(entry.Key.GetValue(null), Is.EqualTo(entry.Value), entry.Key.Name);
        }
    }
}
