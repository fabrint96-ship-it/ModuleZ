using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.Duel3D.AI
{
    public class Duel3DAIDebugInfo : MonoBehaviour
    {
        private Text debugText;
        private Canvas canvas;
        private bool visible = true;

        public void Build()
        {
            GameObject canvasObj = new GameObject("Duel3D_AI_DebugCanvas");

            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject textObj = new GameObject("AI_DebugText");
            textObj.transform.SetParent(canvas.transform, false);

            RectTransform rect = textObj.AddComponent<RectTransform>();

            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);

            rect.anchoredPosition = new Vector2(15f, -15f);
            rect.sizeDelta = new Vector2(500f, 250f);

            debugText = textObj.AddComponent<Text>();
            debugText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            debugText.fontSize = 16;
            debugText.alignment = TextAnchor.UpperLeft;
            debugText.color = Color.yellow;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10))
                Toggle();
        }

        public void UpdateInfo(
            float progress01,
            Duel3DAISettings settings)
        {
            if (debugText == null || settings == null)
                return;

            debugText.text =
                $"IA Progress: {(progress01 * 100f):0}%\n" +
                $"SearchDepth: {settings.searchDepth}\n" +
                $"Randomness: {settings.randomness:0.00}\n" +
                $"ReactionDelay: {settings.reactionDelay:0.00}\n" +
                $"MaxMoves: {settings.maxMovesEvaluated}\n" +
                $"BlockWeight: {settings.blockPlayerWeight:0.00}\n" +
                $"ClearWeight: {settings.clearOwnColorWeight:0.00}\n" +
                $"HeightWeight: {settings.verticalControlWeight:0.00}\n" +
                $"CenterWeight: {settings.centerControlWeight:0.00}\n" +
                $"RiskyMoves: {settings.allowRiskyMoves}";
        }

        private void Toggle()
        {
            visible = !visible;

            if (canvas != null)
                canvas.gameObject.SetActive(visible);
        }
    }
}