using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.UI.HUD
{
    public class OpenWorldSystemMessageHUD : MonoBehaviour
    {
        public static OpenWorldSystemMessageHUD Instance { get; private set; }

        private Canvas canvas;
        private GameObject backgroundObj;
        private Text messageText;
        private float hideTime;

        private void Awake()
        {
            Instance = this;
            CreateHUD();
        }

        private void Update()
        {
            if (hideTime > 0f && Time.time >= hideTime)
                Hide();
        }

        private void CreateHUD()
        {
            GameObject canvasObj = new GameObject("Canvas_OpenWorldSystemMessageHUD");
            canvasObj.transform.SetParent(transform, false);

            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 2000;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            backgroundObj = new GameObject("System_Message_Background");
            backgroundObj.transform.SetParent(canvasObj.transform, false);

            Image bg = backgroundObj.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.82f);

            RectTransform bgRect = backgroundObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.5f, 0.5f);
            bgRect.anchorMax = new Vector2(0.5f, 0.5f);
            bgRect.pivot = new Vector2(0.5f, 0.5f);
            bgRect.sizeDelta = new Vector2(950f, 120f);
            bgRect.anchoredPosition = Vector2.zero;

            GameObject textObj = new GameObject("System_Message_Text");
            textObj.transform.SetParent(backgroundObj.transform, false);

            messageText = textObj.AddComponent<Text>();
            messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            messageText.fontSize = 38;
            messageText.alignment = TextAnchor.MiddleCenter;
            messageText.color = Color.white;
            messageText.text = "";

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            backgroundObj.SetActive(false);
        }

        public void Show(string message, float duration)
        {
            if (backgroundObj != null)
                backgroundObj.SetActive(true);

            if (messageText != null)
                messageText.text = message;

            hideTime = Time.time + duration;

            Debug.Log("[Module Z System HUD] " + message);
        }

        private void Hide()
        {
            hideTime = 0f;

            if (messageText != null)
                messageText.text = "";

            if (backgroundObj != null)
                backgroundObj.SetActive(false);
        }
    }
}