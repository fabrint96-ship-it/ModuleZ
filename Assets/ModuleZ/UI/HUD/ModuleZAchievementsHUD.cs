using System.Collections.Generic;
using System.Text;
using ModuleZ.Core.Achievements;
using ModuleZ.Core.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.UI.HUD
{
    public class ModuleZAchievementsHUD : MonoBehaviour
    {
        private Canvas canvas;
        private Text achievementsText;
        private bool visible;

        public bool IsVisible
        {
            get { return visible; }
        }

        private void Start()
        {
            if (ModuleZHUDOverlayCoordinator.Instance != null)
                ModuleZHUDOverlayCoordinator.Instance.RegisterAchievementsHUD(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
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
                ModuleZHUDOverlayCoordinator.Instance.NotifyAchievementsOpened();

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
            GameObject canvasObj = new GameObject("ModuleZAchievementsHUD");
            canvasObj.transform.SetParent(transform, false);

            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1160;

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
            GameObject bgObj = new GameObject("AchievementsBackground");
            bgObj.transform.SetParent(parent, false);

            Image bg = bgObj.AddComponent<Image>();
            bg.color = ModuleZ70sPalette.UIBackground;

            RectTransform rect = bgObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(920f, 620f);
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
            GameObject titleObj = new GameObject("AchievementsTitle");
            titleObj.transform.SetParent(parent, false);

            Text title = titleObj.AddComponent<Text>();
            title.text = "LOGROS MODULE Z";
            title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            title.fontSize = 34;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = ModuleZ70sPalette.UIText;

            RectTransform rect = titleObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(820f, 60f);
            rect.anchoredPosition = new Vector2(0f, 255f);
        }

        private void CreateText(Transform parent)
        {
            GameObject textObj = new GameObject("AchievementsText");
            textObj.transform.SetParent(parent, false);

            achievementsText = textObj.AddComponent<Text>();
            achievementsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            achievementsText.fontSize = 22;
            achievementsText.alignment = TextAnchor.UpperLeft;
            achievementsText.color = Color.white;
            achievementsText.horizontalOverflow = HorizontalWrapMode.Wrap;
            achievementsText.verticalOverflow = VerticalWrapMode.Truncate;

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(820f, 470f);
            rect.anchoredPosition = new Vector2(20f, -20f);
        }

        private void Refresh()
        {
            if (achievementsText == null)
                return;

            List<ModuleZAchievementState> achievements =
                ModuleZAchievementManager.GetAll();

            StringBuilder builder = new StringBuilder();

            int unlockedCount = 0;

            foreach (ModuleZAchievementState achievement in achievements)
            {
                string name =
                    ModuleZAchievementDisplayNames.Get(
                        achievement.achievementId
                    );

                string description =
                    ModuleZAchievementDisplayNames.GetDescription(
                        achievement.achievementId
                    );

                if (achievement.unlocked)
                {
                    unlockedCount++;

                    builder.AppendLine("✓ " + name);
                    builder.AppendLine("   " + description);
                    builder.AppendLine("   " + achievement.unlockedDate);
                }
                else
                {
                    builder.AppendLine("✗ " + name);
                    builder.AppendLine("   " + description);
                }

                builder.AppendLine();
            }

            builder.AppendLine(
                "Desbloqueados: " +
                unlockedCount +
                " / " +
                achievements.Count
            );

            builder.AppendLine();
            builder.AppendLine("L: cerrar");

            achievementsText.text = builder.ToString();
        }
    }
}