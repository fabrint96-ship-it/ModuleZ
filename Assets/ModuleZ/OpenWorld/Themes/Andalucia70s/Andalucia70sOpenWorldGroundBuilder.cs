using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Andalucia70s
{
    public class Andalucia70sOpenWorldGroundBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateBaseGround();
            CreateCentralPatio();
            CreateAndalusianTiles();

            Debug.Log("[Module Z] Suelo OpenWorld Andalucía años 70 creado.");
        }

        private void CreateBaseGround()
        {
            CreateCube(
                "Ground_Andalucia_70s",
                new Vector3(0f, -0.1f, 0f),
                new Vector3(90f, 0.2f, 90f),
                new Color(0.88f, 0.84f, 0.72f)
            );
        }

        private void CreateCentralPatio()
        {
            CreateCube(
                "Andalucia_Patio_Central",
                new Vector3(0f, 0.02f, 0f),
                new Vector3(34f, 0.08f, 34f),
                new Color(0.95f, 0.92f, 0.84f)
            );
        }

        private void CreateAndalusianTiles()
        {
            Color blue = new Color(0.18f, 0.45f, 0.75f);
            Color white = new Color(0.96f, 0.95f, 0.90f);

            for (int x = -16; x <= 16; x += 2)
            {
                for (int z = -16; z <= 16; z += 2)
                {
                    Color tileColor =
                        ((x + z) % 4 == 0)
                        ? blue
                        : white;

                    CreateCube(
                        "Andalucia_Azulejo",
                        new Vector3(x, 0.11f, z),
                        new Vector3(1.5f, 0.04f, 1.5f),
                        tileColor
                    );
                }
            }
        }

        private GameObject CreateCube(
            string name,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            cube.name = name;
            cube.transform.position = position;
            cube.transform.localScale = scale;

            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material.color = color;

            return cube;
        }
    }
}