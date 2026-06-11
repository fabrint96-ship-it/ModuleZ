using ModuleZ.Core.SceneLoading;
using ModuleZ.UI.Runtime;
using UnityEngine;
using ModuleZ.Core.SaveSystem;
using ModuleZ.Core.Settings;
using UnityEngine.UI;

namespace ModuleZ.UI.MainMenu
{
    public class ModuleZMainMenuBuilder : MonoBehaviour
    {
        private Canvas canvas;
        private Text infoText;
        private bool menuVisible;
        private bool waitingDeleteConfirmation;
        private bool waitingNewGameConfirmation;
        private Text audioButtonText;
        private Text fullscreenButtonText;

        private void Start()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Boot")
                BuildMainMenu();
        }

        public void ShowMainMenu()
        {
            if (menuVisible)
                return;

            BuildMainMenu();
        }

        private void BuildMainMenu()
        {
            if (menuVisible)
                return;

            menuVisible = true;

            canvas = ModuleZUIManager.Instance.CreateCanvas("Canvas_MainMenu");

            CreateBackground();
            CreateTitle();
            CreateButton("Nueva Partida", new Vector2(0, 40), OnNewGameClicked);
            CreateButton("Continuar", new Vector2(0, -25), OnContinueClicked);
            CreateButton("Borrar Partida", new Vector2(0, -220), OnDeleteSaveClicked);
            CreateButton("Opciones", new Vector2(0, -90), OnSettingsClicked);
            CreateButton("Salir", new Vector2(0, -155), OnExitClicked);

            CreateInfoText();
            RefreshSaveInfo();
            CreateVersionText();

            Debug.Log("[Module Z] Menú principal generado por código.");
        }

        private void CreateBackground()
        {
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(canvas.transform, false);

            Image image = bg.AddComponent<Image>();
            image.color = new Color(0.04f, 0.06f, 0.09f, 1f);

            RectTransform rect = bg.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void CreateTitle()
        {
            GameObject titleObj = new GameObject("Title_ModuleZ");
            titleObj.transform.SetParent(canvas.transform, false);

            Text title = titleObj.AddComponent<Text>();
            title.text = "MODULE Z";
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 56;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = Color.white;

            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(500, 90);
            rect.anchoredPosition = new Vector2(0, 180);
        }

        private void CreateButton(string text, Vector2 position, UnityEngine.Events.UnityAction action)
        {
            GameObject buttonObj = new GameObject("Button_" + text);
            buttonObj.transform.SetParent(canvas.transform, false);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.12f, 0.18f, 0.28f, 1f);

            Button button = buttonObj.AddComponent<Button>();
            button.onClick.AddListener(action);

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(260, 48);
            rect.anchoredPosition = position;

            GameObject labelObj = new GameObject("Text");
            labelObj.transform.SetParent(buttonObj.transform, false);

            Text label = labelObj.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 24;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;

            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
        }

        private void OnNewGameClicked()
        {
            waitingDeleteConfirmation = false;

            if (!waitingNewGameConfirmation)
            {
                waitingNewGameConfirmation = true;

                if (infoText != null)
                    infoText.text = ModuleZSaveManager.HasSave()
                        ? "Pulsa otra vez Nueva Partida para borrar el progreso."
                        : "Pulsa otra vez Nueva Partida para empezar.";

                return;
            }

            waitingNewGameConfirmation = false;

            ModuleZSaveManager.DeleteSave();

            ModuleZ.Core.Managers.ModuleZGameState.DuelsWon = 0;
            ModuleZ.Core.Managers.ModuleZGameState.DuelsLost = 0;
            ModuleZ.Core.Managers.ModuleZGameState.DuelsAbandoned = 0;

            ModuleZ.Core.Managers.ModuleZGameState.RivalMadridDefeated = false;
            ModuleZ.Core.Managers.ModuleZGameState.RivalBarcelonaDefeated = false;
            ModuleZ.Core.Managers.ModuleZGameState.RivalValenciaDefeated = false;
            ModuleZ.Core.Managers.ModuleZGameState.RivalAndaluciaDefeated = false;

            ModuleZ.Core.Managers.ModuleZGameState.AndaluciaUnlocked = false;
            ModuleZ.Core.Managers.ModuleZGameState.CurrentOpenWorldTheme =
                ModuleZ.OpenWorld.Runtime.OpenWorldThemeId.Madrid70s;

            menuVisible = false;

            ModuleZUIManager.Instance.DestroyCurrentCanvas();
            ModuleZSceneController.Instance.LoadOpenWorld();
        }

