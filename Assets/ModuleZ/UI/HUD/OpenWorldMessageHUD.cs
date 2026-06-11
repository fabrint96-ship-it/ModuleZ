using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.UI.HUD
{
    public class OpenWorldMessageHUD : MonoBehaviour
    {
        public static OpenWorldMessageHUD Instance { get; private set; }

        private bool hudCreated;

        private GameObject backgroundObj;
        private Text messageText;

        private float hideTime;
        private bool isDialogueMessage;

        private GameObject ambientBackgroundObj;
        private Text ambientText;
        private float ambientHideTime;

        private void Awake()
        {
            Instance = this;
            CreateHUD();
        }

        private void Update()
        {
            if (hideTime > 0f && Time.time >= hideTime)
                HideMessage();

            if (ambientHideTime > 0f && Time.time >= ambientHideTime)
                HideAmbientMessage();
        }

        private void CreateHUD()
        {
            if (hudCreated)
                return;

            hudCreated = true;

            GameObject canvasObj = new GameObject("Canvas_OpenWorldMessageHUD");
            canvasObj.transform.SetParent(transform, false);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 500;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            CreateMainMessageHUD(canvasObj.transform);
            CreateAmbientHUD(canvasObj.transform);
        }

        private void CreateMainMessageHUD(Transform canvasParent)
        {
            backgroundObj = new GameObject("Message_Background");
            backgroundObj.transform.SetParent(canvasParent, false);

            Image bg = backgroundObj.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.65f);

            RectTransform bgRect = backgroundObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.5f, 0.12f);
            bgRect.anchorMax = new Vector2(0.5f, 0.12f);
            bgRect.pivot = new Vector2(0.5f, 0.5f);
            bgRect.sizeDelta = new Vector2(900f, 90f);
            bgRect.anchoredPosition = Vector2.zero;

            GameObject textObj = new GameObject("OpenWorld_Message_Text");
            textObj.transform.SetParent(backgroundObj.transform, false);

            messageText = textObj.AddComponent<Text>();
            messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            messageText.fontSize = 34;
            messageText.alignment = TextAnchor.MiddleCenter;
            messageText.color = Color.white;
            messageText.text = "";

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            backgroundObj.SetActive(false);
        }

        private void CreateAmbientHUD(Transform canvasParent)
        {
            ambientBackgroundObj = new GameObject("Ambient_Message_Background");
            ambientBackgroundObj.transform.SetParent(canvasParent, false);

            Image bg = ambientBackgroundObj.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.35f);

            RectTransform bgRect = ambientBackgroundObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.5f, 0.28f);
            bgRect.anchorMax = new Vector2(0.5f, 0.28f);
            bgRect.pivot = new Vector2(0.5f, 0.5f);
            bgRect.sizeDelta = new Vector2(800f, 60f);
            bgRect.anchoredPosition = Vector2.zero;

            GameObject textObj = new GameObject("Ambient_Message_Text");
            textObj.transform.SetParent(ambientBackgroundObj.transform, false);

            ambientText = textObj.AddComponent<Text>();
            ambientText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ambientText.fontSize = 24;
            ambientText.alignment = TextAnchor.MiddleCenter;
            ambientText.color = Color.white;
            ambientText.text = "";

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            ambientBackgroundObj.SetActive(false);
        }

        public void ShowPrompt(string message)
        {
            if (isDialogueMessage)
                return;

            ShowInternal(message, 0f, false);
        }

        public void ShowDialogue(string message, float duration = 2.5f)
        {
            ShowInternal(message, duration, true);
        }

        private void ShowInternal(string message, float duration, bool dialogue)
        {
            CreateHUD();

            isDialogueMessage = dialogue;

            if (backgroundObj != null)
                backgroundObj.SetActive(true);

            if (messageText != null)
                messageText.text = message;

            hideTime = duration > 0f ? Time.time + duration : 0f;
        }

        public void HideMessage()
        {
            if (messageText != null)
                messageText.text = "";

            hideTime = 0f;
            isDialogueMessage = false;

            if (backgroundObj != null)
                backgroundObj.SetActive(false);
        }

        public void ShowAmbientMessage(string message, float duration = 2.5f)
        {
            if (!CanShowAmbientMessage())
                return;

            CreateHUD();

            if (ambientBackgroundObj != null)
                ambientBackgroundObj.SetActive(true);

            if (ambientText != null)
                ambientText.text = message;

            ambientHideTime = Time.time + duration;
        }

        private void HideAmbientMessage()
        {
            if (ambientText != null)
                ambientText.text = "";

            ambientHideTime = 0f;

            if (ambientBackgroundObj != null)
                ambientBackgroundObj.SetActive(false);
        }

        public bool IsShowingDialogue()
        {
            return isDialogueMessage;
        }

        public bool CanShowAmbientMessage()
        {
            return !isDialogueMessage;
        }
    }
}