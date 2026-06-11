using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Valencia70s
{
    public class Valencia70sOpenWorldGroundBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateBaseGround();
            CreateCentralPromenade();
            CreateMediterraneanTiles();

            Debug.Log("[Module Z] Suelo OpenWorld Valencia años 70 creado.");
        }

        private void CreateBaseGround()
        {
            CreateCube(
                "Ground_Valencia_70s",
                new Vector3(0f, -0.1f, 0f),
                new Vector3(80f, 0.2f, 80f),
                new Color(0.82f, 0.74f, 0.58f)
            );
        }

        private void CreateCentralPromenade()
        {
            CreateCube(
                "Valencia_Paseo_Central",
                new Vector3(0f, 0.02f, 0f),
                new Vector3(16f, 0.08f, 60f),
                new Color(0.92f, 0.86f, 0.72f)
            );
        }

        private void CreateMediterraneanTiles()
        {
            Color orange = new Color(0.95f, 0.55f, 0.15f);
            Color cream = new Color(0.95f, 0.88f, 0.72f);

            for (int x = -14; x <= 14; x += 2)
            {
                for (int z = -28; z <= 28; z += 2)
                {
                    Color tileColor = ((x + z) % 4 == 0)
                        ? orange
                        : cream;

                    CreateCube(
                        "Valencia_Tile",
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