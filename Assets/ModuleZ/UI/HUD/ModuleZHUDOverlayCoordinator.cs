using ModuleZ.Core.Managers;
using UnityEngine;

namespace ModuleZ.UI.HUD
{
    public class ModuleZHUDOverlayCoordinator : MonoBehaviour
    {
        public static ModuleZHUDOverlayCoordinator Instance { get; private set; }

        private OpenWorldProgressHUD progressHUD;
        private ModuleZAchievementsHUD achievementsHUD;
        private ModuleZStatsHUD statsHUD;

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterProgressHUD(OpenWorldProgressHUD hud)
        {
            progressHUD = hud;
        }

        public void RegisterAchievementsHUD(ModuleZAchievementsHUD hud)
        {
            achievementsHUD = hud;
        }

        public void RegisterStatsHUD(ModuleZStatsHUD hud)
        {
            statsHUD = hud;
        }

        public void NotifyProgressOpened()
        {
            ModuleZGameState.IsOverlayOpen = true;

            achievementsHUD?.ForceHideFromCoordinator();
            statsHUD?.ForceHideFromCoordinator();
        }

        public void NotifyAchievementsOpened()
        {
            ModuleZGameState.IsOverlayOpen = true;

            progressHUD?.ForceHideFromCoordinator();
            statsHUD?.ForceHideFromCoordinator();
        }

        public void NotifyStatsOpened()
        {
            ModuleZGameState.IsOverlayOpen = true;

            progressHUD?.ForceHideFromCoordinator();
            achievementsHUD?.ForceHideFromCoordinator();
        }

        public void NotifyOverlayClosed()
        {
            bool anyOpen =
                IsProgressOpen() ||
                IsAchievementsOpen() ||
                IsStatsOpen();

            ModuleZGameState.IsOverlayOpen = anyOpen;
        }

        private bool IsProgressOpen()
        {
            return progressHUD != null && progressHUD.IsVisible;
        }

        private bool IsAchievementsOpen()
        {
            return achievementsHUD != null && achievementsHUD.IsVisible;
        }

        private bool IsStatsOpen()
        {
            return statsHUD != null && statsHUD.IsVisible;
        }

        public bool HasOpenOverlay()
        {
            return
                IsProgressOpen() ||
                IsAchievementsOpen() ||
                IsStatsOpen();
        }

        public void CloseAllOverlays()
        {
            progressHUD?.ForceHide();
            achievementsHUD?.ForceHide();
            statsHUD?.ForceHide();

            ModuleZGameState.IsOverlayOpen = false;
        }
    }
}