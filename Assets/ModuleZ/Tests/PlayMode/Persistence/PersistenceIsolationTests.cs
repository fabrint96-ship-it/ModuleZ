using System;
using System.IO;
using System.Security.Cryptography;
using ModuleZ.Tests.PlayMode.Support;
using NUnit.Framework;
using UnityEngine;

namespace ModuleZ.Tests.PlayMode.Persistence
{
    public sealed class PersistenceIsolationTests
    {
        private const string PathsTypeName =
            "ModuleZ.Core.SaveSystem.ModuleZPersistencePaths";
        private const string SaveManagerTypeName =
            "ModuleZ.Core.SaveSystem.ModuleZSaveManager";
        private const string SettingsManagerTypeName =
            "ModuleZ.Core.Settings.ModuleZSettingsManager";

        private Type pathsType;
        private string isolatedRoot;

        [SetUp]
        public void SetUp()
        {
            pathsType = RuntimeContractProbe.RequireType(PathsTypeName);
            RuntimeContractProbe.InvokePublicStatic(pathsType, "ClearRootOverride");
            isolatedRoot = Path.Combine(
                Path.GetTempPath(),
                "ModuleZValidation",
                "Persistence",
                Guid.NewGuid().ToString("N")
            );
        }

        [TearDown]
        public void TearDown()
        {
            RuntimeContractProbe.InvokePublicStatic(pathsType, "ClearRootOverride");
            if (Directory.Exists(isolatedRoot))
                Directory.Delete(isolatedRoot, true);
        }

        [Test]
        public void PERSIST_001_DefaultRootAndPERSIST_007_ClearRestoreProductionRoot()
        {
            Assert.That(CurrentRoot(), Is.EqualTo(Application.persistentDataPath));

            SetOverride();
            Assert.That(CurrentRoot(), Is.EqualTo(Path.GetFullPath(isolatedRoot)));

            RuntimeContractProbe.InvokePublicStatic(pathsType, "ClearRootOverride");
            Assert.That(CurrentRoot(), Is.EqualTo(Application.persistentDataPath));
        }

        [Test]
        public void PERSIST_002_003_004_SaveGameWritesOnlyIsolatedRoot()
        {
            FileFingerprint realBefore = Fingerprint(RealPath("modulez_save.json"));
            SetOverride();

            Type saveManager = RuntimeContractProbe.RequireType(SaveManagerTypeName);
            RuntimeContractProbe.InvokePublicStatic(saveManager, "SaveGame");

            string isolatedSave = Path.Combine(isolatedRoot, "modulez_save.json");
            Assert.That(File.Exists(isolatedSave), Is.True);
            Assert.That(new FileInfo(isolatedSave).Length, Is.GreaterThan(0));
            Assert.That(Fingerprint(RealPath("modulez_save.json")), Is.EqualTo(realBefore));
        }

        [Test]
        public void PERSIST_005_006_SettingsWriteUsesOnlyIsolatedRoot()
        {
            FileFingerprint realBefore = Fingerprint(RealPath("module_z_settings.json"));
            SetOverride();

            Type settingsManager = RuntimeContractProbe.RequireType(SettingsManagerTypeName);
            RuntimeContractProbe.InvokePublicStatic(settingsManager, "SaveSettings");

            string isolatedSettings = Path.Combine(isolatedRoot, "module_z_settings.json");
            Assert.That(File.Exists(isolatedSettings), Is.True);
            Assert.That(new FileInfo(isolatedSettings).Length, Is.GreaterThan(0));
            Assert.That(Fingerprint(RealPath("module_z_settings.json")), Is.EqualTo(realBefore));
        }

        [Test]
        public void PERSIST_008_OverrideDoesNotLeakBetweenTests()
        {
            Assert.That(CurrentRoot(), Is.EqualTo(Application.persistentDataPath));
            Assert.That(Directory.Exists(isolatedRoot), Is.False);
        }

        [Test]
        public void PERSIST_009_010_SaveOperationsHonorIsolatedRoot()
        {
            FileFingerprint realBefore = Fingerprint(RealPath("modulez_save.json"));
            SetOverride();
            Type saveManager = RuntimeContractProbe.RequireType(SaveManagerTypeName);

            RuntimeContractProbe.InvokePublicStatic(saveManager, "SaveGame");
            Assert.That(
                RuntimeContractProbe.InvokePublicStatic(saveManager, "HasSave"),
                Is.True
            );
            Assert.That(
                RuntimeContractProbe.InvokePublicStatic(saveManager, "GetSaveData"),
                Is.Not.Null
            );

            RuntimeContractProbe.InvokePublicStatic(saveManager, "DeleteSave");
            Assert.That(
                File.Exists(Path.Combine(isolatedRoot, "modulez_save.json")),
                Is.False
            );
            Assert.That(Fingerprint(RealPath("modulez_save.json")), Is.EqualTo(realBefore));
        }

        private void SetOverride()
        {
            RuntimeContractProbe.InvokePublicStatic(
                pathsType,
                "SetRootOverride",
                isolatedRoot
            );
        }

        private string CurrentRoot()
        {
            return (string)RuntimeContractProbe.GetPublicStaticMember(
                pathsType,
                "RootPath"
            );
        }

        private static string RealPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        private static FileFingerprint Fingerprint(string path)
        {
            if (!File.Exists(path))
                return new FileFingerprint(false, 0L, DateTime.MinValue, string.Empty);

            FileInfo file = new FileInfo(path);
            using (FileStream stream = File.OpenRead(path))
            using (SHA256 sha = SHA256.Create())
            {
                return new FileFingerprint(
                    true,
                    file.Length,
                    file.LastWriteTimeUtc,
                    BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", string.Empty)
                );
            }
        }

        private readonly struct FileFingerprint : IEquatable<FileFingerprint>
        {
            private readonly bool exists;
            private readonly long length;
            private readonly DateTime lastWriteTimeUtc;
            private readonly string sha256;

            public FileFingerprint(
                bool exists,
                long length,
                DateTime lastWriteTimeUtc,
                string sha256)
            {
                this.exists = exists;
                this.length = length;
                this.lastWriteTimeUtc = lastWriteTimeUtc;
                this.sha256 = sha256;
            }

            public bool Equals(FileFingerprint other)
            {
                return exists == other.exists &&
                    length == other.length &&
                    lastWriteTimeUtc == other.lastWriteTimeUtc &&
                    sha256 == other.sha256;
            }

            public override bool Equals(object obj)
            {
                return obj is FileFingerprint other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = exists.GetHashCode();
                    hash = (hash * 397) ^ length.GetHashCode();
                    hash = (hash * 397) ^ lastWriteTimeUtc.GetHashCode();
                    return (hash * 397) ^ (sha256?.GetHashCode() ?? 0);
                }
            }
        }
    }
}
