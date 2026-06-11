using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Madrid70s
{
    public class Madrid70sBuildingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateBuildings();

            Debug.Log("[Module Z] Edificios Madrid años 70 creados.");
        }

        private void CreateBuildings()
        {
            CreateBuilding(
                "Edificio_Residencial_A",
                new Vector3(-22f, 3f, 0f),
                new Vector3(8f, 6f, 18f),
                new Color(0.66f, 0.58f, 0.48f)
            );

            CreateBuilding(
                "Edificio_Residencial_B",
                new Vector3(22f, 4f, 0f),
                new Vector3(8f, 8f, 18f),
                new Color(0.60f, 0.52f, 0.42f)
            );

            CreateBuilding(
                "Bar_Madrid_70s",
                new Vector3(0f, 2.5f, 22f),
                new Vector3(16f, 5f, 6f),
                new Color(0.52f, 0.38f, 0.25f)
            );

            CreateBuilding(
                "Tienda_Madrid_70s",
                new Vector3(0f, 2.5f, -22f),
                new Vector3(16f, 5f, 6f),
                new Color(0.72f, 0.64f, 0.50f)
            );
        }

        private void CreateBuilding(
            string buildingName,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);

            building.name = buildingName;
            building.transform.position = position;
            building.transform.localScale = scale;

            Renderer renderer = building.GetComponent<Renderer>();
            renderer.material.color = color;

            CreateFrontDoor(position, scale);
            CreateWindows(position, scale);
        }

        private void CreateFrontDoor(Vector3 buildingPosition, Vector3 buildingScale)
        {
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);

            door.name = "Door";

            door.transform.position = new Vector3(
                buildingPosition.x,
                0.8f,
                buildingPosition.z - buildingScale.z * 0.5f - 0.05f
            );

            door.transform.localScale = new Vector3(
                1.4f,
                1.6f,
                0.1f
            );

            Renderer renderer = door.GetComponent<Renderer>();
            renderer.material.color = new Color(0.20f, 0.12f, 0.05f);
        }

        private void CreateWindows(Vector3 buildingPosition, Vector3 buildingScale)
        {
            int floors = Mathf.Max(2, Mathf.RoundToInt(buildingScale.y));

            for (int floor = 0; floor < floors; floor++)
            {
                float y = 1.2f + floor;

                CreateWindow(
                    new Vector3(
                        buildingPosition.x - 2f,
                        y,
                        buildingPosition.z - buildingScale.z * 0.5f - 0.08f
                    )
                );

                CreateWindow(
                    new Vector3(
                        buildingPosition.x + 2f,
                        y,
                        buildingPosition.z - buildingScale.z * 0.5f - 0.08f
                    )
                );
            }
        }

        private void CreateWindow(Vector3 position)
        {
            GameObject window = GameObject.CreatePrimitive(PrimitiveType.Cube);

            window.name = "Window";

            window.transform.position = position;
            window.transform.localScale = new Vector3(
                1.2f,
                0.8f,
                0.08f
            );

            Renderer renderer = window.GetComponent<Renderer>();
            renderer.material.color = new Color(0.12f, 0.18f, 0.25f);
        }
    }
}