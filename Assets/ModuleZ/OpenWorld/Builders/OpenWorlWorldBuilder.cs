using UnityEngine;
using ModuleZ.Game.Player;
using ModuleZ.Game.Camera;

namespace ModuleZ.OpenWorld.Builders
{
    public class Madrid70sWorldBuilder : MonoBehaviour
    {
        private void Start()
        {
            BuildWorld();
        }

        private void BuildWorld()
        {
            CreateLighting();
            CreateGround();
            CreateMainPlaza();
            CreateBuildings();
            CreateStreetProps();
            CreatePlayer();

            Debug.Log("[Module Z] OpenWorld Madrid años 70 generado.");
        }

        private void CreateLighting()
        {
            GameObject sun = new GameObject("Sun_Madrid_70s");

            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(1f, 0.92f, 0.78f);

            sun.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

            RenderSettings.ambientLight = new Color(0.45f, 0.42f, 0.36f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.56f, 0.50f, 0.42f);
            RenderSettings.fogDensity = 0.008f;
        }

        private void CreateGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground_Madrid_70s";
            ground.transform.position = new Vector3(0f, -0.1f, 0f);
            ground.transform.localScale = new Vector3(60f, 0.2f, 60f);

            Renderer renderer = ground.GetComponent<Renderer>();
            renderer.material.color = new Color(0.42f, 0.36f, 0.30f);
        }

        private void CreateMainPlaza()
        {
            GameObject plaza = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plaza.name = "Plaza_Central_Adoquines";
            plaza.transform.position = new Vector3(0f, 0.02f, 0f);
            plaza.transform.localScale = new Vector3(24f, 0.08f, 24f);

            Renderer renderer = plaza.GetComponent<Renderer>();
            renderer.material.color = new Color(0.50f, 0.47f, 0.42f);

            CreatePlazaTiles();
        }

        private void CreatePlazaTiles()
        {
            for (int x = -10; x <= 10; x += 2)
            {
                for (int z = -10; z <= 10; z += 2)
                {
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    tile.name = "Adoquin_Plaza";
                    tile.transform.position = new Vector3(x, 0.11f, z);
                    tile.transform.localScale = new Vector3(1.8f, 0.04f, 1.8f);

                    Renderer renderer = tile.GetComponent<Renderer>();
                    renderer.material.color = new Color(0.38f, 0.36f, 0.33f);
                }
            }
        }

        private void CreateBuildings()
        {
            CreateBuilding("Edificio_70s_A", new Vector3(-18f, 3f, 0f), new Vector3(5f, 6f, 12f), new Color(0.63f, 0.54f, 0.42f));
            CreateBuilding("Edificio_70s_B", new Vector3(18f, 4f, 0f), new Vector3(5f, 8f, 14f), new Color(0.58f, 0.49f, 0.38f));
            CreateBuilding("Bar_Madrid_70s", new Vector3(0f, 2.5f, 18f), new Vector3(12f, 5f, 5f), new Color(0.52f, 0.38f, 0.25f));
            CreateBuilding("Tienda_70s", new Vector3(0f, 2.5f, -18f), new Vector3(12f, 5f, 5f), new Color(0.66f, 0.58f, 0.45f));
        }

        private void CreateBuilding(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
            building.name = name;
            building.transform.position = position;
            building.transform.localScale = scale;

            Renderer renderer = building.GetComponent<Renderer>();
            renderer.material.color = color;

            CreateWindows(building, position, scale);
            CreateDoor(position, scale);
        }

        private void CreateWindows(GameObject parent, Vector3 buildingPosition, Vector3 buildingScale)
        {
            int floors = Mathf.Max(1, Mathf.FloorToInt(buildingScale.y / 2f));

            for (int i = 0; i < floors; i++)
            {
                float y = 1.3f + i * 1.5f;

                CreateWindow(new Vector3(buildingPosition.x, y, buildingPosition.z - buildingScale.z / 2f - 0.03f));
                CreateWindow(new Vector3(buildingPosition.x, y, buildingPosition.z + buildingScale.z / 2f + 0.03f));
            }
        }

        private void CreateWindow(Vector3 position)
        {
            GameObject window = GameObject.CreatePrimitive(PrimitiveType.Cube);
            window.name = "Ventana_70s";
            window.transform.position = position;
            window.transform.localScale = new Vector3(1.2f, 0.8f, 0.08f);

            Renderer renderer = window.GetComponent<Renderer>();
            renderer.material.color = new Color(0.12f, 0.18f, 0.22f);
        }

