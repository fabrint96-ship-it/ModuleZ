using ModuleZ.Core.Managers;
using ModuleZ.Core.Theme;
using ModuleZ.Duel3D.Rules;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.UI.PauseMenu
{
    public class DuelPauseMenuController : MonoBehaviour
    {
        private Canvas pauseCanvas;
        private bool isPaused;

        private void Update()
        {
            if (ModuleZGameState.DuelCompleted)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
                TogglePause();
        }

        private void TogglePause()
        {
            if (isPaused)
                HidePauseMenu();
            else
                ShowPauseMenu();
        }

        private void ShowPauseMenu()
        {
            isPaused = true;
            Time.timeScale = 0f;
            ModuleZGameState.IsPaused = true;

            pauseCanvas = CreateCanvas();

            CreateBackground();
            CreateTitle();

            CreateButton(
                "Continuar duelo",
                new Vector2(0f, 35f),
                OnResumeClicked
            );

            CreateButton(
                "Abandonar duelo",
                new Vector2(0f, -35f),
                OnAbandonClicked
            );
        }

        private void HidePauseMenu()
        {
            isPaused = false;
            Time.timeScale = 1f;
            ModuleZGameState.IsPaused = false;

            if (pauseCanvas != null)
                Destroy(pauseCanvas.gameObject);

            pauseCanvas = null;
        }

        private Canvas CreateCanvas()
        {
            GameObject canvasObj = new GameObject("Canvas_Duel3D_Pause");
            canvasObj.transform.SetParent(transform, false);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 2000;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
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
            title.text = "PAUSA DUELO 3D";
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 46;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = ModuleZ70sPalette.UIText;

            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(700f, 80f);
            rect.anchoredPosition = new Vector2(0f, 150f);
        }

        private void CreateButton(
            string text,
            Vector2 position,
            UnityEngine.Events.UnityAction action)
        {
            GameObject buttonObj = new GameObject("Button_" + text);
            buttonObj.transform.SetParent(pauseCanvas.transform, false);

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.12f, 0.18f, 0.28f, 0.95f);

            Button button = buttonObj.AddComponent<Button>();
            button.onClick.AddListener(action);

            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(360f, 58f);
            rect.anchoredPosition = position;

            GameObject labelObj = new GameObject("Text");
            labelObj.transform.SetParent(buttonObj.transform, false);

            Text label = labelObj.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 24;
            label.fontStyle = FontStyle.Bold;
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
            HidePauseMenu();
        }

        private void OnAbandonClicked()
        {
            Time.timeScale = 1f;
            ModuleZGameState.IsPaused = false;

            if (pauseCanvas != null)
                Destroy(pauseCanvas.gameObject);

            pauseCanvas = null;
            isPaused = false;

            if (Duel3DResultManager.Instance != null)
                Duel3DResultManager.Instance.AbandonDuel();
            else
                Debug.LogError("[ModuleZ] No existe Duel3DResultManager.");
        }
    }
}