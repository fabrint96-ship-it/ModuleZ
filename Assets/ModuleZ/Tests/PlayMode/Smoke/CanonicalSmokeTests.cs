using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using ModuleZ.Tests.PlayMode.Support;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ModuleZ.Tests.PlayMode.Smoke
{
    [Category("Smoke")]
    internal sealed class CanonicalSmokeTests
    {
        private const string SmokeOptInVariable = "MODULEZ_CANONICAL_SMOKE_OPT_IN";
        private const string PathsType =
            "ModuleZ.Core.SaveSystem.ModuleZPersistencePaths";
        private const string SaveType = "ModuleZ.Core.SaveSystem.ModuleZSaveManager";
        private const string SessionType = "ModuleZ.Core.Managers.ModuleZDuelSessionState";
        private const string GameStateType = "ModuleZ.Core.Managers.ModuleZGameState";
        private const string SceneControllerType =
            "ModuleZ.Core.SceneLoading.ModuleZSceneController";
        private const string DuelStarterType =
            "ModuleZ.OpenWorld.Encounters.DuelStarterInteractable";
        private const string ResolverType =
            "ModuleZ.Duel3D.Rules.Duel3DMatchResolver";
        private const float AiTimeoutSeconds = 15f;

        private string isolatedRoot;
        private ProfileFingerprint saveBefore;
        private ProfileFingerprint settingsBefore;
        private UnityEngine.Random.State randomBefore;
        private float timeScaleBefore;
        private readonly List<string> unexpectedLogs = new List<string>();
        private GameObject bootRuntime;
        private bool setupActivated;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (Environment.GetEnvironmentVariable(SmokeOptInVariable) != "1")
                Assert.Ignore("Canonical smoke requires explicit wrapper opt-in.");

            saveBefore = Fingerprint(RealPath("modulez_save.json"));
            settingsBefore = Fingerprint(RealPath("module_z_settings.json"));
            randomBefore = UnityEngine.Random.state;
            timeScaleBefore = Time.timeScale;

            string isolationParent = IsolationParent();
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                    RuntimeContractProbe.RequireType(PathsType), "RootPath"),
                Is.EqualTo(Path.GetDirectoryName(isolationParent)),
                "Process-level isolation must be active before a per-test override.");

            isolatedRoot = Path.Combine(isolationParent, Guid.NewGuid().ToString("N"));
            Assert.That(Directory.Exists(isolatedRoot), Is.False,
                "Canonical smoke requires a fresh isolated directory.");
            RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(PathsType),
                "SetRootOverride",
                isolatedRoot
            );
            setupActivated = true;
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                    RuntimeContractProbe.RequireType(PathsType), "RootPath"),
                Is.EqualTo(Path.GetFullPath(isolatedRoot)),
                "Persistence isolation must precede Boot loading.");

            Time.timeScale = 1f;
            RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(SessionType), "Clear");
            Assert.That(RuntimeContractProbe.FindSceneComponents(
                    "ModuleZ.Core.SaveSystem.ModuleZAutoSaveController"), Is.Empty,
                "Canonical smoke requires a clean process without a pre-existing autosaver.");
            Application.logMessageReceived += RecordUnexpectedLog;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (!setupActivated)
                yield break;

            Application.logMessageReceived -= RecordUnexpectedLog;
            try
            {
                Time.timeScale = 1f;
                RuntimeContractProbe.InvokePublicStatic(
                    RuntimeContractProbe.RequireType(SessionType), "Clear");
                yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
                if (bootRuntime != null)
                    UnityEngine.Object.Destroy(bootRuntime);
                yield return null;
                yield return null;
            }
            finally
            {
                // OnApplicationQuit on Boot's DDOL autosaver runs after test teardown.
                // Never restore the real root while that component remains alive.
                bool autosaverAlive = RuntimeContractProbe.FindSceneComponents(
                    "ModuleZ.Core.SaveSystem.ModuleZAutoSaveController").Length != 0;
                if (!autosaverAlive)
                    RuntimeContractProbe.InvokePublicStatic(
                        RuntimeContractProbe.RequireType(PathsType), "ClearRootOverride");
                UnityEngine.Random.state = randomBefore;
                Time.timeScale = timeScaleBefore;

                if (!string.IsNullOrEmpty(isolatedRoot) && Directory.Exists(isolatedRoot))
                {
                    string parent = IsolationParent() + Path.DirectorySeparatorChar;
                    Assert.That(Path.GetFullPath(isolatedRoot).StartsWith(
                            parent, StringComparison.OrdinalIgnoreCase), Is.True,
                        "Smoke isolation root escaped its temporary parent.");
                    Directory.Delete(isolatedRoot, true);
                }

                Assert.That(Fingerprint(RealPath("modulez_save.json")),
                    Is.EqualTo(saveBefore), "Real save profile changed during smoke.");
                Assert.That(Fingerprint(RealPath("module_z_settings.json")),
                    Is.EqualTo(settingsBefore), "Real settings profile changed during smoke.");
                Assert.That(autosaverAlive, Is.False,
                    "Boot autosaver survived smoke teardown; persistence override was retained for safety.");
            }
        }

        [UnityTest]
        public IEnumerator SMOKE_001_CanonicalBootOpenWorldDuelReturnSaveMainMenu()
        {
            // Boot is the sole direct scene load. Every later navigation uses production actions.
            yield return SceneTestDriver.LoadSingle(SceneTestDriver.BootPath);
            bootRuntime = GameObject.Find("ModuleZ_Runtime");
            Assert.That(bootRuntime, Is.Not.Null);
            yield return WaitForMenu();
            Assert.That(Controller(), Is.Not.Null, "Boot did not establish SceneController.");
            AssertNoUnexpectedLogs();

            Button newGame = RequireMenuButton();
            newGame.onClick.Invoke();
            Assert.That(SceneManager.GetActiveScene().path,
                Is.EqualTo(SceneTestDriver.BootPath),
                "First New Game click should request confirmation.");
            newGame.onClick.Invoke();
            yield return SceneTestDriver.WaitForActiveScene(
                SceneTestDriver.OpenWorldPath, RuntimeHealthAssertions.OpenWorldDiagnostics);
            yield return SceneTestDriver.WaitForReady(
                RuntimeHealthAssertions.OpenWorldRootType,
                RuntimeHealthAssertions.OpenWorldDiagnostics);
            RuntimeHealthAssertions.AssertOpenWorldOwnership();

            Component openWorldRoot = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.OpenWorldRootType).Single();
            GameObject player = (GameObject)RuntimeContractProbe.GetPublicInstanceProperty(
                openWorldRoot, "Player");
            Assert.That(player, Is.Not.Null);
            Assert.That(player.activeInHierarchy, Is.True);

            Component madrid = RuntimeContractProbe.FindSceneComponents(DuelStarterType)
                .Single(component => component.gameObject.name == "NPC_Rival_Madrid");
            Vector3 expectedReturn = madrid.transform.position + new Vector3(0f, 0f, -3f);
            RuntimeContractProbe.InvokePublicInstance(madrid, "Interact");
            Assert.That(Session("HasActiveDuel"), Is.EqualTo(true));
            Assert.That(Session("RivalId").ToString(), Is.EqualTo("Madrid"));
            Assert.That(Session("IsRematch"), Is.EqualTo(false));
            Vector3 acceptedReturn = (Vector3)Session("ReturnPosition");
            Assert.That(Mathf.Abs(acceptedReturn.x - expectedReturn.x), Is.LessThan(0.01f));
            Assert.That(Mathf.Abs(acceptedReturn.z - expectedReturn.z), Is.LessThan(0.01f));
            // The NPC's talk animation may advance its vertical position before
            // Interact captures the return fact; that accepted fact is authoritative.
            expectedReturn = acceptedReturn;

            yield return SceneTestDriver.WaitForActiveScene(
                SceneTestDriver.DuelPath, RuntimeHealthAssertions.DuelDiagnostics);
            yield return SceneTestDriver.WaitForReady(
                RuntimeHealthAssertions.DuelRootType,
                RuntimeHealthAssertions.DuelDiagnostics);
            Assert.That(Session("HasActiveDuel"), Is.EqualTo(true));
            Component duelRoot = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.DuelRootType).Single();
            Component builder = (Component)RuntimeContractProbe.GetPublicInstanceProperty(
                duelRoot, "RuntimeBuilder");
            Assert.That(builder, Is.Not.Null);
            Assert.That(builder.gameObject.name, Is.EqualTo("Duel3D_Runtime"));
            Component resolver = builder.GetComponent(RuntimeContractProbe.RequireType(ResolverType));
            Assert.That(resolver, Is.Not.Null);
            Assert.That(RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.ResultManagerType), Has.Length.EqualTo(1));
            AssertNoUnexpectedLogs(); // Includes the forbidden direct-scene fallback warning.

            int playerBefore = CubeCount(resolver, "GetPlayerCubeCount");
            int opponentBefore = CubeCount(resolver, "GetOpponentCubeCount");
            Assert.That(playerBefore, Is.Zero);
            Assert.That(opponentBefore, Is.Zero);
            Assert.That(RuntimeContractProbe.InvokePublicInstance(
                builder, "TryCommitCurrentPlayerPiece"), Is.EqualTo(true),
                "The public production player command rejected the first placement.");
            Assert.That(CubeCount(resolver, "GetPlayerCubeCount"),
                Is.GreaterThan(playerBefore), "Player board state did not change.");
            Assert.That(PlacedVisualCount(builder.transform, "Cube_Player_"),
                Is.GreaterThan(0), "Player piece visuals were not created.");

            UnityEngine.Random.InitState(1977);
            float aiDeadline = Time.realtimeSinceStartup + AiTimeoutSeconds;
            while (CubeCount(resolver, "GetOpponentCubeCount") == opponentBefore &&
                   !(bool)RuntimeContractProbe.GetPublicInstanceProperty(
                       resolver, "MatchFinished") &&
                   Time.realtimeSinceStartup < aiDeadline)
                yield return null;

            Assert.That(CubeCount(resolver, "GetOpponentCubeCount"),
                Is.GreaterThan(opponentBefore),
                "No real AI placement observed. " + RuntimeHealthAssertions.DuelDiagnostics());
            Assert.That(PlacedVisualCount(builder.transform, "Cube_Opponent_"),
                Is.GreaterThan(0));
            Assert.That((bool)RuntimeContractProbe.GetPublicInstanceProperty(
                resolver, "MatchFinished"), Is.False,
                "The Duel ended before controlled abandon.");

            int abandonedBefore = (int)GameState("DuelsAbandoned");
            Component resultManager = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.ResultManagerType).Single();
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                    RuntimeContractProbe.RequireType(RuntimeHealthAssertions.ResultManagerType),
                    "Instance"), Is.SameAs(resultManager));
            RuntimeContractProbe.InvokePublicInstance(resultManager, "AbandonDuel");
            Assert.That(GameState("DuelWasAbandoned"), Is.EqualTo(true));
            Assert.That(GameState("DuelCompleted"), Is.EqualTo(true));
            Assert.That((int)GameState("DuelsAbandoned"), Is.EqualTo(abandonedBefore + 1));
            AssertSessionCleared();
            AssertIsolatedSave(); // Result processing saves before delayed navigation.

            yield return SceneTestDriver.WaitForActiveScene(
                SceneTestDriver.OpenWorldPath, RuntimeHealthAssertions.OpenWorldDiagnostics);
            yield return SceneTestDriver.WaitForReady(
                RuntimeHealthAssertions.OpenWorldRootType,
                RuntimeHealthAssertions.OpenWorldDiagnostics);
            RuntimeHealthAssertions.AssertOpenWorldOwnership();
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                    RuntimeContractProbe.RequireType(RuntimeHealthAssertions.ResultManagerType),
                    "Instance"), Is.Null, "ResultManager.Instance survived Duel teardown.");
            AssertSessionCleared();

            openWorldRoot = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.OpenWorldRootType).Single();
            player = (GameObject)RuntimeContractProbe.GetPublicInstanceProperty(
                openWorldRoot, "Player");
            Assert.That(player, Is.Not.Null);
            Assert.That(Mathf.Abs(player.transform.position.x - expectedReturn.x),
                Is.LessThan(0.75f));
            Assert.That(Mathf.Abs(player.transform.position.z - expectedReturn.z),
                Is.LessThan(0.75f));
            yield return WaitForPendingMessageConsumption();

            RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(SaveType), "SaveGame");
            AssertIsolatedSave();
            AssertRealProfileUnchanged();

            RuntimeContractProbe.InvokePublicInstance(Controller(), "ReturnToMainMenu");
            yield return SceneTestDriver.WaitForActiveScene(
                SceneTestDriver.BootPath, MenuDiagnostics);
            yield return WaitForMenu();
            AssertSessionCleared();
            Assert.That(RuntimeContractProbe.GetPublicStaticMember(
                    RuntimeContractProbe.RequireType(RuntimeHealthAssertions.ResultManagerType),
                    "Instance"), Is.Null);
            AssertIsolatedSave();
            AssertNoUnexpectedLogs();
        }

        private void RecordUnexpectedLog(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Warning || type == LogType.Error ||
                type == LogType.Exception || type == LogType.Assert)
                unexpectedLogs.Add($"{type}: {message}");
        }

        private void AssertNoUnexpectedLogs()
        {
            Assert.That(unexpectedLogs, Is.Empty,
                "Canonical production path emitted unexpected warnings/errors: " +
                string.Join(" | ", unexpectedLogs));
        }

        private static IEnumerator WaitForMenu()
        {
            float deadline = Time.realtimeSinceStartup + 20f;
            while (Time.realtimeSinceStartup < deadline)
            {
                GameObject button = GameObject.Find("Button_Nueva Partida");
                if (button != null && button.activeInHierarchy &&
                    button.GetComponent<Button>() != null && Controller() != null)
                    yield break;
                yield return null;
            }
            Assert.Fail("MainMenu did not become operational. " + MenuDiagnostics());
        }

        private static Button RequireMenuButton()
        {
            GameObject button = GameObject.Find("Button_Nueva Partida");
            Assert.That(button, Is.Not.Null, MenuDiagnostics());
            Button component = button.GetComponent<Button>();
            Assert.That(component, Is.Not.Null, MenuDiagnostics());
            return component;
        }

        private static object Controller()
        {
            return RuntimeContractProbe.GetPublicStaticMember(
                RuntimeContractProbe.RequireType(SceneControllerType), "Instance");
        }

        private static string MenuDiagnostics()
        {
            return $"scene={SceneManager.GetActiveScene().path}; " +
                $"button={(GameObject.Find("Button_Nueva Partida") != null)}; " +
                $"controller={(Controller() != null)}.";
        }

        private static object Session(string member)
        {
            return RuntimeContractProbe.GetPublicStaticMember(
                RuntimeContractProbe.RequireType(SessionType), member);
        }

        private static object GameState(string member)
        {
            return RuntimeContractProbe.GetPublicStaticMember(
                RuntimeContractProbe.RequireType(GameStateType), member);
        }

        private static int CubeCount(Component resolver, string method)
        {
            return (int)RuntimeContractProbe.InvokePublicInstance(resolver, method);
        }

        private static int PlacedVisualCount(Transform runtime, string prefix)
        {
            Transform cubes = runtime.Find("Duel3D_CubesRoot");
            Assert.That(cubes, Is.Not.Null);
            return cubes.Cast<Transform>().Count(child =>
                child.name.StartsWith(prefix, StringComparison.Ordinal));
        }

        private static void AssertSessionCleared()
        {
            Assert.That(Session("HasActiveDuel"), Is.EqualTo(false));
            Assert.That(Session("IsRematch"), Is.EqualTo(false));
            Assert.That((Vector3)Session("ReturnPosition"), Is.EqualTo(Vector3.zero));
            Assert.That(Session("RivalId"), Is.EqualTo(
                Activator.CreateInstance(RuntimeContractProbe.RequireType(
                    "ModuleZ.OpenWorld.Encounters.ModuleZRivalId"))));
        }

        private static IEnumerator WaitForPendingMessageConsumption()
        {
            float deadline = Time.realtimeSinceStartup + 5f;
            while (!string.IsNullOrEmpty((string)GameState("PendingOpenWorldMessage")) &&
                   Time.realtimeSinceStartup < deadline)
                yield return null;
            Assert.That(GameState("PendingOpenWorldMessage"), Is.EqualTo(string.Empty),
                "The return result message was not consumed.");
            Assert.That(GameState("ReturningFromDuel"), Is.EqualTo(false));
        }

        private void AssertIsolatedSave()
        {
            string path = Path.Combine(isolatedRoot, "modulez_save.json");
            Assert.That(File.Exists(path), Is.True, "No isolated save was written.");
            string json = File.ReadAllText(path);
            Assert.That(json.Length, Is.GreaterThan(2));
            Assert.That(json.TrimStart().StartsWith("{", StringComparison.Ordinal), Is.True);
            Assert.That(JsonUtility.FromJson(json, RuntimeContractProbe.RequireType(
                "ModuleZ.Core.SaveSystem.ModuleZSaveData")), Is.Not.Null,
                "Isolated save is not valid ModuleZ save JSON.");
        }

        private void AssertRealProfileUnchanged()
        {
            Assert.That(Fingerprint(RealPath("modulez_save.json")),
                Is.EqualTo(saveBefore), "Real save profile changed.");
            Assert.That(Fingerprint(RealPath("module_z_settings.json")),
                Is.EqualTo(settingsBefore), "Real settings profile changed.");
        }

        private static string IsolationParent()
        {
            Assert.That(Environment.GetEnvironmentVariable(
                "MODULEZ_VALIDATION_ISOLATED"), Is.EqualTo("1"),
                "Canonical smoke must run through the isolated validation wrapper.");

            string configuredRoot = Environment.GetEnvironmentVariable(
                "MODULEZ_PERSISTENCE_ROOT");
            Assert.That(string.IsNullOrWhiteSpace(configuredRoot), Is.False,
                "Canonical smoke requires a process-level persistence root.");

            string processRoot = Path.GetFullPath(configuredRoot);
            string realRoot = Path.GetFullPath(Application.persistentDataPath);
            Assert.That(processRoot.Equals(realRoot, StringComparison.OrdinalIgnoreCase) ||
                processRoot.StartsWith(realRoot + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase) ||
                realRoot.StartsWith(processRoot + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase), Is.False,
                "Canonical smoke storage overlaps the real profile.");

            return Path.Combine(processRoot, "canonical-smoke");
        }

        private static string RealPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        private static ProfileFingerprint Fingerprint(string path)
        {
            if (!File.Exists(path))
                return new ProfileFingerprint(false, 0, DateTime.MinValue, string.Empty);

            FileInfo info = new FileInfo(path);
            using (FileStream stream = File.OpenRead(path))
            using (SHA256 sha = SHA256.Create())
            {
                string hash = BitConverter.ToString(sha.ComputeHash(stream))
                    .Replace("-", string.Empty);
                return new ProfileFingerprint(true, info.Length, info.LastWriteTimeUtc, hash);
            }
        }

        private readonly struct ProfileFingerprint : IEquatable<ProfileFingerprint>
        {
            private readonly bool exists;
            private readonly long size;
            private readonly DateTime lastWriteUtc;
            private readonly string sha256;

            public ProfileFingerprint(bool exists, long size, DateTime lastWriteUtc,
                string sha256)
            {
                this.exists = exists;
                this.size = size;
                this.lastWriteUtc = lastWriteUtc;
                this.sha256 = sha256;
            }

            public bool Equals(ProfileFingerprint other)
            {
                return exists == other.exists && size == other.size &&
                    lastWriteUtc == other.lastWriteUtc && sha256 == other.sha256;
            }

            public override bool Equals(object value)
            {
                return value is ProfileFingerprint other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = exists.GetHashCode();
                    hash = (hash * 397) ^ size.GetHashCode();
                    hash = (hash * 397) ^ lastWriteUtc.GetHashCode();
                    return (hash * 397) ^ (sha256?.GetHashCode() ?? 0);
                }
            }
        }
    }
}
