using System.Collections;
using ModuleZ.Core.Managers;
using ModuleZ.Core.SaveSystem;
using ModuleZ.Core.SceneLoading;
using ModuleZ.Core.Theme;
using ModuleZ.OpenWorld.Runtime;
using ModuleZ.UI.HUD;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.UI.PauseMenu
{
    public class OpenWorldPauseMenuController : MonoBehaviour
    {
        private Canvas pauseCanvas;
        private bool isPaused;
        private OpenWorldThemeData themeData;
        private Text pauseInfoText;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape))
                return;

            if (ModuleZHUDOverlayCoordinator.Instance != null &&
                ModuleZHUDOverlayCoordinator.Instance.HasOpenOverlay())
            {
                ModuleZHUDOverlayCoordinator.Instance.CloseAllOverlays();
                return;
            }

            TogglePause();
        }

        private void TogglePause()
        {
            isPaused = !isPaused;

            if (isPaused)
                ShowPauseMenu();
            else
                HidePauseMenu();
        }

        private void ShowPauseMenu()
        {
            ModuleZGameState.IsPaused = true;

            themeData = OpenWorldThemeDatabase.GetThemeData(ModuleZGameState.CurrentOpenWorldTheme);

            Time.timeScale = 0f;

            pauseCanvas = CreateCanvas();
            CreateBackground();
            CreateTitle();
            CreateButton("Continuar", new Vector2(0f, 90f), OnResumeClicked);
            CreateButton("Guardar", new Vector2(0f, 30f), OnSaveClicked);
            CreateButton("Guardar y salir", new Vector2(0f, -30f), OnSaveAndExitClicked);
            CreateButton("Volver al menú", new Vector2(0f, -90f), OnMainMenuClicked);

            CreateInfoText();
        }

        private void HidePauseMenu()
        {
            ModuleZGameState.IsPaused = false;

            Time.timeScale = 1f;

            if (pauseCanvas != null)
                Destroy(pauseCanvas.gameObject);

            pauseCanvas = null;
        }

        private Canvas CreateCanvas()
        {
            GameObject canvasObj = new GameObject("Canvas_OpenWorldPause");
            canvasObj.transform.SetParent(transform, false);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        private void CreateBackground()
        {
            GameObject bgObj = new GameObject("Pause_Background");
            bgObj.transform.SetParent(pauseCanvas.transform, false);

            Image bg = bgObj.AddComponent<Image>();
            bg.color = ModuleZ70sPalette.UIBackground;

            RectTransform rect = bgObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void CreateTitle()
        {
            GameObject titleObj = new GameObject("Pause_Title");
            titleObj.transform.SetParent(pauseCanvas.transform, false);

            Text title = titleObj.AddComponent<Text>();
            title.text = themeData != null
                ? "PAUSA — " + themeData.zoneDisplayName
                : "PAUSA";
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 46;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = ModuleZ70sPalette.UIText;

            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(500f, 80f);
            rect.anchoredPosition = new Vector2(0f, 170f);
        }

        private void CreateButton(string text, Vector2 position, UnityEngine.Events.UnityAction action)
        {
            GameObject buttonObj = new GameObject("Button_" + text);
            buttonObj.transform.SetParent(pauseCanvas.transform, false);

            Image image = buttonObj.AddComponent<Image>();
            image.color = themeData != null
                ? themeData.secondaryColor
                : new Color(0.12f, 0.18f, 0.28f, 1f);

            Button button = buttonObj.AddComponent<Button>();
            button.onClick.AddListener(action);

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(300f, 52f);
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

        private void OnResumeClicked()
        {
            isPaused = false;
            HidePauseMenu();
        }

        private void OnSaveClicked()
        {
            ModuleZSaveManager.SaveGame();

            if (pauseInfoText != null)
                pauseInfoText.text = "Partida guardada.";

            StartCoroutine(ClearPauseInfoText());
        }

        private IEnumerator ClearPauseInfoText()
        {
            yield return new WaitForSecondsRealtime(2f);

            if (pauseInfoText != null)
                pauseInfoText.text = "";
        }

        private void OnMainMenuClicked()
        {
            Time.timeScale = 1f;

            ModuleZ.Core.Managers.ModuleZGameState.IsPaused = false;

            if (pauseCanvas != null)
                Destroy(pauseCanvas.gameObject);

            ModuleZSceneController.Instance.ReturnToMainMenu();
        }

        private void OnSaveAndExitClicked()
        {
            ModuleZSaveManager.SaveGame();

            if (pauseInfoText != null)
                pauseInfoText.text = "Partida guardada. Volviendo al menú...";

            StartCoroutine(ReturnToMenuAfterSave());
        }

        private IEnumerator ReturnToMenuAfterSave()
        {
            yield return new WaitForSecondsRealtime(0.8f);

            Time.timeScale = 1f;

            ModuleZ.Core.Managers.ModuleZGameState.IsPaused = false;

            if (pauseCanvas != null)
                Destroy(pauseCanvas.gameObject);

            ModuleZSceneController.Instance.ReturnToMainMenu();
        }

        private void CreateInfoText()
        {
            GameObject infoObj = new GameObject("Pause_Info_Text");
            infoObj.transform.SetParent(pauseCanvas.transform, false);

            pauseInfoText = infoObj.AddComponent<Text>();
            pauseInfoText.text = "";
            pauseInfoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            pauseInfoText.fontSize = 22;
            pauseInfoText.alignment = TextAnchor.MiddleCenter;
            pauseInfoText.color = Color.white;

            RectTransform rect = infoObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600f, 50f);
            rect.anchoredPosition = new Vector2(0f, -170f);
        }
    }
}