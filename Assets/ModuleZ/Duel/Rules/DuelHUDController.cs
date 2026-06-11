using ModuleZ.Duel.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.Duel.Rules
{
    public class DuelHUDController : MonoBehaviour
    {
        public static DuelHUDController Instance { get; private set; }

        private Text messageText;
        private Text controlsText;
        private Text scoreText;
        private Text timerText;

        private Image messageBackground;

        private void Awake()
        {
            Instance = this;
            CreateHUD();
        }

        private void CreateHUD()
        {
            GameObject canvasObj = new GameObject("Canvas_DuelHUD");

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            CreateMessageText(canvasObj.transform);
            CreateControlsText(canvasObj.transform);
            CreateScoreText(canvasObj.transform);
            CreateTimerText(canvasObj.transform);
        }

        private void CreateMessageText(Transform parent)
        {
            GameObject bgObj = new GameObject("Duel_Message_Background");
            bgObj.transform.SetParent(parent, false);

            messageBackground = bgObj.AddComponent<Image>();
            messageBackground.color = new Color(0f, 0f, 0f, 0.65f);

            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.5f, 1f);
            bgRect.anchorMax = new Vector2(0.5f, 1f);
            bgRect.pivot = new Vector2(0.5f, 1f);
            bgRect.sizeDelta = new Vector2(950f, 80f);
            bgRect.anchoredPosition = new Vector2(0f, -25f);

            GameObject textObj = new GameObject("Duel_Message_Text");
            textObj.transform.SetParent(bgObj.transform, false);

            messageText = textObj.AddComponent<Text>();
            messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            messageText.fontSize = 38;
            messageText.alignment = TextAnchor.MiddleCenter;
            messageText.color = Color.white;
            messageText.text = "";

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void CreateControlsText(Transform parent)
        {
            GameObject controlsObj = new GameObject("Duel_Controls_Text");
            controlsObj.transform.SetParent(parent, false);

            controlsText = controlsObj.AddComponent<Text>();
            controlsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            controlsText.fontSize = 26;
            controlsText.alignment = TextAnchor.MiddleRight;
            controlsText.color = Color.white;
            controlsText.text = "Flechas: mover pieza Z\nQ/E: rotar pieza Z\nESC: salir del duelo";

            RectTransform rect = controlsObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.sizeDelta = new Vector2(500f, 120f);
            rect.anchoredPosition = new Vector2(-30f, 30f);
        }

        private void CreateScoreText(Transform parent)
        {
            GameObject scoreObj = new GameObject("Duel_Score_Text");
            scoreObj.transform.SetParent(parent, false);

            scoreText = scoreObj.AddComponent<Text>();
            scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            scoreText.fontSize = 28;
            scoreText.alignment = TextAnchor.MiddleLeft;
            scoreText.color = Color.white;

            RectTransform rect = scoreObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(700f, 60f);
            rect.anchoredPosition = new Vector2(30f, -30f);

            RefreshScore();
        }

        private void CreateTimerText(Transform parent)
        {
            GameObject timerObj = new GameObject("Duel_Timer_Text");
            timerObj.transform.SetParent(parent, false);

            timerText = timerObj.AddComponent<Text>();
            timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            timerText.fontSize = 34;
            timerText.alignment = TextAnchor.MiddleCenter;
            timerText.color = Color.white;

            RectTransform rect = timerObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(300f, 60f);
            rect.anchoredPosition = new Vector2(0f, -110f);
        }

        public void ShowMessage(string message)
        {
            if (messageText != null)
                messageText.text = message;
        }

        public void ShowThemeLoaded(string themeName)
        {
            ShowMessage("Duelo iniciado — " + themeName);
        }

        public void ShowDuelStart(string themeName, string rivalName)
        {
            ShowMessage("Duelo iniciado — " + themeName + " vs " + rivalName);
        }

        public void ShowDuelIntro(string introMessage, string rivalName)
        {
            ShowMessage(introMessage + " vs " + rivalName);
        }

        public void ShowTimer(float time)
        {
            if (timerText == null)
                return;

            int seconds = Mathf.CeilToInt(Mathf.Max(0f, time));
            timerText.text = "Tiempo: " + seconds;
        }

        public void RefreshScore()
        {
            if (scoreText == null)
                return;

            scoreText.text =
                "Ganados: " + ModuleZ.Core.Managers.ModuleZGameState.DuelsWon +
                " | Perdidos: " + ModuleZ.Core.Managers.ModuleZGameState.DuelsLost +
                " | Abandonados: " + ModuleZ.Core.Managers.ModuleZGameState.DuelsAbandoned;
        }

        public void ApplyTheme(DuelThemeData themeData)
        {
            if (themeData == null)
                return;

            if (messageBackground != null)
                messageBackground.color = themeData.secondaryColor;

            if (messageText != null)
                messageText.color = Color.white;

            if (controlsText != null)
                controlsText.color = themeData.accentColor;

            if (scoreText != null)
                scoreText.color = themeData.accentColor;

            if (timerText != null)
                timerText.color = themeData.accentColor;
        }
    }
}