using ModuleZ.Core.Managers;
using ModuleZ.Game.Camera;
using ModuleZ.Game.Player;
using ModuleZ.OpenWorld.Themes.Andalucia70s;
using ModuleZ.OpenWorld.Themes.Barcelona70s;
using ModuleZ.OpenWorld.Themes.Madrid70s;
using ModuleZ.OpenWorld.Themes.Valencia70s;
using ModuleZ.UI.HUD;
using ModuleZ.UI.PauseMenu;
using System;
using System.Collections;
using UnityEngine;

namespace ModuleZ.OpenWorld.Runtime
{
    public class OpenWorldRuntimeBuilder : MonoBehaviour
    {
        private GameObject player;
        private Camera gameplayCamera;
        private GameObject playerBuilderHost;
        private OpenWorldThemeData currentThemeData;
        private bool buildAttempted;

        public bool TryBuild(
            out GameObject builtPlayer,
            out Camera builtGameplayCamera,
            out AudioListener builtGameplayAudioListener,
            out ModuleZThirdPersonCamera builtCameraController,
            out string failureReason)
        {
            builtPlayer = null;
            builtGameplayCamera = null;
            builtGameplayAudioListener = null;
            builtCameraController = null;
            failureReason = null;

            if (buildAttempted)
            {
                failureReason = "OpenWorld composition has already been attempted.";
                return false;
            }

            buildAttempted = true;
            string stage = "theme data resolution";

            try
            {
                currentThemeData = OpenWorldThemeDatabase.GetThemeData(
                    ModuleZ.Core.Managers.ModuleZGameState.CurrentOpenWorldTheme
                );

                if (currentThemeData == null)
                    throw new InvalidOperationException("Theme data was not found.");

                stage = "HUD composition";
                CreateHUDCoordinator();
                CreateHUD();
                CreateProgressHUD();
                CreateAchievementsHUD();
                CreateStatsHUD();
                CreateAchievementToastHUD();
                CreateZoneHUD();
                CreateSystemMessageHUD();

                stage = "pause composition";
                CreatePauseMenu();

                stage = "theme composition";
                BuildTheme();

                stage = "music composition";
                CreateMusicController();

                stage = "player composition";
                player = CreatePlayer();

                if (player == null)
                    throw new InvalidOperationException("Player creation returned null.");

                stage = "gameplay camera composition";
                gameplayCamera = OpenWorldGameplayCameraBuilder.Create(
                    player.transform,
                    out AudioListener audioListener,
                    out ModuleZThirdPersonCamera cameraController
                );

                builtPlayer = player;
                builtGameplayCamera = gameplayCamera;
                builtGameplayAudioListener = audioListener;
                builtCameraController = cameraController;
                return true;
            }
            catch (Exception exception)
            {
                CleanUpFailedBuild();
                failureReason = stage + " failed: " + exception.Message;
                return false;
            }
        }

        internal void BeginPostBuild()
        {
            StartCoroutine(ShowPendingOpenWorldMessageWhenReady());
        }

        internal void CleanUpFailedBuild()
        {
            DestroyOwnedObject(gameplayCamera != null
                ? gameplayCamera.gameObject
                : null);
            DestroyOwnedObject(player);
            DestroyOwnedObject(playerBuilderHost);

            gameplayCamera = null;
            player = null;
            playerBuilderHost = null;
        }

        private static void DestroyOwnedObject(GameObject ownedObject)
        {
            if (ownedObject == null)
                return;

            ownedObject.SetActive(false);
            Destroy(ownedObject);
        }

        private IEnumerator ShowPendingOpenWorldMessageWhenReady()
        {
            float timeout = 2f;
            float timer = 0f;

            while (OpenWorldMessageHUD.Instance == null && timer < timeout)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            ShowPendingOpenWorldMessage();
        }

