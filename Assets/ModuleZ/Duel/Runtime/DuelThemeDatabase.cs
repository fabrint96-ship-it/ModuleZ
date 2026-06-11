using UnityEngine;

namespace ModuleZ.Duel.Runtime
{
    public static class DuelThemeDatabase
    {
        public static DuelThemeData GetThemeData(ModuleZDuelThemeId themeId)
        {
            switch (themeId)
            {
                case ModuleZDuelThemeId.Madrid70s:
                    return new DuelThemeData
                    {
                        themeId = themeId,
                        themeName = "Madrid 70s",
                        primaryColor = new Color(0.48f, 0.45f, 0.40f),
                        secondaryColor = new Color(0.35f, 0.30f, 0.25f),
                        accentColor = new Color(0.95f, 0.75f, 0.20f),
                        duelTime = 45f,
                        introMessage = "Duelo iniciado — Madrid 70s",
                        victoryMessage = "Victoria en Madrid",
                        defeatMessage = "Derrota en Madrid",
                        musicResourcePath = "Audio/Music/Duel/MUS_Duel_Madrid70s"
                    };

                case ModuleZDuelThemeId.Barcelona70s:
                    return new DuelThemeData
                    {
                        themeId = themeId,
                        themeName = "Barcelona 70s",
                        primaryColor = new Color(0.78f, 0.72f, 0.62f),
                        secondaryColor = new Color(0.55f, 0.45f, 0.35f),
                        accentColor = new Color(0.20f, 0.55f, 0.85f),
                        duelTime = 50f,
                        introMessage = "Duelo iniciado — Barcelona 70s",
                        victoryMessage = "Victoria en Barcelona",
                        defeatMessage = "Derrota en Barcelona",
                        musicResourcePath = "Audio/Music/Duel/MUS_Duel_Barcelona70s"
                    };

                case ModuleZDuelThemeId.Valencia70s:
                    return new DuelThemeData
                    {
                        themeId = themeId,
                        themeName = "Valencia 70s",
                        primaryColor = new Color(0.82f, 0.74f, 0.58f),
                        secondaryColor = new Color(0.68f, 0.42f, 0.18f),
                        accentColor = new Color(0.95f, 0.55f, 0.15f),
                        duelTime = 55f,
                        introMessage = "Duelo iniciado — Valencia 70s",
                        victoryMessage = "Victoria en Valencia",
                        defeatMessage = "Derrota en Valencia",
                        musicResourcePath = "Audio/Music/Duel/MUS_Duel_Valencia70s"
                    };

                case ModuleZDuelThemeId.Andalucia70s:
                    return new DuelThemeData
                    {
                        themeId = themeId,
                        themeName = "Andalucía 70s",
                        primaryColor = new Color(0.88f, 0.84f, 0.72f),
                        secondaryColor = new Color(0.92f, 0.90f, 0.82f),
                        accentColor = new Color(0.18f, 0.45f, 0.75f),
                        duelTime = 60f,
                        introMessage = "Duelo iniciado — Andalucía 70s",
                        victoryMessage = "Victoria en Andalucía",
                        defeatMessage = "Derrota en Andalucía",
                        musicResourcePath = "Audio/Music/Duel/MUS_Duel_Andalucia70s"
                    };

                default:
                    return GetThemeData(ModuleZDuelThemeId.Madrid70s);
            }
        }
    }
}