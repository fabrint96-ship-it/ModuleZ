using UnityEngine;

namespace ModuleZ.UI.MainMenu
{
    public class ModuleZMainMenuEnvironmentBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateCamera();
            CreateLight();
            CreateGround();
            CreateBackgroundCity();
            CreateZPieceMonument();
            CreateFloatingCubes();

            Debug.Log("[ModuleZ] Main Menu Environment creado.");
        }

        private void CreateCamera()
        {
            Camera[] cameras = FindObjectsOfType<Camera>();

            for (int i = 0; i < cameras.Length; i++)
            {
                Destroy(cameras[i].gameObject);
            }

            GameObject cameraObj = new GameObject("MainMenu_Camera");
            cameraObj.tag = "MainCamera";

            Camera camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.fieldOfView = 48f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100f;

            cameraObj.transform.position = new Vector3(0f, 5.2f, -10f);
            cameraObj.transform.LookAt(new Vector3(0f, 1.8f, 0f));

            cameraObj.AddComponent<ModuleZMainMenuCameraAnimator>();
        }

        private void CreateLight()
        {
            RenderSettings.ambientLight =
                new Color(0.28f, 0.32f, 0.40f);

            RenderSettings.fog = true;
            RenderSettings.fogColor =
                new Color(0.06f, 0.08f, 0.12f);

            RenderSettings.fogDensity = 0.018f;

            GameObject lightObj =
                new GameObject("MainMenu_DirectionalLight");

            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.35f;
            light.color = new Color(1f, 0.88f, 0.65f);

            lightObj.transform.rotation =
                Quaternion.Euler(45f, -35f, 0f);
        }

        private void CreateGround()
        {
            CreateCube(
                "MainMenu_Ground",
                new Vector3(0f, -0.08f, 0f),
                new Vector3(24f, 0.16f, 18f),
                new Color(0.10f, 0.12f, 0.15f)
            );
        }

        private void CreateBackgroundCity()
        {
            CreateBuilding(
                "Menu_Building_Left",
                new Vector3(-8f, 1.8f, 4.5f),
                new Vector3(3f, 3.6f, 2f),
                new Color(0.35f, 0.32f, 0.28f)
            );

            CreateBuilding(
                "Menu_Building_Right",
                new Vector3(8f, 2.2f, 4.8f),
                new Vector3(3.5f, 4.4f, 2f),
                new Color(0.30f, 0.28f, 0.25f)
            );

            CreateBuilding(
                "Menu_Building_Back_A",
                new Vector3(-3f, 2.6f, 5.5f),
                new Vector3(3f, 5.2f, 2f),
                new Color(0.28f, 0.28f, 0.30f)
            );

            CreateBuilding(
                "Menu_Building_Back_B",
                new Vector3(3f, 2.1f, 5.7f),
                new Vector3(3f, 4.2f, 2f),
                new Color(0.32f, 0.30f, 0.27f)
            );
        }

        private void CreateBuilding(
            string name,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            CreateCube(name, position, scale, color);

            CreateCube(
                name + "_Window_A",
                position + new Vector3(-0.7f, 0.5f, -1.05f),
                new Vector3(0.4f, 0.45f, 0.05f),
                new Color(0.25f, 0.45f, 0.70f)
            );

            CreateCube(
                name + "_Window_B",
                position + new Vector3(0.7f, 0.5f, -1.05f),
                new Vector3(0.4f, 0.45f, 0.05f),
                new Color(0.25f, 0.45f, 0.70f)
            );
        }

        private void CreateZPieceMonument()
        {
            GameObject monumentRoot = new GameObject("ZPieceMonument");

            monumentRoot.transform.position = new Vector3(0f, 0.4f, 0f);
            monumentRoot.AddComponent<ModuleZMainMenuMonumentAnimator>();

            Color zColor = new Color(0.15f, 0.55f, 1f);

            float size = 0.75f;

            CreateCube(
                "Menu_ZPiece_Block_1",
                monumentRoot.transform,
                new Vector3(-size, 0f, 0f),
                Vector3.one * size,
                zColor
            );

            CreateCube(
                "Menu_ZPiece_Block_2",
                monumentRoot.transform,
                Vector3.zero,
                Vector3.one * size,
                zColor
            );

            CreateCube(
                "Menu_ZPiece_Block_3",
                monumentRoot.transform,
                new Vector3(size, 0f, 0f),
                Vector3.one * size,
                zColor
            );

            CreateCube(
                "Menu_ZPiece_Block_4",
                monumentRoot.transform,
                new Vector3(size, size, 0f),
                Vector3.one * size,
                zColor
            );

            CreateCube(
                "Menu_ZPiece_Block_5",
                monumentRoot.transform,
                new Vector3(size * 2f, size, 0f),
                Vector3.one * size,
                zColor
            );
        }

        private GameObject CreateCube(
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Color color)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = localPosition;
            cube.transform.localScale = localScale;

            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material.color = color;

            return cube;
        }

        private void CreateFloatingCubes()
        {
            CreateAnimatedFloatingCube(
                "Menu_FloatingCube_A",
                new Vector3(-3.2f, 2.8f, -1.5f),
                0.35f,
                new Color(0.35f, 0.75f, 1f)
            );

            CreateAnimatedFloatingCube(
                "Menu_FloatingCube_B",
                new Vector3(3.4f, 3.2f, -1f),
                0.45f,
                new Color(0.95f, 0.55f, 0.15f)
            );

            CreateAnimatedFloatingCube(
                "Menu_FloatingCube_C",
                new Vector3(0f, 3.8f, 1.2f),
                0.40f,
                new Color(0.80f, 0.75f, 0.25f)
            );
        }

        private void CreateAnimatedFloatingCube(
            string name,
            Vector3 position,
            float size,
            Color color)
        {
            GameObject cube =
                CreateCube(
                    name,
                    position,
                    Vector3.one * size,
                    color
                );

            cube.AddComponent<ModuleZMainMenuFloatingCubeAnimator>();
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