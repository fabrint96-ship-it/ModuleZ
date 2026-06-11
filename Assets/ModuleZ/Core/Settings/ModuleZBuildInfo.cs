namespace ModuleZ.Core.Settings
{
    public static class ModuleZBuildInfo
    {
        public const string GameName = "Module Z";
        public const string Version = "v0.1.0";
        public const string BuildName = "Prototype";

        public static string FullVersion =>
            GameName + " • " + Version + " • " + BuildName;
    }
}