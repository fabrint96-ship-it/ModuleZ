using System.Collections;
using ModuleZ.Core.Achievements;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.UI.HUD
{
    public class ModuleZAchievementToastHUD : MonoBehaviour
    {
        private Canvas canvas;
        private Text toastText;
        private Coroutine currentRoutine;

        private void Awake()
        {
            BuildHUD();
            Hide();

            ModuleZAchievementManager.OnAchievementUnlocked += ShowAchievement;
        }

        private void OnDestroy()
        {
            ModuleZAchievementManager.OnAchievementUnlocked -= ShowAchievement;
        }

        private void ShowAchievement(ModuleZAchievementId achievementId)
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);

            currentRoutine = StartCoroutine(
                ShowRoutine(ModuleZAchievementDisplayNames.Get(achievementId))
            );
        }

        private IEnumerator ShowRoutine(string achievementName)
        {
            canvas.gameObject.SetActive(true);

            toastText.text =
                "LOGRO DESBLOQUEADO\n" +
                achievementName;

            yield return new WaitForSeconds(3.2f);

            Hide();
        }

        private void Hide()
        {
            if (canvas != null)
                canvas.gameObject.SetActive(false);
        }

        private void BuildHUD()
        {
            GameObject canvasObj = new GameObject("ModuleZ_AchievementToastHUD");
            canvasObj.transform.SetParent(transform, false);

            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1500;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            CreatePanel(canvas.transform);
            CreateText(canvas.transform);
        }

        private void CreatePanel(Transform parent)
        {
            GameObject panelObj = new GameObject("AchievementToastPanel");
            panelObj.transform.SetParent(parent, false);

            Image image = panelObj.AddComponent<Image>();
            image.color = new Color(0.05f, 0.08f, 0.12f, 0.92f);

            RectTransform rect = panelObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-30f, -30f);
            rect.sizeDelta = new Vector2(430f, 105f);
        }

        private void CreateText(Transform parent)
        {
            GameObject textObj = new GameObject("AchievementToastText");
            textObj.transform.SetParent(parent, false);

            toastText = textObj.AddComponent<Text>();
            toastText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            toastText.fontSize = 22;
            toastText.alignment = TextAnchor.MiddleCenter;
            toastText.color = Color.white;

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-30f, -30f);
            rect.sizeDelta = new Vector2(430f, 105f);
        }
    }
}