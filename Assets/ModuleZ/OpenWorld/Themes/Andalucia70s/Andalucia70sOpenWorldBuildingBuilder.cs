using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Andalucia70s
{
    public class Andalucia70sOpenWorldBuildingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateWhiteHouses();
            CreateMainPatioBuilding();
            CreateArches();

            Debug.Log("[Module Z] Edificios OpenWorld Andalucía años 70 creados.");
        }

        private void CreateWhiteHouses()
        {
            CreateBuilding("Andalucia_Casa_Blanca_A", new Vector3(-28f, 3f, 0f), new Vector3(8f, 6f, 24f));
            CreateBuilding("Andalucia_Casa_Blanca_B", new Vector3(28f, 3f, 0f), new Vector3(8f, 6f, 24f));
            CreateBuilding("Andalucia_Casa_Blanca_C", new Vector3(0f, 3f, 28f), new Vector3(24f, 6f, 8f));
        }

        private void CreateMainPatioBuilding()
        {
            CreateBuilding(
                "Andalucia_Patio_Principal",
                new Vector3(0f, 2.5f, -28f),
                new Vector3(24f, 5f, 8f)
            );

            CreateCube(
                "Andalucia_Cartel_Patio",
                new Vector3(0f, 4.7f, -23.9f),
                new Vector3(8f, 1f, 0.12f),
                new Color(0.18f, 0.45f, 0.75f)
            );
        }

        private void CreateBuilding(string name, Vector3 position, Vector3 scale)
        {
            CreateCube(name, position, scale, new Color(0.96f, 0.94f, 0.86f));
            CreateWindows(position, scale);
            CreateDoor(position, scale);
        }

        private void CreateWindows(Vector3 buildingPosition, Vector3 buildingScale)
        {
            for (int i = 0; i < 2; i++)
            {
                float y = 1.7f + i * 1.5f;

                CreateCube(
                    "Andalucia_Window_Azul",
                    new Vector3(buildingPosition.x - 2f, y, buildingPosition.z - buildingScale.z * 0.5f - 0.08f),
                    new Vector3(1.1f, 0.7f, 0.08f),
                    new Color(0.18f, 0.45f, 0.75f)
                );

                CreateCube(
                    "Andalucia_Window_Azul",
                    new Vector3(buildingPosition.x + 2f, y, buildingPosition.z - buildingScale.z * 0.5f - 0.08f),
                    new Vector3(1.1f, 0.7f, 0.08f),
                    new Color(0.18f, 0.45f, 0.75f)
                );
            }
        }

        private void CreateDoor(Vector3 buildingPosition, Vector3 buildingScale)
        {
            CreateCube(
                "Andalucia_Door",
                new Vector3(buildingPosition.x, 0.8f, buildingPosition.z - buildingScale.z * 0.5f - 0.08f),
                new Vector3(1.4f, 1.6f, 0.1f),
                new Color(0.32f, 0.14f, 0.04f)
            );
        }

        private void CreateArches()
        {
            CreateArch(new Vector3(-8f, 1.5f, -23.8f));
            CreateArch(new Vector3(0f, 1.5f, -23.8f));
            CreateArch(new Vector3(8f, 1.5f, -23.8f));
        }

        private void CreateArch(Vector3 position)
        {
            Color color = new Color(0.98f, 0.96f, 0.88f);

            CreateCube("Andalucia_Arco_Pilar_Izq", position + new Vector3(-1f, 0f, 0f), new Vector3(0.35f, 2.5f, 0.35f), color);
            CreateCube("Andalucia_Arco_Pilar_Der", position + new Vector3(1f, 0f, 0f), new Vector3(0.35f, 2.5f, 0.35f), color);
            CreateCube("Andalucia_Arco_Superior", position + new Vector3(0f, 1.25f, 0f), new Vector3(2.4f, 0.35f, 0.35f), color);
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