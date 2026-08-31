using System.Collections;
using ModuleZ.Core.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.UI.HUD
{
    public class OpenWorldMessageHUD : MonoBehaviour
    {
        public static OpenWorldMessageHUD Instance { get; private set; }

        private Canvas canvas;
        private GameObject panelObj;
        private Text messageText;
        private Coroutine hideRoutine;
        private bool showingDialogue;
        private bool showingAmbient;

        private void Awake()
        {
            Instance = this;
            BuildHUD();
            HideInstant();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void ShowDialogue(string message, float duration = 3f)
        {
            Show(message, duration);
        }

        public void Show(string message, float duration = 3f)
        {
            if (canvas == null)
                BuildHUD();

            if (hideRoutine != null)
                StopCoroutine(hideRoutine);

            showingDialogue = true;
            messageText.text = message;
            panelObj.SetActive(true);

            hideRoutine = StartCoroutine(HideAfter(duration));
        }

        public bool IsShowingDialogue()
        {
            return showingDialogue;
        }

        private IEnumerator HideAfter(float duration)
        {
            yield return new WaitForSeconds(duration);
            HideInstant();
        }

        private void HideInstant()
        {
            showingDialogue = false;
            showingAmbient = false;

            if (panelObj != null)
                panelObj.SetActive(false);
        }

        public void ShowPrompt(string message)
        {
            if (showingDialogue || showingAmbient)
                return;

            if (canvas == null)
                BuildHUD();

            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
                hideRoutine = null;
            }

            messageText.text = message;
            panelObj.SetActive(true);
        }

        public void HideMessage()
        {
            if (showingDialogue || showingAmbient)
                return;

            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
                hideRoutine = null;
            }

            if (panelObj != null)
                panelObj.SetActive(false);
        }

        private void BuildHUD()
        {
            GameObject canvasObj = new GameObject("OpenWorldMessageHUD");
            canvasObj.transform.SetParent(transform, false);

            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1350;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            CreatePanel(canvas.transform);
            CreateText(panelObj.transform);
        }

        private void CreatePanel(Transform parent)
        {
            panelObj = new GameObject("DialoguePanel");
            panelObj.transform.SetParent(parent, false);

            Image image = panelObj.AddComponent<Image>();
            image.color = new Color(0.02f, 0.04f, 0.08f, 0.90f);

            RectTransform rect = panelObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(0f, 45f);
            rect.sizeDelta = new Vector2(980f, 110f);

            GameObject accentObj = new GameObject("AccentLine");
            accentObj.transform.SetParent(panelObj.transform, false);

            Image accent = accentObj.AddComponent<Image>();
            accent.color = ModuleZ70sPalette.UIAccent;

            RectTransform accentRect = accentObj.GetComponent<RectTransform>();
            accentRect.anchorMin = new Vector2(0f, 0f);
            accentRect.anchorMax = new Vector2(0f, 1f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.sizeDelta = new Vector2(8f, 0f);
            accentRect.anchoredPosition = Vector2.zero;
        }

        private void CreateText(Transform parent)
        {
            GameObject textObj = new GameObject("DialogueText");
            textObj.transform.SetParent(parent, false);

            messageText = textObj.AddComponent<Text>();
            messageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            messageText.fontSize = 27;
            messageText.fontStyle = FontStyle.Bold;
            messageText.alignment = TextAnchor.MiddleCenter;
            messageText.color = ModuleZ70sPalette.UIText;
            messageText.horizontalOverflow = HorizontalWrapMode.Wrap;
            messageText.verticalOverflow = VerticalWrapMode.Truncate;

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(35f, 10f);
            rect.offsetMax = new Vector2(-35f, -10f);
        }

        public bool CanShowAmbientMessage()
        {
            return !showingDialogue && !showingAmbient;
        }

        public void ShowAmbientMessage(string message, float duration = 2.5f)
        {
            if (!CanShowAmbientMessage())
                return;

            if (canvas == null)
                BuildHUD();

            if (hideRoutine != null)
                StopCoroutine(hideRoutine);

            showingAmbient = true;
            showingDialogue = false;

            messageText.text = message;
            panelObj.SetActive(true);

            hideRoutine = StartCoroutine(HideAfter(duration));
        }
    }
}