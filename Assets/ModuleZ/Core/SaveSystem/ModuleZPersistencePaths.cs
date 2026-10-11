using System;
using System.IO;
using UnityEngine;

namespace ModuleZ.Core.SaveSystem
{
    public static class ModuleZPersistencePaths
    {
        public const string ProcessRootEnvironmentVariable =
            "MODULEZ_PERSISTENCE_ROOT";
        public const string ValidationIsolationEnvironmentVariable =
            "MODULEZ_VALIDATION_ISOLATED";

        // Captured once: clearing a per-test override cannot redirect an
        // OnApplicationQuit save to the real profile later in this process.
        private static readonly string processRoot = ResolveProcessRoot();
        private static string rootOverride;

        public static string RootPath =>
            string.IsNullOrEmpty(rootOverride)
                ? processRoot ?? Application.persistentDataPath
                : rootOverride;

        public static void SetRootOverride(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Persistence root override is required.", nameof(path));

            rootOverride = Path.GetFullPath(path);
        }

        public static void ClearRootOverride()
        {
            rootOverride = null;
        }

        private static string ResolveProcessRoot()
        {
            string validationMarker = Environment.GetEnvironmentVariable(
                ValidationIsolationEnvironmentVariable
            );
            string configuredRoot = Environment.GetEnvironmentVariable(
                ProcessRootEnvironmentVariable
            );

            if (validationMarker != null && validationMarker != "1")
                throw new InvalidOperationException(
                    "MODULEZ_VALIDATION_ISOLATED must equal 1."
                );

            if (configuredRoot == null)
            {
                // A relaunched batchmode process that lost its inherited
                // environment must fail closed instead of saving to the profile.
                if (validationMarker != null || Application.isBatchMode)
                    throw new InvalidOperationException(
                        "Batchmode validation requires MODULEZ_PERSISTENCE_ROOT."
                    );

                return null;
            }

            if (string.IsNullOrWhiteSpace(configuredRoot))
                throw new InvalidOperationException(
                    "MODULEZ_PERSISTENCE_ROOT must be a non-empty path."
                );

            return Path.GetFullPath(configuredRoot);
        }
    }
}
