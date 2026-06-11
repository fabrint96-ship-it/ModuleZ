using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Barcelona70s
{
    public class Barcelona70sOpenWorldBuildingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateNorthBuildings();
            CreateSouthBuildings();
            CreateEastBuildings();
            CreateWestBuildings();

            Debug.Log("[Module Z] Edificios OpenWorld Barcelona años 70 creados.");
        }

        private void CreateNorthBuildings()
        {
            CreateBuilding(
                "Barcelona_Building_North_A",
                new Vector3(-18f, 3f, 26f),
                new Vector3(10f, 6f, 8f),
                new Color(0.85f, 0.80f, 0.72f)
            );

            CreateBuilding(
                "Barcelona_Building_North_B",
                new Vector3(18f, 4f, 26f),
                new Vector3(12f, 8f, 8f),
                new Color(0.78f, 0.72f, 0.62f)
            );
        }

        private void CreateSouthBuildings()
        {
            CreateBuilding(
                "Barcelona_Building_South_A",
                new Vector3(-18f, 3f, -26f),
                new Vector3(10f, 6f, 8f),
                new Color(0.82f, 0.75f, 0.66f)
            );

            CreateBuilding(
                "Barcelona_Building_South_B",
                new Vector3(18f, 4f, -26f),
                new Vector3(12f, 8f, 8f),
                new Color(0.88f, 0.82f, 0.72f)
            );
        }

        private void CreateEastBuildings()
        {
            CreateBuilding(
                "Barcelona_Building_East",
                new Vector3(30f, 4f, 0f),
                new Vector3(8f, 8f, 20f),
                new Color(0.75f, 0.68f, 0.58f)
            );
        }

        private void CreateWestBuildings()
        {
            CreateBuilding(
                "Barcelona_Building_West",
                new Vector3(-30f, 4f, 0f),
                new Vector3(8f, 8f, 20f),
                new Color(0.80f, 0.72f, 0.62f)
            );
        }

        private void CreateBuilding(
            string name,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);

            building.name = name;
            building.transform.position = position;
            building.transform.localScale = scale;

            Renderer renderer = building.GetComponent<Renderer>();
            renderer.material.color = color;
        }
    }
}