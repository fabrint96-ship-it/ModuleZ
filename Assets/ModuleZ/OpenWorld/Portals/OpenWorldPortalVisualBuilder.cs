using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.OpenWorld.Portals
{
    public static class OpenWorldPortalVisualBuilder
    {
        public static void BuildPortalVisual(
            Transform parent,
            string destinationName,
            Color portalColor)
        {
            if (parent == null)
                return;

            CreatePedestal(parent, portalColor);
            CreatePortalPillars(parent, portalColor);
            CreatePortalCore(parent, portalColor);
            CreateRotatingDestinationLabel(parent, destinationName, portalColor);
        }

        private static void CreateRotatingDestinationLabel(
            Transform parent,
            string destinationName,
            Color color)
        {
            GameObject signRoot = new GameObject("Portal_RotatingSign");
            signRoot.transform.SetParent(parent, false);
            signRoot.transform.localPosition = new Vector3(0f, 2.45f, 0f);
            signRoot.transform.localScale = Vector3.one * 2.0f;

            signRoot.AddComponent<OpenWorldPortalSignRotator>();

            CreateDestinationLabel(
                signRoot.transform,
                destinationName,
                color
            );
        }

        private static void CreatePedestal(Transform parent, Color color)
        {
            GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pedestal.name = "Portal_Pedestal";
            pedestal.transform.SetParent(parent, false);
            pedestal.transform.localPosition = new Vector3(0f, 0.08f, 0f);
            pedestal.transform.localScale = new Vector3(1.25f, 0.08f, 1.25f);

            Renderer renderer = pedestal.GetComponent<Renderer>();
            renderer.material.color = Darken(color, 0.45f);
        }

        private static void CreatePortalPillars(Transform parent, Color color)
        {
            CreatePillar(parent, new Vector3(-0.65f, 1f, 0f), color);
            CreatePillar(parent, new Vector3(0.65f, 1f, 0f), color);

            GameObject top = GameObject.CreatePrimitive(PrimitiveType.Cube);
            top.name = "Portal_TopBeam";
            top.transform.SetParent(parent, false);
            top.transform.localPosition = new Vector3(0f, 1.95f, 0f);
            top.transform.localScale = new Vector3(1.55f, 0.16f, 0.18f);

            Renderer renderer = top.GetComponent<Renderer>();
            renderer.material.color = color;
        }

        private static void CreatePillar(
            Transform parent,
            Vector3 localPosition,
            Color color)
        {
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pillar.name = "Portal_Pillar";
            pillar.transform.SetParent(parent, false);
            pillar.transform.localPosition = localPosition;
            pillar.transform.localScale = new Vector3(0.16f, 1.8f, 0.16f);

            Renderer renderer = pillar.GetComponent<Renderer>();
            renderer.material.color = color;
        }

        private static void CreatePortalCore(Transform parent, Color color)
        {
            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cube);
            core.name = "Portal_Core";
            core.transform.SetParent(parent, false);
            core.transform.localPosition = new Vector3(0f, 1f, 0.02f);
            core.transform.localScale = new Vector3(0.95f, 1.45f, 0.04f);

            Renderer renderer = core.GetComponent<Renderer>();
            renderer.material = CreateTransparentMaterial(
                new Color(color.r, color.g, color.b, 0.35f)
            );
        }

        private static void CreateDestinationLabel(
            Transform parent,
            string destinationName,
            Color color)
        {
            GameObject canvasObj = new GameObject("Portal_LabelCanvas");
            canvasObj.transform.SetParent(parent, false);
            canvasObj.transform.localPosition = Vector3.zero;
            canvasObj.transform.localScale = Vector3.one * 0.01f;

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 30f;

            canvasObj.AddComponent<GraphicRaycaster>();

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(300f, 70f);

            GameObject textObj = new GameObject("Portal_LabelText");
            textObj.transform.SetParent(canvasObj.transform, false);

            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = destinationName;
            text.fontSize = 28;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        private static Material CreateTransparentMaterial(Color color)
        {
            Shader shader = Shader.Find("Standard");
            Material material = new Material(shader);

            material.color = color;
            material.SetFloat("_Mode", 3);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);

            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

            material.renderQueue = 3000;

            return material;
        }

        private static Color Darken(Color color, float amount)
        {
            return new Color(
                color.r * amount,
                color.g * amount,
                color.b * amount,
                color.a
            );
        }
    }
}