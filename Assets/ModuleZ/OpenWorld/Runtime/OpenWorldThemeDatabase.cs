using UnityEngine;

namespace ModuleZ.OpenWorld.Runtime
{
    public static class OpenWorldThemeDatabase
    {
        public static OpenWorldThemeData GetThemeData(OpenWorldThemeId themeId)
        {
            switch (themeId)
            {
                case OpenWorldThemeId.Madrid70s:
                    return new OpenWorldThemeData
                    {
                        themeId = themeId,
                        themeName = "Madrid 70s",
                        primaryColor = new Color(0.42f, 0.36f, 0.30f),
                        secondaryColor = new Color(0.50f, 0.47f, 0.42f),
                        accentColor = new Color(0.95f, 0.75f, 0.20f),
                        musicResourcePath = "Audio/Music/OpenWorld/MUS_OpenWorld_Madrid70s",
                        zoneDisplayName = "Madrid",
                        enterMessage = "Zona actual: Madrid años 70"
                    };

                case OpenWorldThemeId.Barcelona70s:
                    return new OpenWorldThemeData
                    {
                        themeId = themeId,
                        themeName = "Barcelona 70s",
                        primaryColor = new Color(0.78f, 0.72f, 0.62f),
                        secondaryColor = new Color(0.55f, 0.45f, 0.35f),
                        accentColor = new Color(0.20f, 0.55f, 0.85f),
                        musicResourcePath = "Audio/Music/OpenWorld/MUS_OpenWorld_Barcelona70s",
                        zoneDisplayName = "Barcelona",
                        enterMessage = "Zona actual: Barcelona años 70"
                    };

                case OpenWorldThemeId.Valencia70s:
                    return new OpenWorldThemeData
                    {
                        themeId = themeId,
                        themeName = "Valencia 70s",
                        primaryColor = new Color(0.82f, 0.74f, 0.58f),
                        secondaryColor = new Color(0.68f, 0.42f, 0.18f),
                        accentColor = new Color(0.95f, 0.55f, 0.15f),
                        musicResourcePath = "Audio/Music/OpenWorld/MUS_OpenWorld_Valencia70s",
                        zoneDisplayName = "Valencia",
                        enterMessage = "Zona actual: Valencia años 70"
                    };

                case OpenWorldThemeId.Andalucia70s:
                    return new OpenWorldThemeData
                    {
                        themeId = themeId,
                        themeName = "Andalucía 70s",
                        primaryColor = new Color(0.88f, 0.84f, 0.72f),
                        secondaryColor = new Color(0.92f, 0.90f, 0.82f),
                        accentColor = new Color(0.18f, 0.45f, 0.75f),
                        musicResourcePath = "Audio/Music/OpenWorld/MUS_OpenWorld_Andalucia70s",
                        zoneDisplayName = "Andalucía",
                        enterMessage = "Zona actual: Andalucía años 70"
                    };

                default:
                    return GetThemeData(OpenWorldThemeId.Madrid70s);
            }
        }
    }
}