using ModuleZ.Core.Progression;
using ModuleZ.Core.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.UI.HUD
{
    public class ModuleZStatsHUD : MonoBehaviour
    {
        private Canvas canvas;
        private Text statsText;
        private bool visible;

        public bool IsVisible
        {
            get { return visible; }
        }

        private void Start()
        {
            if (ModuleZHUDOverlayCoordinator.Instance != null)
                ModuleZHUDOverlayCoordinator.Instance.RegisterStatsHUD(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
                Toggle();
        }

        private void Toggle()
        {
            visible = !visible;

            if (visible)
                Show();
            else
                Hide();
        }

        private void Show()
        {
            if (ModuleZHUDOverlayCoordinator.Instance != null)
                ModuleZHUDOverlayCoordinator.Instance.NotifyStatsOpened();

            if (canvas == null)
                BuildHUD();

            Refresh();
            canvas.gameObject.SetActive(true);
        }

        private void Hide()
        {
            visible = false;

            if (canvas != null)
                canvas.gameObject.SetActive(false);

            if (ModuleZHUDOverlayCoordinator.Instance != null)
                ModuleZHUDOverlayCoordinator.Instance.NotifyOverlayClosed();
        }

        public void ForceHide()
        {
            visible = false;

            if (canvas != null)
                canvas.gameObject.SetActive(false);

            if (ModuleZHUDOverlayCoordinator.Instance != null)
                ModuleZHUDOverlayCoordinator.Instance.NotifyOverlayClosed();
        }

        public void ForceHideFromCoordinator()
        {
            visible = false;

            if (canvas != null)
                canvas.gameObject.SetActive(false);
        }

        private void BuildHUD()
        {
            GameObject canvasObj = new GameObject("ModuleZStatsHUD");
            canvasObj.transform.SetParent(transform, false);

            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1170;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            CreateBackground(canvas.transform);
            CreateTitle(canvas.transform);
            CreateText(canvas.transform);

            canvas.gameObject.SetActive(false);
        }

        private void CreateBackground(Transform parent)
        {
            GameObject bgObj = new GameObject("StatsBackground");
            bgObj.transform.SetParent(parent, false);

            Image bg = bgObj.AddComponent<Image>();
            bg.color = ModuleZ70sPalette.UIBackground;

            RectTransform rect = bgObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(760f, 560f);
            rect.anchoredPosition = Vector2.zero;

            GameObject accentObj = new GameObject("AccentLine");
            accentObj.transform.SetParent(bgObj.transform, false);

            Image accent = accentObj.AddComponent<Image>();
            accent.color = ModuleZ70sPalette.UIAccent;

            RectTransform accentRect = accentObj.GetComponent<RectTransform>();
            accentRect.anchorMin = new Vector2(0f, 0f);
            accentRect.anchorMax = new Vector2(0f, 1f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.sizeDelta = new Vector2(8f, 0f);
            accentRect.anchoredPosition = Vector2.zero;
        }

        private void CreateTitle(Transform parent)
        {
            GameObject titleObj = new GameObject("StatsTitle");
            titleObj.transform.SetParent(parent, false);

            Text title = titleObj.AddComponent<Text>();
            title.text = "ESTADÍSTICAS MODULE Z";
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 34;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = ModuleZ70sPalette.UIText;

            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(700f, 60f);
            rect.anchoredPosition = new Vector2(0f, 225f);
        }

        private void CreateText(Transform parent)
        {
            GameObject textObj = new GameObject("StatsText");
            textObj.transform.SetParent(parent, false);

            statsText = textObj.AddComponent<Text>();
            statsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            statsText.fontSize = 24;
            statsText.alignment = TextAnchor.UpperLeft;
            statsText.color = Color.white;
            statsText.horizontalOverflow = HorizontalWrapMode.Wrap;
            statsText.verticalOverflow = VerticalWrapMode.Truncate;

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(650f, 410f);
            rect.anchoredPosition = new Vector2(20f, -20f);
        }

        private void Refresh()
        {
            if (statsText == null)
                return;

            statsText.text =
                ModuleZProgressSummary.GetSummary() +
                "\n\nProgreso total: " +
                ModuleZProgressSummary.GetCompletionPercent() +
                "%" +
                "\n\nK: cerrar";
        }
    }
}