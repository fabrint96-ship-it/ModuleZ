using System.Text;
using ModuleZ.Core.Achievements;
using ModuleZ.Core.Managers;
using ModuleZ.Core.SaveSystem;
using ModuleZ.Core.SceneLoading;
using ModuleZ.Core.Settings;
using ModuleZ.Core.Theme;
using ModuleZ.UI.MainMenu;
using ModuleZ.UI.Runtime;
using UnityEngine;
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
        private GameObject achievementsPanel;
        private bool environmentCreated;
        private GameObject menuEnvironmentRoot;

        private void Start()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Boot")
                BuildMainMenu();
        }

        public void ShowMainMenu()
        {
            menuVisible = false;
            canvas = null;
            achievementsPanel = null;

            BuildMainMenu();
        }

        private void BuildMainMenu()
        {
            if (menuVisible && canvas != null)
                return;

            menuVisible = true;

            CreateMenuEnvironment();

            canvas = ModuleZUIManager.Instance.CreateCanvas("Canvas_MainMenu");

            CreateBackground();
            CreateTitle();
            CreateButton("Nueva Partida", new Vector2(0, 70), OnNewGameClicked);
            CreateButton("Continuar", new Vector2(0, 5), OnContinueClicked);
            CreateButton("Opciones", new Vector2(0, -60), OnSettingsClicked);
            CreateButton("Logros", new Vector2(0, -125), OnAchievementsClicked);
            CreateButton("Créditos", new Vector2(0, -190), OnCreditsClicked);
            CreateButton("Borrar Partida", new Vector2(0, -255), OnDeleteSaveClicked);
            CreateButton("Salir", new Vector2(0, -320), OnExitClicked);

            CreateInfoText();
            RefreshSaveInfo();
            CreateVersionText();

            Debug.Log("[Module Z] Menú principal generado por código.");
        }

        private void OnCreditsClicked()
        {
            CancelPendingConfirmations();

            ModuleZUIManager.Instance.DestroyCurrentCanvas();

            canvas = null;
            menuVisible = false;
            achievementsPanel = null;

            BuildCreditsScreen();
        }

        private void BuildCreditsScreen()
        {
            canvas = ModuleZUIManager.Instance.CreateCanvas("Canvas_CreditsMenu");

            CreateBackground();
            CreateCreditsTitle();
            CreateCreditsText();

            CreateButton(
                "Volver",
                new Vector2(0f, -320f),
                OnBackToMainMenuClicked
            );
        }

        private void CreateCreditsTitle()
        {
            GameObject titleObj = new GameObject("Title_Credits");
            titleObj.transform.SetParent(canvas.transform, false);

            Text title = titleObj.AddComponent<Text>();
            title.text = "CRÉDITOS";
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 48;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = ModuleZ70sPalette.UIText;

            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600f, 80f);
            rect.anchoredPosition = new Vector2(0f, 250f);
        }

        private void CreateCreditsText()
        {
            GameObject textObj = new GameObject("Credits_Text");
            textObj.transform.SetParent(canvas.transform, false);

            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 26;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = ModuleZ70sPalette.UIText;

            text.text =
                "MODULE Z\n\n" +
                "Desarrollado por\n" +
                "Fabricio Torres\n\n" +
                "Propietario del juego\n" +
                " José Santiago Rodríguez Samaniego\n\n" +
                "Juego de duelos, puzzles y mundo abierto\n" +
                "inspirado en la España de los años 70.";

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(900f, 360f);
            rect.anchoredPosition = new Vector2(0f, 20f);
        }

        private void CreateAchievementsScrollView(
            Transform parent,
            string content)
        {
            GameObject scrollObj =
                new GameObject("AchievementsScrollView");

            scrollObj.transform.SetParent(parent, false);

            Image bg =
                scrollObj.AddComponent<Image>();

            bg.color = ModuleZ70sPalette.UIBackground;

            ScrollRect scrollRect =
                scrollObj.AddComponent<ScrollRect>();

            RectTransform scrollRectTransform =
                scrollObj.GetComponent<RectTransform>();

            scrollRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            scrollRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            scrollRectTransform.pivot = new Vector2(0.5f, 0.5f);
            scrollRectTransform.sizeDelta = new Vector2(900f, 520f);
            scrollRectTransform.anchoredPosition = new Vector2(0f, -25f);

            GameObject viewportObj =
                new GameObject("Viewport");

            viewportObj.transform.SetParent(scrollObj.transform, false);

            Image viewportImage =
                viewportObj.AddComponent<Image>();

            viewportImage.color =
                new Color(1f, 1f, 1f, 0.01f);

            Mask mask =
                viewportObj.AddComponent<Mask>();

            mask.showMaskGraphic = false;

            RectTransform viewportRect =
                viewportObj.GetComponent<RectTransform>();

            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = new Vector2(35f, 25f);
            viewportRect.offsetMax = new Vector2(-35f, -25f);

            GameObject contentObj =
                new GameObject("Content");

            contentObj.transform.SetParent(viewportObj.transform, false);

            Text contentText =
                contentObj.AddComponent<Text>();

            contentText.font =
                Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            contentText.fontSize = 24;
            contentText.color = Color.white;
            contentText.alignment = TextAnchor.UpperLeft;
            contentText.horizontalOverflow = HorizontalWrapMode.Wrap;
            contentText.verticalOverflow = VerticalWrapMode.Overflow;
            contentText.text = content;

            RectTransform contentRect =
                contentObj.GetComponent<RectTransform>();

            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0f, 1f);
            contentRect.anchoredPosition = Vector2.zero;

            float preferredHeight =
                contentText.preferredHeight + 80f;

            contentRect.sizeDelta =
                new Vector2(0f, preferredHeight);

            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;
            scrollRect.vertical = true;
            scrollRect.horizontal = false;
            scrollRect.scrollSensitivity = 45f;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
        }

        private void CreateMenuEnvironment()
        {
            if (GameObject.Find("MainMenu_EnvironmentRoot") != null)
                return;

            menuEnvironmentRoot = new GameObject("MainMenu_EnvironmentRoot");

            ModuleZMainMenuEnvironmentBuilder builder =
                menuEnvironmentRoot.AddComponent<ModuleZMainMenuEnvironmentBuilder>();

            builder.Build();

            environmentCreated = true;
        }

        private void OnAchievementsClicked()
        {
            CancelPendingConfirmations();

            ModuleZUIManager.Instance.DestroyCurrentCanvas();

            canvas = null;
            menuVisible = false;
            achievementsPanel = null;

            BuildAchievementsScreen();
        }

        private void BuildAchievementsScreen()
        {
            canvas = ModuleZUIManager.Instance.CreateCanvas("Canvas_AchievementsMenu");

            CreateBackground();
            CreateAchievementsTitle();

            System.Text.StringBuilder builder =
                new System.Text.StringBuilder();

            foreach (var achievement in ModuleZAchievementManager.GetAll())
            {
                string name =
                    ModuleZAchievementDisplayNames.Get(
                        achievement.achievementId
                    );

                string description =
                    ModuleZAchievementDisplayNames.GetDescription(
                        achievement.achievementId
                    );

                builder.AppendLine(
                    (achievement.unlocked ? "✓ " : "✗ ") +
                    name
                );

                builder.AppendLine(
                    "   " + description
                );

                if (achievement.unlocked)
                {
                    builder.AppendLine(
                        "   Desbloqueado: " +
                        achievement.unlockedDate
                    );
                }

                builder.AppendLine();
            }

            builder.AppendLine(
                "Logros: " +
                ModuleZAchievementManager.GetUnlockedCount() +
                " / " +
                ModuleZAchievementManager.GetTotalCount()
            );

            CreateAchievementsScrollView(
                canvas.transform,
                builder.ToString()
            );

            CreateButton(
                "Volver",
                new Vector2(0f, -420f),
                OnBackFromAchievementsClicked
            );
        }

        private void CreateAchievementsTitle()
        {
            GameObject titleObj = new GameObject("Title_Achievements");
            titleObj.transform.SetParent(canvas.transform, false);

            Text title = titleObj.AddComponent<Text>();
            title.text = "LOGROS";
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 48;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = ModuleZ70sPalette.UIText;

            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(500f, 80f);
            rect.anchoredPosition = new Vector2(0f, 320f);
        }

        private void OnBackFromAchievementsClicked()
        {
            ModuleZUIManager.Instance.DestroyCurrentCanvas();

            canvas = null;
            menuVisible = false;
            achievementsPanel = null;

            BuildMainMenu();
        }

        private void CreateBackground()
        {
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(canvas.transform, false);

            Image image = bg.AddComponent<Image>();
            image.color = new Color(0.02f, 0.03f, 0.05f, 0.45f);

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
            title.fontSize = 72;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = ModuleZ70sPalette.UIText;

            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(700f, 100f);
            rect.anchoredPosition = new Vector2(0f, 250f);

            GameObject subtitleObj = new GameObject("Subtitle_ModuleZ");
            subtitleObj.transform.SetParent(canvas.transform, false);

            Text subtitle = subtitleObj.AddComponent<Text>();
            subtitle.text = "DUELS · PUZZLES · OPEN WORLD";
            subtitle.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            subtitle.fontSize = 22;
            subtitle.alignment = TextAnchor.MiddleCenter;
            subtitle.color = new Color(1f, 1f, 1f, 0.75f);

            RectTransform subRect = subtitleObj.GetComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0.5f, 0.5f);
            subRect.anchorMax = new Vector2(0.5f, 0.5f);
            subRect.pivot = new Vector2(0.5f, 0.5f);
            subRect.sizeDelta = new Vector2(700f, 40f);
            subRect.anchoredPosition = new Vector2(0f, 195f);
        }

        private void CreateButton(
    string text,
    Vector2 position,
    UnityEngine.Events.UnityAction action)
        {
            GameObject buttonObj = new GameObject("Button_" + text);
            buttonObj.transform.SetParent(canvas.transform, false);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.08f, 0.14f, 0.24f, 0.92f);

            Button button = buttonObj.AddComponent<Button>();
            button.onClick.AddListener(action);

            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.08f, 0.14f, 0.24f, 0.92f);
            colors.highlightedColor = new Color(0.15f, 0.35f, 0.65f, 1f);
            colors.pressedColor = new Color(0.05f, 0.10f, 0.18f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(320f, 54f);
            rect.anchoredPosition = position;

            GameObject accentObj = new GameObject("AccentLine");
            accentObj.transform.SetParent(buttonObj.transform, false);

            Image accent = accentObj.AddComponent<Image>();
            accent.color = ModuleZ70sPalette.UIAccent;

            RectTransform accentRect = accentObj.GetComponent<RectTransform>();
            accentRect.anchorMin = new Vector2(0f, 0f);
            accentRect.anchorMax = new Vector2(0f, 1f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.sizeDelta = new Vector2(6f, 0f);
            accentRect.anchoredPosition = Vector2.zero;

            GameObject labelObj = new GameObject("Text");
            labelObj.transform.SetParent(buttonObj.transform, false);

            Text label = labelObj.AddComponent<Text>();
            label.text = text.ToUpper();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 23;
            label.fontStyle = FontStyle.Bold;
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
            ModuleZ.Core.Managers.ModuleZGameState.ResetGameState();

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
                ModuleZ.Core.Managers.ModuleZGameState.ResetGameState();

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

            ModuleZSaveManager.DeleteSave();
            ModuleZ.Core.Managers.ModuleZGameState.ResetGameState();

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
            infoText.fontSize = 20;
            infoText.alignment = TextAnchor.UpperCenter;
            infoText.color = Color.white;

            RectTransform rect = infoObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(800f, 120f);
            rect.anchoredPosition = new Vector2(0f, -490f);
        }

        private void RefreshSaveInfo()
        {
            if (infoText == null)
                return;

            if (!ModuleZSaveManager.HasSave())
            {
                infoText.text =
                    "NUEVA AVENTURA DISPONIBLE\n" +
                    "Explora Madrid, Barcelona, Valencia y Andalucía";
                return;
            }

            ModuleZSaveData data =
                ModuleZSaveManager.GetSaveData();

            if (data == null)
            {
                infoText.text =
                    "PARTIDA GUARDADA DISPONIBLE";
                return;
            }

            infoText.text =
                "ZONA: " +
                GetCleanZoneName(data.currentOpenWorldTheme) +
                "\n" +
                "DUEL0S GANADOS: " + data.duelsWon +
                "   |   PERDIDOS: " + data.duelsLost +
                "\n" +
                "LOGROS: " +
                ModuleZAchievementManager.GetUnlockedCount() +
                "/" +
                ModuleZAchievementManager.GetTotalCount();
        }

        private string GetCleanZoneName(ModuleZ.OpenWorld.Runtime.OpenWorldThemeId theme)
        {
            switch (theme)
            {
                case ModuleZ.OpenWorld.Runtime.OpenWorldThemeId.Madrid70s:
                    return "Madrid";

                case ModuleZ.OpenWorld.Runtime.OpenWorldThemeId.Barcelona70s:
                    return "Barcelona";

                case ModuleZ.OpenWorld.Runtime.OpenWorldThemeId.Valencia70s:
                    return "Valencia";

                case ModuleZ.OpenWorld.Runtime.OpenWorldThemeId.Andalucia70s:
                    return "Andalucía";

                default:
                    return theme.ToString();
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
            title.color = ModuleZ70sPalette.UIText;

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

            canvas = null;
            menuVisible = false;
            achievementsPanel = null;

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