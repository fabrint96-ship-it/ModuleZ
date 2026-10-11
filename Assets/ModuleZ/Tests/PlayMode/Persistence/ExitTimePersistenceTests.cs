using System;
using System.IO;
using ModuleZ.Tests.PlayMode.Support;
using NUnit.Framework;
using UnityEngine;

namespace ModuleZ.Tests.PlayMode.Persistence
{
    // Run only via the dedicated shutdown stage, in its own Unity process.
    // The DDOL host intentionally survives the test so the production
    // OnApplicationQuit callback exercises process-lifetime path isolation.
    [Category("Shutdown")]
    internal sealed class ExitTimePersistenceTests
    {
        [Test]
        public void PERSIST_EXIT_001_AutosaverUsesProcessRootAtUnityExit()
        {
            string configuredRoot = Environment.GetEnvironmentVariable(
                "MODULEZ_PERSISTENCE_ROOT"
            );
            Assert.That(configuredRoot, Is.Not.Null.And.Not.Empty,
                "The shutdown stage must establish process-level isolation first.");

            string processRoot = Path.GetFullPath(configuredRoot);
            string activeRoot = (string)RuntimeContractProbe.GetPublicStaticMember(
                RuntimeContractProbe.RequireType(
                    "ModuleZ.Core.SaveSystem.ModuleZPersistencePaths"),
                "RootPath"
            );
            Assert.That(activeRoot, Is.EqualTo(processRoot));
            Assert.That(activeRoot, Is.Not.EqualTo(Application.persistentDataPath));
            Assert.That(File.Exists(Path.Combine(processRoot, "modulez_save.json")),
                Is.False, "The shutdown regression requires a fresh process root.");

            GameObject host = new GameObject("ExitTimeAutosaveProbe");
            host.AddComponent(RuntimeContractProbe.RequireType(
                "ModuleZ.Core.SaveSystem.ModuleZAutoSaveController"));
            UnityEngine.Object.DontDestroyOnLoad(host);

            // Wrapper checks the isolated write and real profile only after
            // this Unity process has exited; an in-test assertion is too early.
        }
    }
}
