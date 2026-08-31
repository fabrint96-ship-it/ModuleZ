using ModuleZ.Core.Managers;
using ModuleZ.Game.Camera;
using ModuleZ.Game.Player;
using ModuleZ.OpenWorld.Themes.Andalucia70s;
using ModuleZ.OpenWorld.Themes.Barcelona70s;
using ModuleZ.OpenWorld.Themes.Madrid70s;
using ModuleZ.OpenWorld.Themes.Valencia70s;
using ModuleZ.UI.HUD;
using ModuleZ.UI.PauseMenu;
using System.Collections;
using UnityEngine;

namespace ModuleZ.OpenWorld.Runtime
{
    public class OpenWorldRuntimeBuilder : MonoBehaviour
    {
        private GameObject player;
        private OpenWorldThemeData currentThemeData;

        private void Start()
        {
            BuildOpenWorld();
        }

        private void BuildOpenWorld()
        {
            currentThemeData = OpenWorldThemeDatabase.GetThemeData(
                ModuleZ.Core.Managers.ModuleZGameState.CurrentOpenWorldTheme
            );

            CreateHUDCoordinator();

            CreateHUD();
            CreateProgressHUD();
            CreateAchievementsHUD();
            CreateStatsHUD();
            CreateAchievementToastHUD();
            CreateZoneHUD();
            CreateSystemMessageHUD();

            CreatePauseMenu();

            BuildTheme();

            CreateMusicController();

            CreatePlayer();
            CreateThirdPersonCamera(player.transform);

            StartCoroutine(ShowPendingOpenWorldMessageWhenReady());

            Debug.Log("[Module Z] OpenWorld generado: " + currentThemeData.themeName);
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

        private void CreatePlayer()
        {
            GameObject builderObj = new GameObject("PlayerBuilder");
            ModuleZPlayerBuilder builder = builderObj.AddComponent<ModuleZPlayerBuilder>();

            Vector3 spawnPosition = ModuleZ.Core.Managers.ModuleZGameState.ReturningFromDuel
                ? ModuleZ.Core.Managers.ModuleZGameState.OpenWorldReturnPosition
                : new Vector3(0f, 0.1f, -4f);

            player = builder.BuildPlayer(spawnPosition);

            Destroy(builderObj);
        }

        private void CreateThirdPersonCamera(Transform target)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";

            Camera camera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
            camera.fieldOfView = 50f;
            camera.clearFlags = CameraClearFlags.Skybox;

            ModuleZThirdPersonCamera followCamera =
                cameraObj.AddComponent<ModuleZThirdPersonCamera>();

            followCamera.SetTarget(target);

            cameraObj.transform.position =
                target.position + target.rotation * new Vector3(0f, 3.2f, -4.5f);

            cameraObj.transform.LookAt(target.position + Vector3.up * 1.6f);
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