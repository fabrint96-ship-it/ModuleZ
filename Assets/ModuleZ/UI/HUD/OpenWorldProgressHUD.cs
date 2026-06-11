using ModuleZ.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.UI.HUD
{
    public class OpenWorldProgressHUD : MonoBehaviour
    {
        private Text progressText;

        private void Start()
        {
            CreateHUD();
            Refresh();
        }

        private void CreateHUD()
        {
            GameObject canvasObj = new GameObject("Canvas_OpenWorldProgressHUD");
            canvasObj.transform.SetParent(transform, false);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 400;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject textObj = new GameObject("OpenWorld_Progress_Text");
            textObj.transform.SetParent(canvasObj.transform, false);

            progressText = textObj.AddComponent<Text>();
            progressText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            progressText.fontSize = 28;
            progressText.alignment = TextAnchor.MiddleLeft;
            progressText.color = Color.white;

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(900f, 100f);
            rect.anchoredPosition = new Vector2(30f, -30f);
        }

        public void Refresh()
        {
            if (progressText == null)
                return;

            progressText.text =
                "Ganados: " + ModuleZGameState.DuelsWon +
                " | Perdidos: " + ModuleZGameState.DuelsLost +
                " | Abandonados: " + ModuleZGameState.DuelsAbandoned +
                "\nMadrid: " + GetStatus(ModuleZGameState.RivalMadridDefeated) +
                " | Barcelona: " + GetStatus(ModuleZGameState.RivalBarcelonaDefeated) +
                " | Valencia: " + GetStatus(ModuleZGameState.RivalValenciaDefeated);
        }

        private string GetStatus(bool defeated)
        {
            return defeated ? "Derrotado" : "Pendiente";
        }
    }
}