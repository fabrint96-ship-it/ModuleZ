using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Valencia70s
{
    public class Valencia70sOpenWorldBuildingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateMarketBuilding();
            CreateResidentialBlocks();
            CreateShops();

            Debug.Log("[Module Z] Edificios OpenWorld Valencia años 70 creados.");
        }

        private void CreateMarketBuilding()
        {
            CreateBuilding(
                "Valencia_Mercado_70s",
                new Vector3(0f, 3f, 28f),
                new Vector3(18f, 6f, 8f),
                new Color(0.86f, 0.76f, 0.56f)
            );

            CreateCube(
                "Valencia_Mercado_Cartel",
                new Vector3(0f, 5.2f, 23.9f),
                new Vector3(8f, 1f, 0.12f),
                new Color(0.95f, 0.55f, 0.15f)
            );
        }

        private void CreateResidentialBlocks()
        {
            CreateBuilding(
                "Valencia_Residencial_Izq",
                new Vector3(-26f, 3f, 0f),
                new Vector3(8f, 6f, 26f),
                new Color(0.88f, 0.80f, 0.62f)
            );

            CreateBuilding(
                "Valencia_Residencial_Der",
                new Vector3(26f, 3f, 0f),
                new Vector3(8f, 6f, 26f),
                new Color(0.92f, 0.84f, 0.66f)
            );
        }

        private void CreateShops()
        {
            CreateBuilding(
                "Valencia_Tienda_Naranja",
                new Vector3(-10f, 2.5f, -28f),
                new Vector3(10f, 5f, 7f),
                new Color(0.90f, 0.65f, 0.35f)
            );

            CreateBuilding(
                "Valencia_Tienda_Ceramica",
                new Vector3(10f, 2.5f, -28f),
                new Vector3(10f, 5f, 7f),
                new Color(0.82f, 0.74f, 0.58f)
            );
        }

        private void CreateBuilding(
            string name,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            GameObject building = CreateCube(name, position, scale, color);

            CreateWindows(position, scale);
            CreateDoor(position, scale);
        }

        private void CreateWindows(Vector3 buildingPosition, Vector3 buildingScale)
        {
            for (int i = 0; i < 3; i++)
            {
                float y = 1.6f + i * 1.4f;

                CreateCube(
                    "Valencia_Window",
                    new Vector3(
                        buildingPosition.x - 2f,
                        y,
                        buildingPosition.z - buildingScale.z * 0.5f - 0.08f
                    ),
                    new Vector3(1.1f, 0.7f, 0.08f),
                    new Color(0.18f, 0.38f, 0.55f)
                );

                CreateCube(
                    "Valencia_Window",
                    new Vector3(
                        buildingPosition.x + 2f,
                        y,
                        buildingPosition.z - buildingScale.z * 0.5f - 0.08f
                    ),
                    new Vector3(1.1f, 0.7f, 0.08f),
                    new Color(0.18f, 0.38f, 0.55f)
                );
            }
        }

        private void CreateDoor(Vector3 buildingPosition, Vector3 buildingScale)
        {
            CreateCube(
                "Valencia_Door",
                new Vector3(
                    buildingPosition.x,
                    0.8f,
                    buildingPosition.z - buildingScale.z * 0.5f - 0.08f
                ),
                new Vector3(1.4f, 1.6f, 0.1f),
                new Color(0.35f, 0.16f, 0.06f)
            );
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