        private void CancelPendingConfirmations()
        {
            waitingNewGameConfirmation = false;
            waitingDeleteConfirmation = false;
        }

        private void OnContinueClicked()
        {
            CancelPendingConfirmations();

            if (!ModuleZSaveManager.HasSave())
            {
                Debug.Log("[Module Z] No hay partida guardada.");

                if (infoText != null)
                    infoText.text = "No hay partida guardada.";

                return;
            }

            ModuleZSaveManager.LoadGame();

            menuVisible = false;

            ModuleZUIManager.Instance.DestroyCurrentCanvas();
            ModuleZSceneController.Instance.LoadOpenWorld();
        }

        private void OnDeleteSaveClicked()
        {
            waitingNewGameConfirmation = false;

            if (!ModuleZSaveManager.HasSave())
            {
                if (infoText != null)
                    infoText.text = "No hay partida para borrar.";

                return;
            }

            if (!waitingDeleteConfirmation)
            {
                waitingDeleteConfirmation = true;

                if (infoText != null)
                    infoText.text = "Pulsa otra vez Borrar Partida para confirmar.";

                return;
            }

            waitingDeleteConfirmation = false;

            if (infoText != null)
                infoText.text = "Partida guardada eliminada.";
        }

        private void OnSettingsClicked()
        {
            CancelPendingConfirmations();

            ModuleZUIManager.Instance.DestroyCurrentCanvas();
            menuVisible = false;

            BuildSettingsMenu();
        }

        private void OnExitClicked()
        {
            CancelPendingConfirmations();

            Debug.Log("[Module Z] Saliendo del juego.");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
        }

