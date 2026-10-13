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
            Type achievements = Require("ModuleZ.Core.Achievements.ModuleZAchievementManager");
            foreach (object state in (IEnumerable)achievements.GetMethod("GetAll").Invoke(null, null))
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
            Assert.That(Path.GetFullPath(isolatedRoot), Is.Not.EqualTo(
                Path.GetFullPath(Application.persistentDataPath)));
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
                Restore(priorSession);
                foreach (var entry in priorAchievements)
                {
                    Type stateType = entry.Key.GetType();
                    stateType.GetField("unlocked").SetValue(entry.Key, entry.Value[0]);
                    stateType.GetField("unlockedDate").SetValue(entry.Key, entry.Value[1]);
                }
            }
            finally
            {
                paths.GetMethod("ClearRootOverride").Invoke(null, null);
                Assert.That((string)paths.GetProperty("RootPath").GetValue(null),
                    Is.EqualTo(originalRoot));
                if (Directory.Exists(isolatedRoot))
                    Directory.Delete(isolatedRoot, true);
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
            object token = Begin(1)[1];
            gameState.GetField("PendingDuelRival").SetValue(null,
                Enum.ToObject(rival, 2));
            Assert.That(Cancel(token), Is.False);
            Assert.That(gameState.GetField("PendingDuelRival").GetValue(null),
                Is.EqualTo(Enum.ToObject(rival, 2)));
            Assert.That(Session("HasActiveDuel"), Is.False);
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

        private static void AssertUnchanged(Dictionary<FieldInfo, object> state)
        {
            foreach (var entry in state)
                Assert.That(entry.Key.GetValue(null), Is.EqualTo(entry.Value), entry.Key.Name);
        }
    }
}
