using ModuleZ.Duel.Themes.Madrid70s;
using ModuleZ.Duel.Themes.Barcelona70s;
using ModuleZ.Duel.Themes.Valencia70s;
using ModuleZ.Duel.Themes.Andalucia70s;
using ModuleZ.Game.Camera;
using ModuleZ.Game.Player;
using UnityEngine;
using ModuleZ.Duel.Builders;
using ModuleZ.Duel.Rules;
using ModuleZ.UI.PauseMenu;

namespace ModuleZ.Duel.Runtime
{
    public class DuelRuntimeBuilder : MonoBehaviour
    {
        private GameObject player;
        private string currentThemeName = "Madrid 70s";
        private DuelThemeData currentThemeData;

        private void Start()
        {
            BuildDuel();
        }

        private void BuildDuel()
        {
            currentThemeData = DuelThemeDatabase.GetThemeData(
                ModuleZ.Core.Managers.ModuleZGameState.CurrentDuelTheme
            );

            BuildTheme();
            CreatePlayer();
            CreateArenaCamera(player.transform);
            CreatePuzzleZ();
            CreateFeedbackController();
            CreateResultManager();
            CreatePauseMenu();
            CreateTimer();
            CreateAudioFeedbackController();
            CreateMusicController();
            CreateHUD();

            if (DuelHUDController.Instance != null)
            {
                DuelHUDController.Instance.ApplyTheme(currentThemeData);

                DuelHUDController.Instance.ShowDuelIntro(
                    currentThemeData.introMessage,
                    GetCurrentRivalName()
                );
            }

            Debug.Log("[Module Z] Duel generado desde RuntimeBuilder.");
        }

        private void BuildMadrid70sDuel()
        {
            gameObject.AddComponent<Madrid70sDuelThemeBuilder>().Build();
        }

        private void CreatePlayer()
        {
            GameObject builderObj = new GameObject("Duel_PlayerBuilder");
            ModuleZPlayerBuilder builder = builderObj.AddComponent<ModuleZPlayerBuilder>();

            player = builder.BuildPlayer(new Vector3(0f, 0.1f, -11f));
            player.name = "Duel_Player";

            ModuleZPlayerController controller = player.GetComponent<ModuleZPlayerController>();
            if (controller != null)
                controller.CanMove = false;

            Destroy(builderObj);

            player.AddComponent<DuelPlayerIdleAnimation>();
        }

        private void CreateArenaCamera(Transform target)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";

            Camera camera = cameraObj.AddComponent<Camera>();
            camera.fieldOfView = 50f;
            camera.clearFlags = CameraClearFlags.Skybox;

            ModuleZThirdPersonCamera followCamera = cameraObj.AddComponent<ModuleZThirdPersonCamera>();
            followCamera.SetTarget(target);

            cameraObj.transform.position = target.position + new Vector3(0f, 9f, -10f);
            cameraObj.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
        }

        private void CreatePuzzleZ()
        {
            gameObject.AddComponent<DuelPuzzleZBuilder>().Build();
        }

        private void CreateHUD()
        {
            gameObject.AddComponent<DuelHUDController>();
        }

        private void BuildTheme()
        {
            switch (ModuleZ.Core.Managers.ModuleZGameState.CurrentDuelTheme)
            {
                case ModuleZDuelThemeId.Madrid70s:
                    currentThemeName = "Madrid 70s";
                    BuildMadrid70sDuel();
                    break;

                case ModuleZDuelThemeId.Barcelona70s:
                    currentThemeName = "Barcelona 70s";
                    gameObject.AddComponent<Barcelona70sDuelThemeBuilder>().Build();
                    break;

                case ModuleZDuelThemeId.Valencia70s:
                    currentThemeName = "Valencia 70s";
                    gameObject.AddComponent<Valencia70sDuelThemeBuilder>().Build();
                    break;

                case ModuleZDuelThemeId.Andalucia70s:
                    currentThemeName = "Andalucía 70s";
                    gameObject.AddComponent<Andalucia70sDuelThemeBuilder>().Build();
                    break;
            }
        }

        private string GetCurrentRivalName()
        {
            switch (ModuleZ.Core.Managers.ModuleZGameState.CurrentDuelRival)
            {
                case ModuleZ.OpenWorld.Encounters.ModuleZRivalId.Madrid:
                    return "Rival Madrid";

                case ModuleZ.OpenWorld.Encounters.ModuleZRivalId.Barcelona:
                    return "Rival Barcelona";

                case ModuleZ.OpenWorld.Encounters.ModuleZRivalId.Valencia:
                    return "Rival Valencia";

                default:
                    return "Rival";
            }
        }

        private void CreateResultManager()
        {
            gameObject.AddComponent<DuelResultManager>();
        }

        private void CreateTimer()
        {
            DuelTimerController timer = gameObject.AddComponent<DuelTimerController>();
            timer.SetTime(currentThemeData.duelTime);
        }

        private void CreateFeedbackController()
        {
            gameObject.AddComponent<DuelFeedbackController>();
        }

        private void CreateAudioFeedbackController()
        {
            gameObject.AddComponent<DuelAudioFeedbackController>();
        }

        private void CreateMusicController()
        {
            DuelMusicController music =
                gameObject.AddComponent<DuelMusicController>();

            music.PlayThemeMusic(currentThemeData);
        }

        private void CreatePauseMenu()
        {
            gameObject.AddComponent<DuelPauseMenuController>();
        }
    }
}