        private void CreateInfoText()
        {
            GameObject infoObj = new GameObject("Info_Text");
            infoObj.transform.SetParent(canvas.transform, false);

            infoText = infoObj.AddComponent<Text>();
            infoText.text = "";
            infoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            infoText.fontSize = 22;
            infoText.alignment = TextAnchor.MiddleCenter;
            infoText.color = Color.white;

            RectTransform rect = infoObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600f, 50f);
            rect.anchoredPosition = new Vector2(0f, -285f);
        }

        private void RefreshSaveInfo()
        {
            if (infoText == null)
                return;

            if (!ModuleZSaveManager.HasSave())
            {
                infoText.text = "No hay partida guardada.";
                return;
            }

            ModuleZSaveData data = ModuleZSaveManager.GetSaveData();

            if (data == null)
            {
                infoText.text = "Partida guardada encontrada.";
                return;
            }

            infoText.text =
                "Guardado: " + GetCleanZoneName(data.currentOpenWorldTheme) +
                " | Ganados: " + data.duelsWon +
                " | Perdidos: " + data.duelsLost +
                " | Abandonados: " + data.duelsAbandoned;
        }

        private string GetCleanZoneName(string themeName)
        {
            switch (themeName)
            {
                case "Madrid70s":
                    return "Madrid";

                case "Barcelona70s":
                    return "Barcelona";

                case "Valencia70s":
                    return "Valencia";

                case "Andalucia70s":
                    return "Andalucía";

                default:
                    return themeName;
            }
        }

        private void BuildSettingsMenu()
        {
            canvas = ModuleZUIManager.Instance.CreateCanvas("Canvas_SettingsMenu");

            CreateBackground();

            CreateSettingsTitle();

            CreateAudioButton(new Vector2(0f, 40f));
            CreateFullscreenButton(new Vector2(0f, -25f));
            CreateButton("Restaurar opciones", new Vector2(0f, -90f), OnResetSettingsClicked);
            CreateButton("Volver", new Vector2(0f, -155f), OnBackToMainMenuClicked);

            CreateInfoText();
        }

        private void CreateSettingsTitle()
        {
            GameObject titleObj = new GameObject("Title_Settings");
            titleObj.transform.SetParent(canvas.transform, false);

            Text title = titleObj.AddComponent<Text>();
            title.text = "OPCIONES";
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 48;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = Color.white;

            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(500f, 80f);
            rect.anchoredPosition = new Vector2(0f, 170f);
        }

        private void OnAudioClicked()
        {
            ModuleZ.Core.Managers.ModuleZGameState.AudioEnabled =
                !ModuleZ.Core.Managers.ModuleZGameState.AudioEnabled;

            AudioListener.volume =
                ModuleZ.Core.Managers.ModuleZGameState.AudioEnabled ? 1f : 0f;

            RefreshAudioButtonText();

            ModuleZSettingsManager.SaveSettings();
        }

        private void RefreshAudioButtonText()
        {
            if (audioButtonText == null)
                return;

            audioButtonText.text =
                ModuleZ.Core.Managers.ModuleZGameState.AudioEnabled
                    ? "Audio: ON"
                    : "Audio: OFF";
        }

        private void OnFullscreenClicked()
        {
            ModuleZ.Core.Managers.ModuleZGameState.FullscreenEnabled =
                !ModuleZ.Core.Managers.ModuleZGameState.FullscreenEnabled;

            Screen.fullScreen =
                ModuleZ.Core.Managers.ModuleZGameState.FullscreenEnabled;

            RefreshFullscreenButtonText();

            ModuleZSettingsManager.SaveSettings();
        }

        private void OnBackToMainMenuClicked()
        {
            ModuleZUIManager.Instance.DestroyCurrentCanvas();
            BuildMainMenu();
        }

        private void CreateAudioButton(Vector2 position)
        {
            GameObject buttonObj = new GameObject("Button_Audio");
            buttonObj.transform.SetParent(canvas.transform, false);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.12f, 0.18f, 0.28f, 1f);

            Button button = buttonObj.AddComponent<Button>();
            button.onClick.AddListener(OnAudioClicked);

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(260, 48);
            rect.anchoredPosition = position;

            GameObject labelObj = new GameObject("Text");
            labelObj.transform.SetParent(buttonObj.transform, false);

            audioButtonText = labelObj.AddComponent<Text>();
            audioButtonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            audioButtonText.fontSize = 24;
            audioButtonText.alignment = TextAnchor.MiddleCenter;
            audioButtonText.color = Color.white;

            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            RefreshAudioButtonText();
        }

        private void CreateFullscreenButton(Vector2 position)
        {
            GameObject buttonObj = new GameObject("Button_Fullscreen");
            buttonObj.transform.SetParent(canvas.transform, false);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.12f, 0.18f, 0.28f, 1f);

            Button button = buttonObj.AddComponent<Button>();
            button.onClick.AddListener(OnFullscreenClicked);

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(300, 48);
            rect.anchoredPosition = position;

            GameObject labelObj = new GameObject("Text");
            labelObj.transform.SetParent(buttonObj.transform, false);

            fullscreenButtonText = labelObj.AddComponent<Text>();
            fullscreenButtonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            fullscreenButtonText.fontSize = 24;
            fullscreenButtonText.alignment = TextAnchor.MiddleCenter;
            fullscreenButtonText.color = Color.white;

            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            RefreshFullscreenButtonText();
        }

        private void RefreshFullscreenButtonText()
        {
            if (fullscreenButtonText == null)
                return;

            fullscreenButtonText.text =
                ModuleZ.Core.Managers.ModuleZGameState.FullscreenEnabled
                    ? "Pantalla: Completa"
                    : "Pantalla: Ventana";
        }

        private void OnResetSettingsClicked()
        {
            ModuleZSettingsManager.ResetSettings();

            RefreshAudioButtonText();
            RefreshFullscreenButtonText();

            if (infoText != null)
                infoText.text = "Opciones restauradas.";
        }

        private void CreateVersionText()
        {
            GameObject versionObj = new GameObject("Version_Text");
            versionObj.transform.SetParent(canvas.transform, false);

            Text versionText = versionObj.AddComponent<Text>();
            versionText.text = ModuleZBuildInfo.FullVersion;
            versionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            versionText.fontSize = 18;
            versionText.alignment = TextAnchor.LowerRight;
            versionText.color = new Color(1f, 1f, 1f, 0.65f);

            RectTransform rect = versionObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.sizeDelta = new Vector2(600f, 40f);
            rect.anchoredPosition = new Vector2(-20f, 15f);
        }
    }
}