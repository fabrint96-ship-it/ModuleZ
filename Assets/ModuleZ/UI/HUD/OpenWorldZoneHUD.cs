using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.UI.HUD
{
    public class OpenWorldZoneHUD : MonoBehaviour
    {
        private Text zoneText;

        private void Start()
        {
            CreateHUD();
            Refresh();
        }

        private void CreateHUD()
        {
            GameObject canvasObj = new GameObject("Canvas_OpenWorldZoneHUD");
            canvasObj.transform.SetParent(transform, false);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 410;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject textObj = new GameObject("OpenWorld_Zone_Text");
            textObj.transform.SetParent(canvasObj.transform, false);

            zoneText = textObj.AddComponent<Text>();
            zoneText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            zoneText.fontSize = 28;
            zoneText.alignment = TextAnchor.MiddleRight;
            zoneText.color = Color.white;

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.sizeDelta = new Vector2(500f, 60f);
            rect.anchoredPosition = new Vector2(-30f, -30f);
        }

        public void Refresh()
        {
            OpenWorldThemeData themeData =
                OpenWorldThemeDatabase.GetThemeData(ModuleZGameState.CurrentOpenWorldTheme);

            zoneText.text = "Zona: " + themeData.zoneDisplayName;
        }
    }
}