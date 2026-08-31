using UnityEngine;

namespace ModuleZ.OpenWorld.Builders
{
    public class OpenWorldRoadPropsBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateOuterRoads();
            CreateSidewalks();
            CreateCrosswalks();
            CreateTrafficLights();

            Debug.Log("[ModuleZ] Carreteras, aceras, pasos de peatones, semáforos y coches creados.");
        }

        private void CreateOuterRoads()
        {
            Color road = new Color(0.10f, 0.10f, 0.10f);

            CreateCube("Road_North", new Vector3(0f, 0.01f, 24f), new Vector3(42f, 0.04f, 3.5f), road);
            CreateCube("Road_South", new Vector3(0f, 0.01f, -24f), new Vector3(42f, 0.04f, 3.5f), road);
            CreateCube("Road_East", new Vector3(24f, 0.01f, 0f), new Vector3(3.5f, 0.04f, 42f), road);
            CreateCube("Road_West", new Vector3(-24f, 0.01f, 0f), new Vector3(3.5f, 0.04f, 42f), road);
        }

        private void CreateSidewalks()
        {
            Color sidewalk = new Color(0.48f, 0.46f, 0.42f);

            CreateCube("Sidewalk_North_Inner", new Vector3(0f, 0.04f, 21.8f), new Vector3(42f, 0.08f, 1.1f), sidewalk);
            CreateCube("Sidewalk_South_Inner", new Vector3(0f, 0.04f, -21.8f), new Vector3(42f, 0.08f, 1.1f), sidewalk);
            CreateCube("Sidewalk_East_Inner", new Vector3(21.8f, 0.04f, 0f), new Vector3(1.1f, 0.08f, 42f), sidewalk);
            CreateCube("Sidewalk_West_Inner", new Vector3(-21.8f, 0.04f, 0f), new Vector3(1.1f, 0.08f, 42f), sidewalk);
        }

        private void CreateCrosswalks()
        {
            CreateCrosswalkHorizontal(new Vector3(0f, 0.08f, 24f));
            CreateCrosswalkHorizontal(new Vector3(0f, 0.08f, -24f));

            CreateCrosswalkVertical(new Vector3(24f, 0.08f, 0f));
            CreateCrosswalkVertical(new Vector3(-24f, 0.08f, 0f));
        }

        private void CreateCrosswalkHorizontal(Vector3 center)
        {
            Color white = new Color(0.86f, 0.84f, 0.76f);

            for (int i = -3; i <= 3; i++)
            {
                CreateCube(
                    "Crosswalk_Stripe_H",
                    center + new Vector3(i * 0.55f, 0f, 0f),
                    new Vector3(0.32f, 0.025f, 3.5f),
                    white
                );
            }
        }

        private void CreateCrosswalkVertical(Vector3 center)
        {
            Color white = new Color(0.86f, 0.84f, 0.76f);

            for (int i = -3; i <= 3; i++)
            {
                CreateCube(
                    "Crosswalk_Stripe_V",
                    center + new Vector3(0f, 0f, i * 0.55f),
                    new Vector3(3.5f, 0.025f, 0.32f),
                    white
                );
            }
        }

        private void CreateTrafficLights()
        {
            CreateTrafficLight(new Vector3(-21.5f, 0f, 21.5f));
            CreateTrafficLight(new Vector3(21.5f, 0f, 21.5f));
            CreateTrafficLight(new Vector3(-21.5f, 0f, -21.5f));
            CreateTrafficLight(new Vector3(21.5f, 0f, -21.5f));
        }

        private void CreateTrafficLight(Vector3 basePosition)
        {
            Color pole = new Color(0.08f, 0.08f, 0.07f);
            Color box = new Color(0.12f, 0.10f, 0.08f);

            CreateCube("TrafficLight_Pole", basePosition + new Vector3(0f, 1.1f, 0f), new Vector3(0.15f, 2.2f, 0.15f), pole);
            CreateCube("TrafficLight_Box", basePosition + new Vector3(0f, 2.25f, 0f), new Vector3(0.45f, 0.9f, 0.25f), box);

            CreateCube("TrafficLight_Red", basePosition + new Vector3(0f, 2.52f, -0.14f), new Vector3(0.22f, 0.18f, 0.04f), new Color(0.8f, 0.05f, 0.04f));
            CreateCube("TrafficLight_Amber", basePosition + new Vector3(0f, 2.25f, -0.14f), new Vector3(0.22f, 0.18f, 0.04f), new Color(0.95f, 0.55f, 0.05f));
            CreateCube("TrafficLight_Green", basePosition + new Vector3(0f, 1.98f, -0.14f), new Vector3(0.22f, 0.18f, 0.04f), new Color(0.05f, 0.55f, 0.10f));
        }

        private GameObject CreateCube(
            string name,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            obj.name = name;
            obj.transform.SetParent(transform, false);
            obj.transform.position = position;
            obj.transform.localScale = scale;

            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material.color = color;

            return obj;
        }
    }
}