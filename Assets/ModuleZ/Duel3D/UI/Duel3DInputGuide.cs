using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.Duel3D.UI
{
    public class Duel3DInputGuide : MonoBehaviour
    {
        private Text guideText;

        public void Build()
        {
            Canvas canvas = FindObjectOfType<Canvas>();

            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Duel3D_InputGuideCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            guideText = CreateText(canvas.transform);

            guideText.text =
                "CONTROLES\n" +
                "Flechas: mover pieza X/Z\n" +
                "R / F: subir / bajar altura\n" +
                "Q / E: rotar pieza Z\n" +
                "Espacio: colocar pieza";
        }

        private Text CreateText(Transform parent)
        {
            GameObject obj = new GameObject("Duel3D_InputGuideText");
            obj.transform.SetParent(parent, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(20f, 20f);
            rect.sizeDelta = new Vector2(500f, 150f);

            Text text = obj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 18;
            text.alignment = TextAnchor.LowerLeft;
            text.color = Color.white;

            return text;
        }
    }
}