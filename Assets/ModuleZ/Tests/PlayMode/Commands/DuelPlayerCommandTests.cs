using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using ModuleZ.Tests.PlayMode.Support;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ModuleZ.Tests.PlayMode.Commands
{
    internal sealed class DuelPlayerCommandTests
    {
        private const string BuilderTypeName =
            "ModuleZ.Duel3D.Runtime.Duel3DRuntimeBuilder";
        private const string ResolverTypeName =
            "ModuleZ.Duel3D.Rules.Duel3DMatchResolver";
        private const string PersistencePathsTypeName =
            "ModuleZ.Core.SaveSystem.ModuleZPersistencePaths";
        private const float AiTimeoutSeconds = 15f;

        private string isolatedRoot;
        private GameObject unbuiltRuntime;
        private FileFingerprint realSaveBefore;
        private FileFingerprint realSettingsBefore;
        private UnityEngine.Random.State originalRandomState;
        private float originalTimeScale;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            realSaveBefore = Fingerprint(RealPath("modulez_save.json"));
            realSettingsBefore = Fingerprint(RealPath("module_z_settings.json"));
            originalRandomState = UnityEngine.Random.state;
            originalTimeScale = Time.timeScale;

            isolatedRoot = Path.Combine(
                IsolationParent(),
                Guid.NewGuid().ToString("N")
            );
            RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(PersistencePathsTypeName),
                "SetRootOverride",
                isolatedRoot
            );
            Assert.That(
                RuntimeContractProbe.GetPublicStaticMember(
                    RuntimeContractProbe.RequireType(PersistencePathsTypeName),
                    "RootPath"
                ),
                Is.EqualTo(Path.GetFullPath(isolatedRoot))
            );

            Time.timeScale = 1f;
            RuntimeContractProbe.InvokePublicStatic(
                RuntimeContractProbe.RequireType(
                    "ModuleZ.Core.Managers.ModuleZDuelSessionState"
                ),
                "Clear"
            );
            yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            try
            {
                Time.timeScale = 1f;
                if (unbuiltRuntime != null)
                    UnityEngine.Object.Destroy(unbuiltRuntime);
                yield return SceneTestDriver.DestroyLoadedProductionSceneRoots();
            }
            finally
            {
                RuntimeContractProbe.InvokePublicStatic(
                    RuntimeContractProbe.RequireType(PersistencePathsTypeName),
                    "ClearRootOverride"
                );
                UnityEngine.Random.state = originalRandomState;
                Time.timeScale = originalTimeScale;

                if (!string.IsNullOrEmpty(isolatedRoot) &&
                    Directory.Exists(isolatedRoot))
                {
                    string parent = IsolationParent() + Path.DirectorySeparatorChar;
                    Assert.That(
                        Path.GetFullPath(isolatedRoot).StartsWith(
                            parent,
                            StringComparison.OrdinalIgnoreCase
                        ),
                        Is.True,
                        "Isolated Duel command root escaped its temporary parent."
                    );
                    Directory.Delete(isolatedRoot, true);
                }

                Assert.That(Fingerprint(RealPath("modulez_save.json")),
                    Is.EqualTo(realSaveBefore),
                    "The real ModuleZ save profile changed during command validation.");
                Assert.That(Fingerprint(RealPath("module_z_settings.json")),
                    Is.EqualTo(realSettingsBefore),
                    "The real ModuleZ settings profile changed during command validation.");
            }
        }

        [Test]
        public void CMD_001_009_PublicPlayerCommandExistsAndAiTurnRemainsPrivate()
        {
            Type builderType = RuntimeContractProbe.RequireType(BuilderTypeName);
            MethodInfo command = RequireCommand(builderType);

            Assert.That(command.ReturnType, Is.EqualTo(typeof(bool)));
            Assert.That(command.GetParameters(), Is.Empty);
            Assert.That(builderType.GetMethod(
                "PlayOpponentTurn",
                BindingFlags.Public | BindingFlags.Instance
            ), Is.Null);
        }

        [UnityTest]
        public IEnumerator CMD_002_UnbuiltCommandRejectsWithoutCreatingGameplayState()
        {
            Type builderType = RuntimeContractProbe.RequireType(BuilderTypeName);
            unbuiltRuntime = new GameObject("UnbuiltDuelCommandProbe");

            Component builder = unbuiltRuntime.AddComponent(builderType);
            Assert.That(InvokeCommand(builder), Is.False);
            Assert.That(unbuiltRuntime.transform.childCount, Is.Zero);
            Assert.That(unbuiltRuntime.GetComponent(
                RuntimeContractProbe.RequireType(ResolverTypeName)
            ), Is.Null);

            UnityEngine.Object.Destroy(unbuiltRuntime);
            yield return null;
            unbuiltRuntime = null;
        }

        [UnityTest]
        public IEnumerator CMD_003_004_005_006_007_ReadyCommandPlacesPieceAndAiResponds()
        {
            LogAssert.Expect(
                LogType.Warning,
                "[ModuleZ] DuelBootstrap using transitional direct-scene fallback."
            );
            yield return SceneTestDriver.LoadSingle(SceneTestDriver.DuelPath);
            yield return SceneTestDriver.WaitForReady(
                RuntimeHealthAssertions.DuelRootType,
                RuntimeHealthAssertions.DuelDiagnostics
            );

            Component root = RuntimeContractProbe.FindSceneComponents(
                RuntimeHealthAssertions.DuelRootType
            ).Single();
            Component builder = (Component)RuntimeContractProbe.GetPublicInstanceProperty(
                root,
                "RuntimeBuilder"
            );
            Component resolver = builder.GetComponent(
                RuntimeContractProbe.RequireType(ResolverTypeName)
            );
            Assert.That(builder, Is.Not.Null);
            Assert.That(resolver, Is.Not.Null);

            int playerBefore = CubeCount(resolver, "GetPlayerCubeCount");
            int opponentBefore = CubeCount(resolver, "GetOpponentCubeCount");
            Assert.That(playerBefore, Is.Zero);
            Assert.That(opponentBefore, Is.Zero);

            Assert.That(InvokeCommand(builder), Is.True);
            Assert.That(CubeCount(resolver, "GetPlayerCubeCount"),
                Is.GreaterThan(playerBefore),
                "The accepted command did not change player board state.");
            Assert.That(PlacedVisualCount(builder.transform, "Cube_Player_"),
                Is.GreaterThan(0),
                "The accepted command did not create player visuals.");

            int playerAfter = CubeCount(resolver, "GetPlayerCubeCount");
            Assert.That(InvokeCommand(builder), Is.False,
                "The player command was accepted again before the AI turn.");
            Assert.That(CubeCount(resolver, "GetPlayerCubeCount"),
                Is.EqualTo(playerAfter),
                "A rejected second command mutated player board state.");

            // The production AI consumes Unity Random while ranking moves.
            // Pin its input here and restore the prior state during teardown.
            UnityEngine.Random.InitState(1977);
            float deadline = Time.realtimeSinceStartup + AiTimeoutSeconds;
            while (CubeCount(resolver, "GetOpponentCubeCount") == opponentBefore &&
                   !MatchFinished(resolver) &&
                   Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }

            Assert.That(CubeCount(resolver, "GetOpponentCubeCount"),
                Is.GreaterThan(opponentBefore),
                "The production AI did not place a piece. " +
                $"matchFinished={MatchFinished(resolver)}; " +
                RuntimeHealthAssertions.DuelDiagnostics());
            Assert.That(PlacedVisualCount(builder.transform, "Cube_Opponent_"),
                Is.GreaterThan(0));

            Assert.That(MatchFinished(resolver), Is.False,
                "The Duel ended before the normal player turn resumed.");
            float previewDeadline = Time.realtimeSinceStartup + 5f;
            Transform previewRoot = builder.transform.Find("Duel3D_PreviewRoot");
            Assert.That(previewRoot, Is.Not.Null);
            while (previewRoot.childCount == 0 &&
                   Time.realtimeSinceStartup < previewDeadline)
                yield return null;
            Assert.That(previewRoot.childCount, Is.GreaterThan(0),
                "The player's next-turn preview did not resume after the AI action.");
        }

        [Test]
        public void CMD_008_LegacySpaceDelegatesToCanonicalCommand()
        {
            string source = File.ReadAllText(Path.Combine(
                Application.dataPath,
                "ModuleZ",
                "Duel3D",
                "Runtime",
                "Duel3DRuntimeBuilder.cs"
            ));
            Assert.That(Regex.IsMatch(
                source,
                @"if\s*\(\s*Input\.GetKeyDown\(KeyCode\.Space\)\s*\)\s*TryCommitCurrentPlayerPiece\s*\(\s*\)\s*;"
            ), Is.True);
            Assert.That(source.Contains("TryPlacePlayerPiece"), Is.False);
        }

        private static MethodInfo RequireCommand(Type builderType)
        {
            MethodInfo method = builderType.GetMethod(
                "TryCommitCurrentPlayerPiece",
                BindingFlags.Public | BindingFlags.Instance
            );
            Assert.That(method, Is.Not.Null);
            return method;
        }

        private static bool InvokeCommand(Component builder)
        {
            return (bool)RequireCommand(builder.GetType()).Invoke(builder, null);
        }

        private static int CubeCount(Component resolver, string methodName)
        {
            MethodInfo method = resolver.GetType().GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Instance
            );
            Assert.That(method, Is.Not.Null);
            return (int)method.Invoke(resolver, null);
        }

        private static bool MatchFinished(Component resolver)
        {
            return (bool)RuntimeContractProbe.GetPublicInstanceProperty(
                resolver,
                "MatchFinished"
            );
        }

        private static int PlacedVisualCount(Transform runtime, string prefix)
        {
            Transform cubesRoot = runtime.Find("Duel3D_CubesRoot");
            Assert.That(cubesRoot, Is.Not.Null);
            return cubesRoot.Cast<Transform>().Count(
                child => child.name.StartsWith(prefix, StringComparison.Ordinal)
            );
        }

        private static string IsolationParent()
        {
            return Path.GetFullPath(Path.Combine(
                Path.GetTempPath(),
                "ModuleZValidation",
                "DuelCommand"
            ));
        }

        private static string RealPath(string name)
        {
            return Path.Combine(Application.persistentDataPath, name);
        }

        private static FileFingerprint Fingerprint(string path)
        {
            if (!File.Exists(path))
                return new FileFingerprint(false, 0, DateTime.MinValue, string.Empty);

            FileInfo info = new FileInfo(path);
            using (FileStream stream = File.OpenRead(path))
            using (SHA256 sha = SHA256.Create())
            {
                string hash = BitConverter.ToString(sha.ComputeHash(stream))
                    .Replace("-", string.Empty);
                return new FileFingerprint(true, info.Length,
                    info.LastWriteTimeUtc, hash);
            }
        }

        private readonly struct FileFingerprint : IEquatable<FileFingerprint>
        {
            private readonly bool exists;
            private readonly long length;
            private readonly DateTime lastWriteUtc;
            private readonly string sha256;

            public FileFingerprint(bool exists, long length,
                DateTime lastWriteUtc, string sha256)
            {
                this.exists = exists;
                this.length = length;
                this.lastWriteUtc = lastWriteUtc;
                this.sha256 = sha256;
            }

            public bool Equals(FileFingerprint other)
            {
                return exists == other.exists && length == other.length &&
                    lastWriteUtc == other.lastWriteUtc && sha256 == other.sha256;
            }

            public override bool Equals(object value)
            {
                return value is FileFingerprint other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = exists.GetHashCode();
                    hash = (hash * 397) ^ length.GetHashCode();
                    hash = (hash * 397) ^ lastWriteUtc.GetHashCode();
                    return (hash * 397) ^ (sha256?.GetHashCode() ?? 0);
                }
            }
        }
    }
}