        private void ShowPendingOpenWorldMessage()
        {
            string message = ModuleZ.Core.Managers.ModuleZGameState.PendingOpenWorldMessage;

            bool hasPendingImportantMessage = !string.IsNullOrEmpty(message);

            if (!hasPendingImportantMessage)
                message = currentThemeData.enterMessage;

            float duration = hasPendingImportantMessage ? 7f : 3f;

            if (OpenWorldSystemMessageHUD.Instance != null)
            {
                OpenWorldSystemMessageHUD.Instance.Show(message, duration);
            }
            else
            {
                Debug.LogWarning("[Module Z] OpenWorldSystemMessageHUD no existe.");
            }

            ModuleZ.Core.Managers.ModuleZGameState.PendingOpenWorldMessage = "";
            ModuleZ.Core.Managers.ModuleZGameState.LastDuelResultMessage = "";
            ModuleZ.Core.Managers.ModuleZGameState.ReturningFromDuel = false;

            ModuleZ.OpenWorld.Encounters.ModuleZRivalWorldHUDController.RefreshAll();
        }

        private void BuildTheme()
        {
            switch (ModuleZ.Core.Managers.ModuleZGameState.CurrentOpenWorldTheme)
            {
                case OpenWorldThemeId.Madrid70s:
                    gameObject.AddComponent<ModuleZ.OpenWorld.Themes.Madrid70s.Madrid70sOpenWorldThemeBuilder>().Build();
                    break;

                case OpenWorldThemeId.Barcelona70s:
                    gameObject.AddComponent<Barcelona70sOpenWorldThemeBuilder>().Build();
                    break;

                case OpenWorldThemeId.Valencia70s:
                    gameObject.AddComponent<Valencia70sOpenWorldThemeBuilder>().Build();
                    break;

                case OpenWorldThemeId.Andalucia70s:
                    gameObject.AddComponent<Andalucia70sOpenWorldThemeBuilder>().Build();
                    break;
            }
        }

        private GameObject CreatePlayer()
        {
            playerBuilderHost = new GameObject("PlayerBuilder");

            try
            {
                ModuleZPlayerBuilder builder =
                    playerBuilderHost.AddComponent<ModuleZPlayerBuilder>();

                Vector3 spawnPosition = ModuleZ.Core.Managers.ModuleZGameState.ReturningFromDuel
                    ? ModuleZ.Core.Managers.ModuleZGameState.OpenWorldReturnPosition
                    : new Vector3(0f, 0.1f, -4f);

                return builder.BuildPlayer(spawnPosition);
            }
            finally
            {
                DestroyOwnedObject(playerBuilderHost);
                playerBuilderHost = null;
            }
        }

        private void CreateHUD()
        {
            gameObject.AddComponent<OpenWorldMessageHUD>();
        }

        private void CreateProgressHUD()
        {
            gameObject.AddComponent<OpenWorldProgressHUD>();
        }

        private void CreateAchievementsHUD()
        {
            gameObject.AddComponent<ModuleZAchievementsHUD>();
        }

        private void CreateAchievementToastHUD()
        {
            gameObject.AddComponent<ModuleZAchievementToastHUD>();
        }

        private void CreateMusicController()
        {
            OpenWorldMusicController music = gameObject.AddComponent<OpenWorldMusicController>();
            music.PlayThemeMusic(currentThemeData);
        }

        private void CreateZoneHUD()
        {
            gameObject.AddComponent<OpenWorldZoneHUD>();
        }

        private void CreatePauseMenu()
        {
            gameObject.AddComponent<OpenWorldPauseMenuController>();
        }

        private void CreateSystemMessageHUD()
        {
            gameObject.AddComponent<OpenWorldSystemMessageHUD>();
        }

        private void CreateStatsHUD()
        {
            gameObject.AddComponent<ModuleZStatsHUD>();
        }

        private void CreateHUDCoordinator()
        {
            gameObject.AddComponent<ModuleZHUDOverlayCoordinator>();
        }
    }
}
