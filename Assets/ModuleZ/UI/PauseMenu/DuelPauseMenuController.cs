using ModuleZ.Duel.Rules;
using UnityEngine;
using UnityEngine.UI;
using ModuleZ.Core.Managers;
using ModuleZ.Duel.Runtime;

namespace ModuleZ.UI.PauseMenu
{
    public class DuelPauseMenuController : MonoBehaviour
    {
        private Canvas pauseCanvas;
        private bool isPaused;
        private DuelThemeData themeData;

        private void Update()
        {
            if (ModuleZ.Core.Managers.ModuleZGameState.DuelCompleted)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
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
            themeData = DuelThemeDatabase.GetThemeData(ModuleZGameState.CurrentDuelTheme);

            Time.timeScale = 0f;

            pauseCanvas = CreateCanvas();
            CreateBackground();
            CreateTitle();
            CreateButton("Continuar duelo", new Vector2(0f, 40f), OnResumeClicked);
            CreateButton("Abandonar duelo", new Vector2(0f, -30f), OnAbandonClicked);

            ModuleZ.Core.Managers.ModuleZGameState.IsPaused = true;
        }

        private void HidePauseMenu()
        {
            Time.timeScale = 1f;

            if (pauseCanvas != null)
                Destroy(pauseCanvas.gameObject);

            pauseCanvas = null;

            ModuleZ.Core.Managers.ModuleZGameState.IsPaused = false;
        }

        private Canvas CreateCanvas()
        {
            GameObject canvasObj = new GameObject("Canvas_DuelPause");
            canvasObj.transform.SetParent(transform, false);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1200;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        private void CreateBackground()
        {
            GameObject bgObj = new GameObject("DuelPause_Background");
            bgObj.transform.SetParent(pauseCanvas.transform, false);

            Image bg = bgObj.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.75f);

            RectTransform rect = bgObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void CreateTitle()
        {
            GameObject titleObj = new GameObject("DuelPause_Title");
            titleObj.transform.SetParent(pauseCanvas.transform, false);

            Text title = titleObj.AddComponent<Text>();
            title.text = themeData != null
                ? "PAUSA — " + themeData.themeName
                : "PAUSA DUELO";
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 46;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = Color.white;

            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(600f, 80f);
            rect.anchoredPosition = new Vector2(0f, 150f);
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
            rect.sizeDelta = new Vector2(320f, 52f);
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

        private void OnAbandonClicked()
        {
            Time.timeScale = 1f;

            if (pauseCanvas != null)
                Destroy(pauseCanvas.gameObject);

            if (DuelResultManager.Instance != null)
                DuelResultManager.Instance.AbandonDuel();

            ModuleZ.Core.Managers.ModuleZGameState.IsPaused = false;
        }
    }
}