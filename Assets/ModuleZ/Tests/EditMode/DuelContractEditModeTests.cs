using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace ModuleZ.Tests.EditMode
{
    // Production remains in Assembly-CSharp. Probe only its public contracts;
    // no private production state or lifecycle method is accessed.
    public sealed class DuelContractEditModeTests
    {
        private const string ContextName = "ModuleZ.Game.DuelTransition.DuelContext";
        private const string ResultName = "ModuleZ.Game.DuelTransition.DuelResult";
        private const string OutcomeName = "ModuleZ.Game.DuelTransition.DuelOutcome";
        private const string ResultBridgeName = "ModuleZ.Game.DuelTransition.DuelResultBridge";
        private const string TransitionBridgeName = "ModuleZ.Game.DuelTransition.DuelTransitionBridge";
        private const string RivalName = "ModuleZ.OpenWorld.Encounters.ModuleZRivalId";
        private const string SessionName = "ModuleZ.Core.Managers.ModuleZDuelSessionState";
        private const string GameStateName = "ModuleZ.Core.Managers.ModuleZGameState";
        private const string SaveManagerName = "ModuleZ.Core.SaveSystem.ModuleZSaveManager";
        private const string PersistencePathsName = "ModuleZ.Core.SaveSystem.ModuleZPersistencePaths";
        private const string ThemeName = "ModuleZ.OpenWorld.Runtime.OpenWorldThemeId";

        private object capturedContext;
        private object capturedResult;
        private int captureCount;
        private Dictionary<FieldInfo, object> priorSession;
        private Dictionary<FieldInfo, object> priorGameState;

        [SetUp]
        public void SetUp()
        {
            priorGameState = Snapshot(RequireType(GameStateName));
            priorSession = Snapshot(RequireType(SessionName));
            InvokeSession("Clear");
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (priorGameState != null)
                    Restore(priorGameState);
            }
            finally
            {
                if (priorSession != null)
                    Restore(priorSession);
            }
        }

        [Test]
        public void L1_CONTEXT_001_PayloadIsPreservedAndInvalidRivalIsRejectedAtEntry()
        {
            object madrid = Enum.Parse(RequireType(RivalName), "Madrid");
            Vector3 returnPosition = new Vector3(12f, 2f, -4f);
            object context = Context(madrid, true, returnPosition);

            Assert.That(Property(context, "RivalId"), Is.EqualTo(madrid));
            Assert.That(Property(context, "IsRematch"), Is.EqualTo(true));
            Assert.That(Property(context, "ReturnPosition"), Is.EqualTo(returnPosition));

            // The value object accepts arbitrary enum bits; the public entry
            // bridge, not the constructor, enforces rival validity.
            object invalidRival = Enum.ToObject(RequireType(RivalName), int.MaxValue);
            object invalidContext = Context(invalidRival, false, Vector3.zero);
            object[] args = { invalidContext, null };
            bool accepted = (bool)RequireType(TransitionBridgeName)
                .GetMethod("Begin", new[] { RequireType(ContextName), typeof(string).MakeByRefType() })
                .Invoke(null, args);

            Assert.That(accepted, Is.False);
            Assert.That(args[1], Is.EqualTo("Duel rival is invalid."));
            AssertInactiveSession();
        }

        [Test]
        public void L1_CONTEXT_002_STATE_ENTRY_001_ValidEntryPublishesTheAcceptedContextPayload()
        {
            // STATE-ENTRY-001: Begin writes nine Legacy compatibility fields.
            // Restore exactly the public fields it changes.
            string[] names = {
                "PendingDuelRival", "CurrentDuelRival", "CurrentDuelIsRematch",
                "OpenWorldReturnPosition", "ReturningFromDuel", "DuelCompleted",
                "DuelWasCancelled", "DuelWasLost", "DuelWasAbandoned"
            };
            Type gameState = RequireType(GameStateName);
            var prior = new Dictionary<FieldInfo, object>();
            foreach (string name in names)
            {
                FieldInfo field = gameState.GetField(name,
                    BindingFlags.Public | BindingFlags.Static);
                Assert.That(field, Is.Not.Null, name);
                prior.Add(field, field.GetValue(null));
            }

            try
            {
                object rival = Enum.Parse(RequireType(RivalName), "Andalucia");
                Vector3 position = new Vector3(4f, 1f, -8f);
                object[] args = { Context(rival, true, position), null };
                bool accepted = (bool)RequireType(TransitionBridgeName)
                    .GetMethod("Begin", new[] { RequireType(ContextName), typeof(string).MakeByRefType() })
                    .Invoke(null, args);

                Assert.That(accepted, Is.True);
                Assert.That(args[1], Is.Null);
                Assert.That(SessionField("HasActiveDuel"), Is.EqualTo(true));
                Assert.That(SessionField("RivalId"), Is.EqualTo(rival));
                Assert.That(SessionField("IsRematch"), Is.EqualTo(true));
                Assert.That(SessionField("ReturnPosition"), Is.EqualTo(position));
                var expected = new Dictionary<string, object> {
                    { "PendingDuelRival", rival },
                    { "CurrentDuelRival", rival },
                    { "CurrentDuelIsRematch", true },
                    { "OpenWorldReturnPosition", position },
                    { "ReturningFromDuel", true },
                    { "DuelCompleted", false },
                    { "DuelWasCancelled", false },
                    { "DuelWasLost", false },
                    { "DuelWasAbandoned", false }
                };
                foreach (KeyValuePair<string, object> entry in expected)
                    Assert.That(gameState.GetField(entry.Key).GetValue(null),
                        Is.EqualTo(entry.Value), entry.Key);

                // Rejection must leave both the active session and every
                // published Legacy projection exactly as they were.
                var publishedSession = Snapshot(RequireType(SessionName));
                object invalidRival = Enum.ToObject(RequireType(RivalName), int.MaxValue);
                object[] invalidArgs = { Context(invalidRival, false, Vector3.zero), null };
                bool invalidAccepted = (bool)RequireType(TransitionBridgeName)
                    .GetMethod("Begin", new[] { RequireType(ContextName), typeof(string).MakeByRefType() })
                    .Invoke(null, invalidArgs);
                Assert.That(invalidAccepted, Is.False);
                Assert.That(invalidArgs[1], Is.EqualTo("Duel rival is invalid."));
                AssertUnchanged(publishedSession);
                foreach (KeyValuePair<string, object> entry in expected)
                    Assert.That(gameState.GetField(entry.Key).GetValue(null),
                        Is.EqualTo(entry.Value), entry.Key + " after invalid Begin");
            }
            finally
            {
                Restore(prior);
            }
        }

        [Test]
        public void STATE_RESET_001_ResetRestoresThirtyDefaultsAndPreservesConfiguration()
        {
            Type gameState = RequireType(GameStateName);
            var original = Snapshot(gameState);
            Assert.That(original.Count, Is.EqualTo(32), "GameState public field inventory");
            var defaults = new Dictionary<string, object> {
                { "OpenWorldReturnPosition", new Vector3(0f, 0.1f, -4f) },
                { "ReturningFromDuel", false },
                { "CurrentDuelRival", Enum.Parse(RequireType(RivalName), "Madrid") },
                { "RivalMadridDefeated", false },
                { "RivalBarcelonaDefeated", false },
                { "RivalValenciaDefeated", false },
                { "RivalAndaluciaDefeated", false },
                { "AndaluciaUnlocked", false },
                { "DuelsWon", 0 },
                { "DuelsLost", 0 },
                { "DuelsAbandoned", 0 },
                { "DuelWasCancelled", false },
                { "DuelCompleted", false },
                { "DuelWasLost", false },
                { "DuelWasAbandoned", false },
                { "LastDuelResultMessage", "" },
                { "PendingOpenWorldMessage", "" },
                { "IsPaused", false },
                { "CurrentOpenWorldTheme", Enum.Parse(RequireType(ThemeName), "Madrid70s") },
                { "MainProgressionCompleted", false },
                { "CurrentDuelIsRematch", false },
                { "RematchesWon", 0 },
                { "RematchesLost", 0 },
                { "RematchesAbandoned", 0 },
                { "IsOverlayOpen", false },
                { "RivalMadridPersonalityCompleted", false },
                { "RivalBarcelonaPersonalityCompleted", false },
                { "RivalValenciaPersonalityCompleted", false },
                { "RivalAndaluciaPersonalityCompleted", false },
                { "PendingDuelRival", Enum.Parse(RequireType(RivalName), "Madrid") }
            };
            Assert.That(defaults.Count, Is.EqualTo(30));

            try
            {
                SetDistinctiveGameStateValues(gameState);

                gameState.GetMethod("ResetGameState", BindingFlags.Public | BindingFlags.Static)
                    .Invoke(null, null);
                foreach (KeyValuePair<string, object> entry in defaults)
                    Assert.That(gameState.GetField(entry.Key).GetValue(null),
                        Is.EqualTo(entry.Value), entry.Key);
                Assert.That(gameState.GetField("AudioEnabled").GetValue(null), Is.False);
                Assert.That(gameState.GetField("FullscreenEnabled").GetValue(null), Is.False);
            }
            finally
            {
                Restore(original);
            }
        }

        [Test]
        public void STATE_LOAD_ABSENT_001_MissingIsolatedSaveDoesNotMutateState()
        {
            var gameState = Snapshot(RequireType(GameStateName));
            var session = Snapshot(RequireType(SessionName));
            Type paths = RequireType(PersistencePathsName);
            string originalRoot = (string)paths.GetProperty("RootPath").GetValue(null);
            string isolatedRoot = Path.Combine(Path.GetTempPath(),
                "ModuleZ-EditMode-AbsentSave-" + Guid.NewGuid().ToString("N"));
            Assert.That(Directory.Exists(isolatedRoot), Is.False);
            Assert.That(File.Exists(Path.Combine(isolatedRoot, "modulez_save.json")), Is.False);
            Assert.That(Path.GetFullPath(isolatedRoot), Is.Not.EqualTo(Path.GetFullPath(originalRoot)));
            string realProfile = Path.GetFullPath(Application.persistentDataPath)
                .TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            Assert.That(Path.GetFullPath(isolatedRoot).StartsWith(realProfile,
                StringComparison.OrdinalIgnoreCase), Is.False);

            try
            {
                paths.GetMethod("SetRootOverride", BindingFlags.Public | BindingFlags.Static)
                    .Invoke(null, new object[] { isolatedRoot });
                Assert.That(paths.GetProperty("RootPath").GetValue(null), Is.EqualTo(isolatedRoot));
                SetDistinctiveGameStateValues(RequireType(GameStateName));
                InvokeSession("StartDuel", Enum.Parse(RequireType(RivalName), "Valencia"),
                    true, new Vector3(13f, 2f, -9f));
                var expectedGameState = Snapshot(RequireType(GameStateName));
                var expectedSession = Snapshot(RequireType(SessionName));
                RequireType(SaveManagerName)
                    .GetMethod("LoadGame", BindingFlags.Public | BindingFlags.Static)
                    .Invoke(null, null);
                AssertUnchanged(expectedGameState);
                AssertUnchanged(expectedSession);
                Assert.That(Directory.Exists(isolatedRoot), Is.False,
                    "A missing-save read must not create storage");
            }
            finally
            {
                paths.GetMethod("ClearRootOverride", BindingFlags.Public | BindingFlags.Static)
                    .Invoke(null, null);
                Restore(gameState);
                Restore(session);
            }
            Assert.That(paths.GetProperty("RootPath").GetValue(null), Is.EqualTo(originalRoot));
        }

        [Test]
        public void L1_RESULT_001_DeclaredOutcomesPreserveIdentityAndReachProcessor()
        {
            Type outcomeType = RequireType(OutcomeName);
            CollectionAssert.AreEquivalent(
                new[] { "Victory", "Defeat", "Abandoned" },
                Enum.GetNames(outcomeType));
            // Draw is a match result mapped to Defeat before DuelResult exists.
            object rival = Enum.Parse(RequireType(RivalName), "Barcelona");
            object context = Context(rival, true, new Vector3(1f, 0f, 3f));

            foreach (string name in Enum.GetNames(outcomeType))
            {
                object outcome = Enum.Parse(outcomeType, name);
                object result = Result(outcome, rival);
                Assert.That(Property(result, "Outcome"), Is.EqualTo(outcome));
                Assert.That(Property(result, "RivalId"), Is.EqualTo(rival));

                captureCount = 0;
                object[] args = Complete(context, result, CaptureDelegate());
                Assert.That(args[3], Is.Null, name);
                Assert.That(captureCount, Is.EqualTo(1), name);
                Assert.That(Property(capturedContext, "RivalId"), Is.EqualTo(rival));
                Assert.That(Property(capturedResult, "Outcome"), Is.EqualTo(outcome));
            }
        }

        [Test]
        public void L1_RESULT_002_InvalidFactsAndMismatchedIdentityNeverReachProcessor()
        {
            object madrid = Enum.Parse(RequireType(RivalName), "Madrid");
            object barcelona = Enum.Parse(RequireType(RivalName), "Barcelona");
            object context = Context(madrid, false, Vector3.zero);
            object victory = Enum.Parse(RequireType(OutcomeName), "Victory");
            object invalidOutcome = Enum.ToObject(RequireType(OutcomeName), int.MaxValue);
            object invalidRival = Enum.ToObject(RequireType(RivalName), int.MaxValue);

            AssertRejected(context, Result(invalidOutcome, madrid),
                "Duel outcome is invalid.");
            AssertRejected(context, Result(victory, invalidRival),
                "Duel result rival is invalid.");
            AssertRejected(context, Result(victory, barcelona),
                "Duel result rival does not match the accepted context.");

            object[] withoutProcessor = Complete(context, Result(victory, madrid), null);
            Assert.That(withoutProcessor[3], Is.EqualTo("Duel result processor is unavailable."));
            Assert.That(captureCount, Is.Zero);
        }

        [Test]
        public void L1_SESSION_001_PublishesPayloadAndClearIsFullyIdempotent()
        {
            AssertInactiveSession();
            object rival = Enum.Parse(RequireType(RivalName), "Valencia");
            Vector3 returnPosition = new Vector3(-2f, 0.5f, 9f);

            InvokeSession("StartDuel", rival, true, returnPosition);
            Assert.That(SessionField("HasActiveDuel"), Is.EqualTo(true));
            Assert.That(SessionField("RivalId"), Is.EqualTo(rival));
            Assert.That(SessionField("IsRematch"), Is.EqualTo(true));
            Assert.That(SessionField("ReturnPosition"), Is.EqualTo(returnPosition));

            InvokeSession("Clear");
            AssertInactiveSession();
            InvokeSession("Clear");
            AssertInactiveSession();
        }

        private void AssertRejected(object context, object result, string reason)
        {
            captureCount = 0;
            object[] args = Complete(context, result, CaptureDelegate());
            Assert.That(args[3], Is.EqualTo(reason));
            Assert.That(captureCount, Is.Zero);
        }

        private static object[] Complete(object context, object result, Delegate processor)
        {
            object[] args = { context, result, processor, null };
            bool accepted = (bool)RequireType(ResultBridgeName)
                .GetMethod("Complete", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, args);
            Assert.That(accepted, Is.EqualTo(processor != null && args[3] == null));
            return args;
        }

        private Delegate CaptureDelegate()
        {
            Type actionType = typeof(Action<,>).MakeGenericType(
                RequireType(ContextName), RequireType(ResultName));
            MethodInfo capture = GetType()
                .GetMethod(nameof(Capture), BindingFlags.Public | BindingFlags.Instance)
                .MakeGenericMethod(RequireType(ContextName), RequireType(ResultName));
            return Delegate.CreateDelegate(actionType, this, capture);
        }

        public void Capture<TContext, TResult>(TContext context, TResult result)
        {
            capturedContext = context;
            capturedResult = result;
            captureCount++;
        }

        private static object Context(object rival, bool rematch, Vector3 position)
        {
            return Activator.CreateInstance(RequireType(ContextName), rival, rematch, position);
        }

        private static object Result(object outcome, object rival)
        {
            return Activator.CreateInstance(RequireType(ResultName), outcome, rival);
        }

        private static object Property(object value, string name)
        {
            return value.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance)
                .GetValue(value);
        }

        private static object SessionField(string name)
        {
            return RequireType(SessionName).GetField(name, BindingFlags.Public | BindingFlags.Static)
                .GetValue(null);
        }

        private static Dictionary<FieldInfo, object> Snapshot(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .ToDictionary(field => field, field => field.GetValue(null));
        }

        private static void SetDistinctiveGameStateValues(Type gameState)
        {
            foreach (FieldInfo field in gameState.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                object sentinel;
                if (field.FieldType == typeof(bool))
                    sentinel = field.Name == "AudioEnabled" ||
                        field.Name == "FullscreenEnabled" ? (object)false : true;
                else if (field.FieldType == typeof(int))
                    sentinel = 937;
                else if (field.FieldType == typeof(string))
                    sentinel = "ep02-sentinel";
                else if (field.FieldType == typeof(Vector3))
                    sentinel = new Vector3(44f, 55f, 66f);
                else if (field.FieldType == RequireType(RivalName))
                    sentinel = Enum.Parse(field.FieldType, "Barcelona");
                else if (field.FieldType == RequireType(ThemeName))
                    sentinel = Enum.Parse(field.FieldType, "Barcelona70s");
                else
                {
                    Assert.Fail("Unrecognized GameState field: " + field.Name);
                    continue;
                }
                field.SetValue(null, sentinel);
            }
        }

        private static void AssertUnchanged(Dictionary<FieldInfo, object> snapshot)
        {
            foreach (KeyValuePair<FieldInfo, object> entry in snapshot)
                Assert.That(entry.Key.GetValue(null), Is.EqualTo(entry.Value),
                    entry.Key.DeclaringType.Name + "." + entry.Key.Name);
        }

        private static void Restore(Dictionary<FieldInfo, object> snapshot)
        {
            foreach (KeyValuePair<FieldInfo, object> entry in snapshot)
                entry.Key.SetValue(null, entry.Value);
        }

        private static void InvokeSession(string name, params object[] args)
        {
            RequireType(SessionName).GetMethod(name, BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, args);
        }

        private static void AssertInactiveSession()
        {
            Assert.That(SessionField("HasActiveDuel"), Is.EqualTo(false));
            Assert.That(SessionField("RivalId"),
                Is.EqualTo(Activator.CreateInstance(RequireType(RivalName))));
            Assert.That(SessionField("IsRematch"), Is.EqualTo(false));
            Assert.That(SessionField("ReturnPosition"), Is.EqualTo(Vector3.zero));
        }

        private static Type RequireType(string name)
        {
            Type type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType(name, false))
                .FirstOrDefault(candidate => candidate != null);
            Assert.That(type, Is.Not.Null, "Public production contract not found: " + name);
            return type;
        }
    }
}
