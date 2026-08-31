using System;

namespace ModuleZ.Core.Achievements
{
    [Serializable]
    public class ModuleZAchievementState
    {
        public ModuleZAchievementId achievementId;

        public bool unlocked;

        public string unlockedDate;

        public ModuleZAchievementState(
            ModuleZAchievementId achievementId)
        {
            this.achievementId = achievementId;
            unlocked = false;
            unlockedDate = "";
        }

        public void Unlock()
        {
            if (unlocked)
                return;

            unlocked = true;

            unlockedDate =
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void Reset()
        {
            unlocked = false;
            unlockedDate = "";
        }
    }
}