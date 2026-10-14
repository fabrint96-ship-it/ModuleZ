using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ModuleZ.Tests.PlayMode.Support;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ModuleZ.Tests.PlayMode
{
    [Category("ApplicationState")]
    public sealed class ApplicationStateMenuTests
    {
        private const string GameStateName = "ModuleZ.Core.Managers.ModuleZGameState";
        private const string SessionName = "ModuleZ.Core.Managers.ModuleZDuelSessionState";
        private const string PathsName = "ModuleZ.Core.SaveSystem.ModuleZPersistencePaths";
        private const string AchievementName = "ModuleZ.Core.Achievements.ModuleZAchievementManager";
        private readonly Dictionary<FieldInfo, object> gameBefore = new Dictionary<FieldInfo, object>();
        private readonly Dictionary<FieldInfo, object> sessionBefore = new Dictionary<FieldInfo, object>();
        private readonly Dictionary<object, object[]> achievementsBefore = new Dictionary<object, object[]>();
        private string isolatedRoot;
        private string processRoot;
        private GameObject bootRuntime;
        private float timeScaleBefore;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            gameBefore.Clear();
            sessionBefore.Clear();
            achievementsBefore.Clear();
            timeScaleBefore = Time.timeScale;
            Type paths = RuntimeContractProbe.RequireType(PathsName);
            processRoot = (string)RuntimeContractProbe.GetPublicStaticMember(paths, "RootPath");
            Assert.That(Environment.GetEnvironmentVariable("MODULEZ_VALIDATION_ISOLATED"),
                Is.EqualTo("1"), "Menu tests require process-level persistence isolation.");
            Assert.That(processRoot, Is.Not.EqualTo(Application.persistentDataPath));
            isolatedRoot = Path.Combine(processRoot, "ep02-menu", Guid.NewGuid().ToString("N"));
            RuntimeContractProbe.InvokePublicStatic(paths, "SetRootOverride", isolatedRoot);
            Snapshot(RuntimeContractProbe.RequireType(GameStateName), gameBefore);
            Snapshot(RuntimeContractProbe.RequireType(SessionName), sessionBefore);
            foreach (object state in (IEnumerable)RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(AchievementName), "GetAll"))
            {
                Type type = state.GetType();
                achievementsBefore.Add(state, new[] {
                    type.GetField("unlocked").GetValue(state),
                    type.GetField("unlockedDate").GetValue(state)
                });
            }
            Time.timeScale = 1f;
            yield return SceneTestDriver.LoadSingle(SceneTestDriver.BootPath);
            bootRuntime = GameObject.Find("ModuleZ_Runtime");
            Assert.That(bootRuntime, Is.Not.Null);
            yield return WaitForMenu();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Time.timeScale = 1f;
            yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
            if (bootRuntime != null)
                UnityEngine.Object.Destroy(bootRuntime);
            yield return null;
            yield return null;
            Time.timeScale = timeScaleBefore;
            Restore(gameBefore);
            RestoreSession(sessionBefore);
            foreach (var entry in achievementsBefore)
            {
                Type type = entry.Key.GetType();
                type.GetField("unlocked").SetValue(entry.Key, entry.Value[0]);
                type.GetField("unlockedDate").SetValue(entry.Key, entry.Value[1]);
            }
            Type achievementState = RuntimeContractProbe.RequireType(
                "ModuleZ.Core.Achievements.ModuleZAchievementState");
            IList originalStates = (IList)Activator.CreateInstance(
                typeof(List<>).MakeGenericType(achievementState));
            foreach (object state in achievementsBefore.Keys)
                originalStates.Add(state);
            RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(AchievementName), "LoadStates", originalStates);
            Assert.That(RuntimeContractProbe.FindSceneComponents(
                    "ModuleZ.Core.SaveSystem.ModuleZAutoSaveController"), Is.Empty,
                "Boot autosaver survived test teardown; retain isolated root.");
            RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(PathsName), "ClearRootOverride");
            if (Directory.Exists(isolatedRoot))
            {
                string expectedParent = Path.GetFullPath(Path.Combine(
                    processRoot, "ep02-menu")) + Path.DirectorySeparatorChar;
                Assert.That(Path.GetFullPath(isolatedRoot).StartsWith(
                    expectedParent, StringComparison.OrdinalIgnoreCase), Is.True);
                Directory.Delete(isolatedRoot, true);
            }
        }

        [UnityTest]
        public IEnumerator STATE_NEW_001_003_TwoClicksDeleteResetAndClearSession()
        {
            WriteValidSave();
            StartStaleSession();
            SetGame("DuelsWon", 27);
            Button button = MenuButton("Nueva Partida");
            button.onClick.Invoke();
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(SceneTestDriver.BootPath));
            Assert.That(Session("HasActiveDuel"), Is.True);
            button.onClick.Invoke();
            yield return SceneTestDriver.WaitForActiveScene(SceneTestDriver.OpenWorldPath,
                () => "New Game did not navigate to OpenWorld.");
            yield return SceneTestDriver.WaitForReady(
                "ModuleZ.OpenWorld.Runtime.OpenWorldSceneRoot",
                () => "New Game OpenWorld root did not become Ready.");
            Assert.That(File.Exists(SavePath()), Is.False);
            Assert.That(Session("HasActiveDuel"), Is.False);
            Assert.That(Game("DuelsWon"), Is.EqualTo(0));
        }

        [UnityTest]
        public IEnumerator STATE_NEW_002_DeleteFailurePreservesStateAndMenu()
        {
            WriteValidSave();
            StartStaleSession();
            SetGame("DuelsWon", 27);
            using (new FileStream(SavePath(), FileMode.Open, FileAccess.Read, FileShare.None))
            {
                LogAssert.Expect(LogType.Warning, new Regex("No se pudo borrar la partida"));
                Button button = MenuButton("Nueva Partida");
                button.onClick.Invoke();
                button.onClick.Invoke();
                yield return null;
                Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(SceneTestDriver.BootPath));
                Assert.That(Session("HasActiveDuel"), Is.True);
                Assert.That(Game("DuelsWon"), Is.EqualTo(27));
                Assert.That(Info(), Does.Contain("No se pudo borrar"));
            }
        }

        [UnityTest]
        public IEnumerator STATE_CONTINUE_001_ValidLoadClearsSessionAndNavigates()
        {
            WriteValidSave();
            StartStaleSession();
            MenuButton("Continuar").onClick.Invoke();
            yield return SceneTestDriver.WaitForActiveScene(SceneTestDriver.OpenWorldPath,
                () => "Continue did not navigate to OpenWorld.");
            yield return SceneTestDriver.WaitForReady(
                "ModuleZ.OpenWorld.Runtime.OpenWorldSceneRoot",
                () => "Continue OpenWorld root did not become Ready.");
            Assert.That(Session("HasActiveDuel"), Is.False);
            Assert.That(Game("DuelsWon"), Is.EqualTo(2));
        }

        [UnityTest]
        public IEnumerator STATE_CONTINUE_002_MissingSavePreservesStateAndMenu()
        {
            StartStaleSession();
            yield return AssertContinueRejected("No hay partida guardada");
        }

        [UnityTest]
        public IEnumerator STATE_CONTINUE_003_004_InvalidSavePreservesStateAndMenu()
        {
            Directory.CreateDirectory(isolatedRoot);
            File.WriteAllText(SavePath(), "{invalid");
            StartStaleSession();
            yield return AssertContinueRejected("No se pudo cargar");
            File.WriteAllText(SavePath(),
                "{\"duelsWon\":-1,\"duelsLost\":0,\"duelsAbandoned\":0}");
            LogAssert.Expect(LogType.Warning, new Regex("SaveData inválido"));
            yield return AssertContinueRejected("No se pudo cargar");
        }

        [UnityTest]
        public IEnumerator STATE_CONTINUE_005_ReadErrorPreservesStateAndMenu()
        {
            WriteValidSave();
            StartStaleSession();
            using (new FileStream(SavePath(), FileMode.Open, FileAccess.Read, FileShare.None))
            {
                LogAssert.Expect(LogType.Warning, new Regex("No se pudo leer la partida"));
                yield return AssertContinueRejected("No se pudo cargar");
            }
        }

        private IEnumerator AssertContinueRejected(string message)
        {
            var game = Current(RuntimeContractProbe.RequireType(GameStateName));
            var session = Current(RuntimeContractProbe.RequireType(SessionName));
            MenuButton("Continuar").onClick.Invoke();
            yield return null;
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(SceneTestDriver.BootPath));
            Assert.That(Info(), Does.Contain(message));
            AssertSame(game);
            AssertSame(session);
        }

        private static IEnumerator WaitForMenu()
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < deadline)
            {
                if (GameObject.Find("Button_Nueva Partida") != null &&
                    GameObject.Find("Button_Continuar") != null)
                    yield break;
                yield return null;
            }
            Assert.Fail("Boot MainMenu did not become operational.");
        }

        private static Button MenuButton(string name)
        {
            GameObject button = GameObject.Find("Button_" + name);
            Assert.That(button, Is.Not.Null, name);
            return button.GetComponent<Button>();
        }

        private static string Info() => GameObject.Find("Info_Text")
            .GetComponent<Text>().text;

        private void WriteValidSave()
        {
            Directory.CreateDirectory(isolatedRoot);
            File.WriteAllText(SavePath(),
                "{\"duelsWon\":2,\"duelsLost\":1,\"duelsAbandoned\":0," +
                "\"currentOpenWorldTheme\":0}");
        }

        private string SavePath() => Path.Combine(isolatedRoot, "modulez_save.json");

        private static void StartStaleSession() => RuntimeContractProbe.InvokePublicStatic(
            RuntimeContractProbe.RequireType(SessionName), "StartDuel",
            Enum.ToObject(RuntimeContractProbe.RequireType(
                "ModuleZ.OpenWorld.Encounters.ModuleZRivalId"), 1), true, Vector3.one);

        private static object Session(string name) => RuntimeContractProbe.GetPublicStaticMember(
            RuntimeContractProbe.RequireType(SessionName), name);

        private static object Game(string name) => RuntimeContractProbe.GetPublicStaticMember(
            RuntimeContractProbe.RequireType(GameStateName), name);

        private static void SetGame(string name, object value) =>
            RuntimeContractProbe.RequireType(GameStateName)
                .GetField(name, BindingFlags.Public | BindingFlags.Static).SetValue(null, value);

        private static void Snapshot(Type type, Dictionary<FieldInfo, object> target)
        {
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
                target.Add(field, field.GetValue(null));
        }

        private static Dictionary<FieldInfo, object> Current(Type type)
        {
            var values = new Dictionary<FieldInfo, object>();
            Snapshot(type, values);
            return values;
        }

        private static void Restore(Dictionary<FieldInfo, object> values)
        {
            foreach (var item in values)
                item.Key.SetValue(null, item.Value);
        }

        private static void RestoreSession(Dictionary<FieldInfo, object> values)
        {
            Type type = RuntimeContractProbe.RequireType(SessionName);
            if ((bool)values[type.GetField("HasActiveDuel")])
                RuntimeContractProbe.InvokePublicStatic(type, "StartDuel",
                    values[type.GetField("RivalId")],
                    values[type.GetField("IsRematch")],
                    values[type.GetField("ReturnPosition")]);
            else RuntimeContractProbe.InvokePublicStatic(type, "Clear");
        }

        private static void AssertSame(Dictionary<FieldInfo, object> values)
        {
            foreach (var item in values)
                Assert.That(item.Key.GetValue(null), Is.EqualTo(item.Value), item.Key.Name);
        }
    }
}
