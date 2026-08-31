using ModuleZ.Core.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.OpenWorld.Encounters
{
    public static class ModuleZRivalWorldHUDBuilder
    {
        public static GameObject Build(
            Transform rival,
            ModuleZRivalId rivalId)
        {
            GameObject root = new GameObject("RivalWorldHUD_" + rivalId);
            root.transform.SetParent(rival, false);
            root.transform.localPosition = new Vector3(0f, 3.0f, 0f);

            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 50;

            CanvasScaler scaler = root.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 12f;

            root.AddComponent<GraphicRaycaster>();
            root.AddComponent<ModuleZRivalWorldHUDBillboard>();

            RectTransform canvasRect = root.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(320f, 145f);
            canvasRect.localScale = new Vector3(0.008f, 0.008f, 0.008f);

            Text status;
            Text name;
            Text description;
            Text action;

            CreatePanel(
                root.transform,
                rivalId,
                out status,
                out name,
                out description,
                out action
            );

            ModuleZRivalWorldHUDController controller =
                root.AddComponent<ModuleZRivalWorldHUDController>();

            controller.Initialize(
                rivalId,
                status,
                name,
                description,
                action
            );

            return root;
        }

        private static void CreatePanel(
            Transform parent,
            ModuleZRivalId rivalId,
            out Text statusText,
            out Text nameText,
            out Text descriptionText,
            out Text actionText)
        {
            GameObject panelObj = new GameObject("Panel");
            panelObj.transform.SetParent(parent, false);

            Image panel = panelObj.AddComponent<Image>();
            panel.color = ModuleZ70sPalette.UIBackground;

            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            CreateAccentLine(panelObj.transform);

            statusText = CreateText(
                "Status",
                panelObj.transform,
                ModuleZRivalHUDTextLibrary.GetRivalStatus(rivalId),
                new Vector2(0f, -10f),
                new Vector2(290f, 30f),
                24,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.CabinaRed
            );

            nameText = CreateText(
                "Name",
                panelObj.transform,
                ModuleZRivalHUDTextLibrary.GetRivalName(rivalId),
                new Vector2(0f, -42f),
                new Vector2(290f, 28f),
                21,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.Cream
            );

            descriptionText = CreateText(
                "Description",
                panelObj.transform,
                ModuleZRivalHUDTextLibrary.GetRivalDescription(rivalId),
                new Vector2(0f, -72f),
                new Vector2(285f, 36f),
                16,
                FontStyle.Normal,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.WarmPaper
            );

            actionText = CreateText(
                "Action",
                panelObj.transform,
                ModuleZRivalHUDTextLibrary.GetRivalAction(rivalId),
                new Vector2(0f, -115f),
                new Vector2(290f, 24f),
                17,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.Orange
            );
        }

        private static void CreateAccentLine(Transform parent)
        {
            GameObject accentObj = new GameObject("AccentLine");
            accentObj.transform.SetParent(parent, false);

            Image accent = accentObj.AddComponent<Image>();
            accent.color = ModuleZ70sPalette.UIAccent;

            RectTransform rect = accentObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(8f, 0f);
            rect.anchoredPosition = Vector2.zero;
        }

        private static Text CreateText(
            string name,
            Transform parent,
            string text,
            Vector2 position,
            Vector2 size,
            int fontSize,
            FontStyle fontStyle,
            TextAnchor alignment,
            Color color)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            Text label = obj.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.alignment = alignment;
            label.color = color;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            return label;
        }
    }
}