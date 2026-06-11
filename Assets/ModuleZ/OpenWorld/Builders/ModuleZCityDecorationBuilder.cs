using UnityEngine;

namespace ModuleZ.OpenWorld.Builders
{
    public static class ModuleZCityDecorationBuilder
    {
        public static GameObject CreateBench(Vector3 position, ModuleZCityDecorationTheme theme)
        {
            CreateCube("City_Bench_Seat", position, new Vector3(2.4f, 0.25f, 0.75f), theme.benchSeatColor);
            return CreateCube("City_Bench_Back", position + new Vector3(0f, 0.45f, 0.35f), new Vector3(2.4f, 0.7f, 0.18f), theme.benchBackColor);
        }

        public static GameObject CreateStreetLamp(Vector3 position, ModuleZCityDecorationTheme theme)
        {
            CreateCube("City_Lamp_Post", position + new Vector3(0f, 1.5f, 0f), new Vector3(0.15f, 3f, 0.15f), theme.lampPostColor);
            return CreateCube("City_Lamp_Light", position + new Vector3(0f, 3.1f, 0f), new Vector3(0.4f, 0.4f, 0.4f), theme.lampLightColor);
        }

        public static GameObject CreatePlanter(Vector3 position, ModuleZCityDecorationTheme theme)
        {
            CreateCube("City_Planter_Base", position, new Vector3(1.4f, 0.6f, 1.4f), theme.planterBaseColor);
            return CreateCube("City_Planter_Plant", position + new Vector3(0f, 0.55f, 0f), new Vector3(1f, 0.5f, 1f), theme.plantColor);
        }

        public static GameObject CreateSign(
            string name,
            string text,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            GameObject sign = CreateCube(name, position, scale, color);

            GameObject textObj = new GameObject(name + "_Text");
            textObj.transform.SetParent(sign.transform, false);

            // Texto delante del cartel, no detrás
            textObj.transform.localPosition = new Vector3(0f, 0f, -0.62f);
            textObj.transform.localRotation = Quaternion.identity;

            TextMesh mesh = textObj.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.fontSize = 28;
            mesh.characterSize = 0.06f;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.color = Color.black;

            return sign;
        }

        private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.position = position;
            cube.transform.localScale = scale;

            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material.color = color;

            return cube;
        }

        public static void BuildPreset(
            ModuleZCityDecorationPreset preset,
            ModuleZCityDecorationTheme theme)
        {
            if (preset.signs != null)
            {
                foreach (CitySignData sign in preset.signs)
                {
                    CreateSign(
                        sign.name,
                        sign.text,
                        sign.position,
                        sign.scale,
                        sign.color
                    );
                }
            }

            if (preset == null || theme == null)
                return;

            if (preset.benchPositions != null)
            {
                foreach (Vector3 position in preset.benchPositions)
                    CreateBench(position, theme);
            }

            if (preset.lampPositions != null)
            {
                foreach (Vector3 position in preset.lampPositions)
                    CreateStreetLamp(position, theme);
            }

            if (preset.planterPositions != null)
            {
                foreach (Vector3 position in preset.planterPositions)
                    CreatePlanter(position, theme);
            }
        }
    }
}