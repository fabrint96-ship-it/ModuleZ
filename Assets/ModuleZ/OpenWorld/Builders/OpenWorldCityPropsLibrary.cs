using UnityEngine;

namespace ModuleZ.OpenWorld.Builders
{
    public static class OpenWorldCityPropsLibrary
    {
        public static GameObject CreateBench(
            Transform parent,
            Vector3 position,
            Color woodColor,
            Color metalColor)
        {
            GameObject root = new GameObject("Bench");
            root.transform.position = position;

            CreatePart(
                "Seat",
                root.transform,
                new Vector3(0f, 0.45f, 0f),
                new Vector3(1.6f, 0.12f, 0.45f),
                woodColor
            );

            CreatePart(
                "Back",
                root.transform,
                new Vector3(0f, 0.85f, -0.18f),
                new Vector3(1.6f, 0.6f, 0.12f),
                woodColor
            );

            CreatePart(
                "Leg_L",
                root.transform,
                new Vector3(-0.6f, 0.2f, 0f),
                new Vector3(0.08f, 0.4f, 0.08f),
                metalColor
            );

            CreatePart(
                "Leg_R",
                root.transform,
                new Vector3(0.6f, 0.2f, 0f),
                new Vector3(0.08f, 0.4f, 0.08f),
                metalColor
            );

            return root;
        }

        public static GameObject CreateStreetLamp(
            Transform parent,
            Vector3 position,
            Color poleColor,
            Color lightColor)
        {
            GameObject root = new GameObject("StreetLamp");
            root.transform.position = position;

            CreatePart(
                "Pole",
                root.transform,
                new Vector3(0f, 2f, 0f),
                new Vector3(0.15f, 4f, 0.15f),
                poleColor
            );

            CreatePart(
                "Arm",
                root.transform,
                new Vector3(0.45f, 3.75f, 0f),
                new Vector3(0.9f, 0.1f, 0.1f),
                poleColor
            );

            CreatePart(
                "Light",
                root.transform,
                new Vector3(0.9f, 3.55f, 0f),
                new Vector3(0.25f, 0.25f, 0.25f),
                lightColor
            );

            return root;
        }

        public static GameObject CreateSign(
            Transform parent,
            Vector3 position,
            string name,
            Color postColor,
            Color signColor)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;

            CreatePart(
                "Post",
                root.transform,
                new Vector3(0f, 1f, 0f),
                new Vector3(0.12f, 2f, 0.12f),
                postColor
            );

            CreatePart(
                "Board",
                root.transform,
                new Vector3(0f, 1.9f, 0f),
                new Vector3(1.4f, 0.7f, 0.1f),
                signColor
            );

            return root;
        }

        public static GameObject CreateFlowerPot(
            Transform parent,
            Vector3 position,
            Color potColor,
            Color plantColor)
        {
            GameObject root = new GameObject("FlowerPot");
            root.transform.position = position;

            CreatePart(
                "Pot",
                root.transform,
                new Vector3(0f, 0.2f, 0f),
                new Vector3(0.5f, 0.4f, 0.5f),
                potColor
            );

            CreatePart(
                "Plant",
                root.transform,
                new Vector3(0f, 0.7f, 0f),
                new Vector3(0.7f, 0.8f, 0.7f),
                plantColor
            );

            return root;
        }

        public static GameObject CreateCrate(
            Transform parent,
            Vector3 position,
            Color color)
        {
            GameObject root = new GameObject("Crate");
            root.transform.position = position;

            CreatePart(
                "Box",
                root.transform,
                Vector3.zero,
                Vector3.one,
                color
            );

            return root;
        }

        public static GameObject CreateFenceSegment(
            Transform parent,
            Vector3 position,
            Color color)
        {
            GameObject root = new GameObject("Fence");
            root.transform.position = position;

            CreatePart(
                "Post_L",
                root.transform,
                new Vector3(-0.75f, 0.5f, 0f),
                new Vector3(0.1f, 1f, 0.1f),
                color
            );

            CreatePart(
                "Post_R",
                root.transform,
                new Vector3(0.75f, 0.5f, 0f),
                new Vector3(0.1f, 1f, 0.1f),
                color
            );

            CreatePart(
                "Rail_1",
                root.transform,
                new Vector3(0f, 0.35f, 0f),
                new Vector3(1.6f, 0.08f, 0.08f),
                color
            );

            CreatePart(
                "Rail_2",
                root.transform,
                new Vector3(0f, 0.7f, 0f),
                new Vector3(1.6f, 0.08f, 0.08f),
                color
            );

            return root;
        }

        public static GameObject CreateTable(
            Transform parent,
            Vector3 position,
            Color color)
        {
            GameObject root = new GameObject("Table");
            root.transform.position = position;

            CreatePart(
                "Top",
                root.transform,
                new Vector3(0f, 0.8f, 0f),
                new Vector3(1.5f, 0.12f, 1f),
                color
            );

            CreatePart(
                "Leg_1",
                root.transform,
                new Vector3(-0.6f, 0.35f, -0.35f),
                new Vector3(0.08f, 0.7f, 0.08f),
                color
            );

            CreatePart(
                "Leg_2",
                root.transform,
                new Vector3(0.6f, 0.35f, -0.35f),
                new Vector3(0.08f, 0.7f, 0.08f),
                color
            );

            CreatePart(
                "Leg_3",
                root.transform,
                new Vector3(-0.6f, 0.35f, 0.35f),
                new Vector3(0.08f, 0.7f, 0.08f),
                color
            );

            CreatePart(
                "Leg_4",
                root.transform,
                new Vector3(0.6f, 0.35f, 0.35f),
                new Vector3(0.08f, 0.7f, 0.08f),
                color
            );

            return root;
        }

        private static GameObject CreatePart(
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 scale,
            Color color)
        {
            GameObject obj =
                GameObject.CreatePrimitive(PrimitiveType.Cube);

            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPosition;
            obj.transform.localScale = scale;

            Renderer renderer =
                obj.GetComponent<Renderer>();

            renderer.material.color = color;

            return obj;
        }
    }
}