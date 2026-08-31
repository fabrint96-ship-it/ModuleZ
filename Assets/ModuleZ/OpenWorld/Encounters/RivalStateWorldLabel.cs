using ModuleZ.Core.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.OpenWorld.Encounters
{
    public class RivalStateWorldLabel : MonoBehaviour
    {
        [SerializeField] private ModuleZRivalId rivalId;

        private Canvas canvas;
        private Text labelText;
        private Vector3 baseLocalPosition;

        private void Start()
        {
            BuildLabel();
            Refresh();
        }

        private void Update()
        {
            FaceCamera();
            AnimateFloat();
        }

        public void SetRival(ModuleZRivalId rival)
        {
            rivalId = rival;

            if (labelText != null)
                Refresh();
        }

        private void BuildLabel()
        {
            GameObject canvasObj = new GameObject("RivalStateCanvas");
            canvasObj.transform.SetParent(transform, false);
            canvasObj.transform.localPosition = new Vector3(0f, 2.35f, 0f);

            baseLocalPosition = canvasObj.transform.localPosition;

            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 30f;

            canvasObj.AddComponent<GraphicRaycaster>();

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(320f, 95f);
            canvasRect.localScale = Vector3.one * 0.01f;

            GameObject bgObj = new GameObject("LabelBackground");
            bgObj.transform.SetParent(canvasObj.transform, false);

            Image bg = bgObj.AddComponent<Image>();
            bg.color = ModuleZ70sPalette.UIBackground;

            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            GameObject textObj = new GameObject("StateText");
            textObj.transform.SetParent(canvasObj.transform, false);

            labelText = textObj.AddComponent<Text>();
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.fontSize = 26;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.fontStyle = FontStyle.Bold;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        public void Refresh()
        {
            if (labelText == null)
                return;

            if (ModuleZRivalProgression.IsRivalDefeated(rivalId))
            {
                labelText.text = "✓ DERROTADO\nE: REMATCH";
                labelText.color = new Color(0.55f, 0.75f, 1f);
                return;
            }

            if (ModuleZRivalProgression.IsRivalUnlocked(rivalId))
            {
                labelText.text = "⚔ DISPONIBLE";
                labelText.color = new Color(0.4f, 1f, 0.4f);
                return;
            }

            labelText.text = "🔒 BLOQUEADO";
            labelText.color = new Color(1f, 0.45f, 0.45f);
        }

        private void FaceCamera()
        {
            if (canvas == null)
                return;

            Camera cam = Camera.main;

            if (cam == null)
                return;

            canvas.transform.rotation = Quaternion.LookRotation(
                canvas.transform.position - cam.transform.position
            );
        }

        private void AnimateFloat()
        {
            if (canvas == null)
                return;

            float offset = Mathf.Sin(Time.time * 2f) * 0.08f;

            canvas.transform.localPosition =
                baseLocalPosition + Vector3.up * offset;
        }
    }
}