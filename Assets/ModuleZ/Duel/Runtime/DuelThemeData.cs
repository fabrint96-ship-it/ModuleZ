using UnityEngine;

namespace ModuleZ.Duel.Runtime
{
    [System.Serializable]
    public class DuelThemeData
    {
        public ModuleZDuelThemeId themeId;
        public string themeName;

        public Color primaryColor;
        public Color secondaryColor;
        public Color accentColor;

        public float duelTime;

        public string introMessage;
        public string victoryMessage;
        public string defeatMessage;

        public string musicResourcePath;
    }
}