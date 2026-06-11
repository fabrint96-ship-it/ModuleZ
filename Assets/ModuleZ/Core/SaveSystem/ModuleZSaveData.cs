namespace ModuleZ.Core.SaveSystem
{
    [System.Serializable]
    public class ModuleZSaveData
    {
        public int duelsWon;
        public int duelsLost;
        public int duelsAbandoned;

        public bool rivalMadridDefeated;
        public bool rivalBarcelonaDefeated;
        public bool rivalValenciaDefeated;
        public bool rivalAndaluciaDefeated;

        public bool andaluciaUnlocked;

        public string currentOpenWorldTheme;
    }
}