        private void CreateDoor(Vector3 buildingPosition, Vector3 buildingScale)
        {
            GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
            door.name = "Puerta_70s";
            door.transform.position = new Vector3(
                buildingPosition.x,
                0.75f,
                buildingPosition.z - buildingScale.z / 2f - 0.04f
            );
            door.transform.localScale = new Vector3(1.2f, 1.5f, 0.08f);

            Renderer renderer = door.GetComponent<Renderer>();
            renderer.material.color = new Color(0.18f, 0.10f, 0.05f);
        }

        private void CreateStreetProps()
        {
            CreateStreetLamp(new Vector3(-8f, 1.5f, -8f));
            CreateStreetLamp(new Vector3(8f, 1.5f, -8f));
            CreateStreetLamp(new Vector3(-8f, 1.5f, 8f));
            CreateStreetLamp(new Vector3(8f, 1.5f, 8f));

            CreateBench(new Vector3(-5f, 0.4f, 6f));
            CreateBench(new Vector3(5f, 0.4f, 6f));

            CreateRetroCar(new Vector3(-10f, 0.45f, -14f));
            CreateRetroCar(new Vector3(10f, 0.45f, 14f));
        }

        private void CreateStreetLamp(Vector3 position)
        {
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pole.name = "Farola_70s";
            pole.transform.position = position;
            pole.transform.localScale = new Vector3(0.18f, 3f, 0.18f);

            Renderer poleRenderer = pole.GetComponent<Renderer>();
            poleRenderer.material.color = new Color(0.08f, 0.08f, 0.08f);

            GameObject lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lamp.name = "Luz_Farola_70s";
            lamp.transform.position = position + new Vector3(0f, 1.65f, 0f);
            lamp.transform.localScale = new Vector3(0.6f, 0.3f, 0.6f);

            Renderer lampRenderer = lamp.GetComponent<Renderer>();
            lampRenderer.material.color = new Color(1f, 0.82f, 0.45f);
        }

        private void CreateBench(Vector3 position)
        {
            GameObject bench = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bench.name = "Banco_70s";
            bench.transform.position = position;
            bench.transform.localScale = new Vector3(2.4f, 0.25f, 0.7f);

            Renderer renderer = bench.GetComponent<Renderer>();
            renderer.material.color = new Color(0.28f, 0.14f, 0.06f);
        }

        private void CreateRetroCar(Vector3 position)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Coche_Retro_70s";
            body.transform.position = position;
            body.transform.localScale = new Vector3(2.8f, 0.7f, 1.4f);

            Renderer renderer = body.GetComponent<Renderer>();
            renderer.material.color = new Color(0.55f, 0.12f, 0.08f);

            GameObject roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roof.name = "Techo_Coche_Retro_70s";
            roof.transform.position = position + new Vector3(0f, 0.55f, 0f);
            roof.transform.localScale = new Vector3(1.5f, 0.6f, 1.1f);

            Renderer roofRenderer = roof.GetComponent<Renderer>();
            roofRenderer.material.color = new Color(0.45f, 0.08f, 0.06f);
        }

        private void CreatePlayer()
        {
            GameObject builderObj = new GameObject("PlayerBuilder");
            ModuleZPlayerBuilder builder = builderObj.AddComponent<ModuleZPlayerBuilder>();

            GameObject player = builder.BuildPlayer(new Vector3(0f, 0.1f, -4f));

            Destroy(builderObj);

            CreateThirdPersonCamera(player.transform);

            Debug.Log("[Module Z] Player humano cúbico creado con cámara tercera persona.");
        }

        private void CreateThirdPersonCamera(Transform target)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";

            Camera camera = cameraObj.AddComponent<Camera>();
            camera.fieldOfView = 50f;
            camera.clearFlags = CameraClearFlags.Skybox;

            ModuleZThirdPersonCamera followCamera =
                cameraObj.AddComponent<ModuleZThirdPersonCamera>();

            followCamera.SetTarget(target);

            cameraObj.transform.position =
                target.position + target.rotation * new Vector3(0f, 3.2f, -4.5f);

            cameraObj.transform.LookAt(target.position + Vector3.up * 1.6f);
        }
    }
}