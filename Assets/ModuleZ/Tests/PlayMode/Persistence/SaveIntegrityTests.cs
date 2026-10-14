using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ModuleZ.Tests.PlayMode.Support;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ModuleZ.Tests.PlayMode.Persistence
{
    // Public production contracts only: production still compiles in Assembly-CSharp.
    [Category("Persistence")]
    public sealed class SaveIntegrityTests
    {
        private const string SaveName = "modulez_save.json";
        private const string HistoricalJson =
            "{\"duelsWon\":1,\"duelsLost\":0,\"duelsAbandoned\":0}";
        private Type paths;
        private Type manager;
        private string root;
        private readonly List<AchievementSnapshot> achievements = new List<AchievementSnapshot>();

        [SetUp]
        public void SetUp()
        {
            Assert.That(Environment.GetEnvironmentVariable("MODULEZ_VALIDATION_ISOLATED"),
                Is.EqualTo("1"), "Run via the isolated validation wrapper.");
            string processRoot = Environment.GetEnvironmentVariable("MODULEZ_PERSISTENCE_ROOT");
            Assert.That(processRoot, Is.Not.Null.And.Not.Empty);
            processRoot = Path.GetFullPath(processRoot);
            string realRoot = Path.GetFullPath(Application.persistentDataPath);
            Assert.That(string.Equals(processRoot, realRoot,
                StringComparison.OrdinalIgnoreCase), Is.False);
            Assert.That(processRoot.StartsWith(realRoot + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase), Is.False);
            Assert.That(realRoot.StartsWith(processRoot + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase), Is.False);

            paths = RuntimeContractProbe.RequireType(
                "ModuleZ.Core.SaveSystem.ModuleZPersistencePaths");
            manager = RuntimeContractProbe.RequireType(
                "ModuleZ.Core.SaveSystem.ModuleZSaveManager");
            root = Path.Combine(processRoot, "save-integrity-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            RuntimeContractProbe.InvokePublicStatic(paths, "SetRootOverride", root);

            Type achievementManager = RuntimeContractProbe.RequireType(
                "ModuleZ.Core.Achievements.ModuleZAchievementManager");
            foreach (object state in (IEnumerable)RuntimeContractProbe.InvokePublicStatic(
                achievementManager, "GetAll"))
                achievements.Add(new AchievementSnapshot(state));
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                foreach (AchievementSnapshot snapshot in achievements)
                    snapshot.Restore();
            }
            finally
            {
                if (paths != null)
                    RuntimeContractProbe.InvokePublicStatic(paths, "ClearRootOverride");
                if (root != null && Directory.Exists(root))
                    Directory.Delete(root, true);
            }
        }

        [Test]
        public void PERSIST_SAFE_001_FirstSaveAndExistingVoidApi()
        {
            Assert.That(SaveStatus(), Is.EqualTo("Success"));
            Assert.That(File.Exists(Main), Is.True);
            Assert.That(File.Exists(Pending), Is.False);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(manager, "HasSave"), Is.True);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(manager, "GetSaveData"), Is.Not.Null);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(manager, "SaveGame"), Is.Null);
            Assert.That(File.Exists(Backup), Is.True);
            Assert.That(File.ReadAllText(Backup), Is.EqualTo(File.ReadAllText(Main)));
        }

        [Test]
        public void PERSIST_SAFE_002_ReplacementPreservesPreviousValidSave()
        {
            File.WriteAllText(Main, HistoricalJson);
            Assert.That(SaveStatus(), Is.EqualTo("Success"));
            Assert.That(File.ReadAllText(Backup), Is.EqualTo(HistoricalJson));
            Assert.That(File.Exists(Pending), Is.False);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(manager, "GetSaveData"), Is.Not.Null);
        }

        [Test]
        public void PERSIST_SAFE_003_TemporaryWriteFailureKeepsMainAndForeignPending()
        {
            File.WriteAllText(Main, HistoricalJson);
            Directory.CreateDirectory(Pending);
            Assert.That(SaveStatus(), Is.EqualTo("TemporaryWriteError"));
            Assert.That(File.ReadAllText(Main), Is.EqualTo(HistoricalJson));
            Assert.That(Directory.Exists(Pending), Is.True);
        }

        [Test]
        public void PERSIST_SAFE_004_PublicationFailureKeepsMainAndVerifiedPending()
        {
            File.WriteAllText(Main, HistoricalJson);
            Directory.CreateDirectory(Backup);
            Assert.That(SaveStatus(), Is.EqualTo("PublicationError"));
            Assert.That(File.ReadAllText(Main), Is.EqualTo(HistoricalJson));
            Assert.That(File.Exists(Pending), Is.True);
            Assert.That(new FileInfo(Pending).Length, Is.GreaterThan(0));
        }

        [Test]
        public void PERSIST_SAFE_005_CorruptMainReadsValidBackupWithoutPromotion()
        {
            File.WriteAllText(Main, "{not json");
            File.WriteAllText(Backup, HistoricalJson);
            LogAssert.Expect(LogType.Warning,
                "[ModuleZ] Guardado principal inválido; se leyó la copia de recuperación.");
            Assert.That(RuntimeContractProbe.InvokePublicStatic(manager, "GetSaveData"), Is.Not.Null);
            Assert.That(File.ReadAllText(Main), Is.EqualTo("{not json"));
            Assert.That(SaveStatus(), Is.EqualTo("ExistingSaveInvalid"));
            Assert.That(File.ReadAllText(Backup), Is.EqualTo(HistoricalJson));
        }

        [Test]
        public void PERSIST_SAFE_006_ValidMainTakesPriorityOverOlderBackup()
        {
            File.WriteAllText(Main, HistoricalJson);
            File.WriteAllText(Backup,
                "{\"duelsWon\":9,\"duelsLost\":0,\"duelsAbandoned\":0}");
            object data = RuntimeContractProbe.InvokePublicStatic(manager, "GetSaveData");
            FieldInfo won = data.GetType().GetField("duelsWon");
            Assert.That(won.GetValue(data), Is.EqualTo(1));
        }

        [Test]
        public void PERSIST_SAFE_007_OrphanedPendingIsNeverPromoted()
        {
            File.WriteAllText(Pending, HistoricalJson);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(manager, "HasSave"), Is.False);
            Assert.That(SaveStatus(), Is.EqualTo("TemporaryWriteError"));
            Assert.That(File.Exists(Main), Is.False);
            Assert.That(File.ReadAllText(Pending), Is.EqualTo(HistoricalJson));
        }

        [Test]
        public void PERSIST_SAFE_008_OrphanedBackupBlocksNewPublication()
        {
            File.WriteAllText(Backup, HistoricalJson);
            Assert.That(SaveStatus(), Is.EqualTo("ExistingSaveInvalid"));
            Assert.That(File.Exists(Main), Is.False);
            Assert.That(File.ReadAllText(Backup), Is.EqualTo(HistoricalJson));
        }

        [Test]
        public void PERSIST_SAFE_009_ExplicitDeleteRemovesOnlyIsolatedSaveAndSidecars()
        {
            File.WriteAllText(Main, HistoricalJson);
            File.WriteAllText(Backup, HistoricalJson);
            File.WriteAllText(Pending, HistoricalJson);
            Assert.That(RuntimeContractProbe.InvokePublicStatic(manager, "TryDeleteSave"), Is.True);
            Assert.That(File.Exists(Main) || File.Exists(Backup) || File.Exists(Pending), Is.False);
        }

        private string Main => Path.Combine(root, SaveName);
        private string Pending => Main + ".pending";
        private string Backup => Main + ".bak";

        private string SaveStatus() => RuntimeContractProbe.InvokePublicStatic(
            manager, "TrySaveGame").ToString();

        private sealed class AchievementSnapshot
        {
            private readonly object state;
            private readonly FieldInfo unlockedField;
            private readonly FieldInfo dateField;
            private readonly object unlocked;
            private readonly object date;

            internal AchievementSnapshot(object state)
            {
                this.state = state;
                unlockedField = state.GetType().GetField("unlocked");
                dateField = state.GetType().GetField("unlockedDate");
                Assert.That(unlockedField, Is.Not.Null);
                Assert.That(dateField, Is.Not.Null);
                unlocked = unlockedField.GetValue(state);
                date = dateField.GetValue(state);
            }

            internal void Restore()
            {
                unlockedField.SetValue(state, unlocked);
                dateField.SetValue(state, date);
            }
        }
    }
}
