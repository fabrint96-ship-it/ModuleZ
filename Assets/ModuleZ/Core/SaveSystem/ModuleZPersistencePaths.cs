using System;
using System.IO;
using UnityEngine;

namespace ModuleZ.Core.SaveSystem
{
    public static class ModuleZPersistencePaths
    {
        private static string rootOverride;

        public static string RootPath =>
            string.IsNullOrEmpty(rootOverride)
                ? Application.persistentDataPath
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
    }
}
