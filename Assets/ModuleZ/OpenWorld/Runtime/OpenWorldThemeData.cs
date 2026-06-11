using UnityEngine;

namespace ModuleZ.OpenWorld.Runtime
{
    [System.Serializable]
    public class OpenWorldThemeData
    {
        public OpenWorldThemeId themeId;
        public string themeName;

        public Color primaryColor;
        public Color secondaryColor;
        public Color accentColor;

        public string musicResourcePath;

        public string zoneDisplayName;
        public string enterMessage;
    }
}