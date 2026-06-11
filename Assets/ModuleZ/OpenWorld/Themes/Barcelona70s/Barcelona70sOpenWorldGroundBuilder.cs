using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Barcelona70s
{
    public class Barcelona70sOpenWorldGroundBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateBaseGround();
            CreateMainPlaza();
            CreateMosaicTiles();

            Debug.Log("[Module Z] Suelo OpenWorld Barcelona años 70 creado.");
        }

        private void CreateBaseGround()
        {
            CreateCube(
                "Ground_Barcelona_70s",
                new Vector3(0f, -0.1f, 0f),
                new Vector3(80f, 0.2f, 80f),
                new Color(0.70f, 0.64f, 0.54f)
            );
        }

        private void CreateMainPlaza()
        {
            CreateCube(
                "Plaza_Barcelona_Mediterranea",
                new Vector3(0f, 0.02f, 0f),
                new Vector3(30f, 0.08f, 30f),
                new Color(0.78f, 0.72f, 0.62f)
            );
        }

        private void CreateMosaicTiles()
        {
            Color blue = new Color(0.20f, 0.55f, 0.85f);
            Color cream = new Color(0.86f, 0.80f, 0.68f);

            for (int x = -14; x <= 14; x += 2)
            {
                for (int z = -14; z <= 14; z += 2)
                {
                    Color color = ((x + z) % 4 == 0) ? blue : cream;

                    CreateCube(
                        "Barcelona_Mosaic_Tile",
                        new Vector3(x, 0.11f, z),
                        new Vector3(1.6f, 0.04f, 1.6f),
                        color
                    );
                }
            }
        }

        private GameObject CreateCube(string name, Vector3 position, Vector3 scale, Color color)
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