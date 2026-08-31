using System;
using ModuleZ.OpenWorld.Runtime;
using System.Collections.Generic;
using ModuleZ.Core.Achievements;

namespace ModuleZ.Core.SaveSystem
{
    [Serializable]
    public class ModuleZSaveData
    {
        public float returnX;
        public float returnY;
        public float returnZ;

        public bool rivalMadridDefeated;
        public bool rivalBarcelonaDefeated;
        public bool rivalValenciaDefeated;
        public bool rivalAndaluciaDefeated;

        public bool andaluciaUnlocked;

        public int duelsWon;
        public int duelsLost;
        public int duelsAbandoned;

        public OpenWorldThemeId currentOpenWorldTheme;

        public bool mainProgressionCompleted;

        public int rematchesWon;
        public int rematchesLost;
        public int rematchesAbandoned;

        public List<ModuleZAchievementState> achievements = new List<ModuleZAchievementState>();

        public bool rivalMadridPersonalityCompleted;
        public bool rivalBarcelonaPersonalityCompleted;
        public bool rivalValenciaPersonalityCompleted;
        public bool rivalAndaluciaPersonalityCompleted;
    }
}