using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Builders;
using ModuleZ.OpenWorld.Portals;
using ModuleZ.OpenWorld.Runtime;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Valencia70s
{
    public class Valencia70sOpenWorldPropsBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateCityDecorationPreset();
            CreatePalmTrees();
            CreateOrangeTrees();
            CreateZonePortals();

            Debug.Log("[Module Z] Props OpenWorld Valencia años 70 creados.");
        }

        private void CreatePalmTrees()
        {
            CreatePalmTree(new Vector3(-9f, 0f, 7f));
            CreatePalmTree(new Vector3(9f, 0f, 7f));
        }

        private void CreatePalmTree(Vector3 basePosition)
        {
            CreateCube(
                "Valencia_OW_Palmera_Tronco",
                basePosition + new Vector3(0f, 1.6f, 0f),
                new Vector3(0.45f, 3.2f, 0.45f),
                new Color(0.45f, 0.25f, 0.10f)
            );

            Vector3 top = basePosition + new Vector3(0f, 3.35f, 0f);

            CreateCube("Valencia_OW_Palmera_Hoja_A", top + new Vector3(0.8f, 0f, 0f), new Vector3(1.8f, 0.25f, 0.45f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Valencia_OW_Palmera_Hoja_B", top + new Vector3(-0.8f, 0f, 0f), new Vector3(1.8f, 0.25f, 0.45f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Valencia_OW_Palmera_Hoja_C", top + new Vector3(0f, 0f, 0.8f), new Vector3(0.45f, 0.25f, 1.8f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Valencia_OW_Palmera_Hoja_D", top + new Vector3(0f, 0f, -0.8f), new Vector3(0.45f, 0.25f, 1.8f), new Color(0.10f, 0.45f, 0.18f));
        }

        private void CreateOrangeTrees()
        {
            CreateOrangeTree(new Vector3(-6.5f, 0f, 6.5f));
            CreateOrangeTree(new Vector3(6.5f, 0f, 6.5f));
            CreateOrangeTree(new Vector3(-6.5f, 0f, -6.5f));
            CreateOrangeTree(new Vector3(6.5f, 0f, -6.5f));
        }

        private void CreateOrangeTree(Vector3 basePosition)
        {
            CreateCube(
                "Valencia_OW_Naranjo_Tronco",
                basePosition + new Vector3(0f, 1.2f, 0f),
                new Vector3(0.35f, 2.4f, 0.35f),
                new Color(0.42f, 0.22f, 0.08f)
            );

            Vector3 crown = basePosition + new Vector3(0f, 2.7f, 0f);

            CreateCube(
                "Valencia_OW_Naranjo_Copa",
                crown,
                new Vector3(1.8f, 1.2f, 1.8f),
                new Color(0.10f, 0.42f, 0.16f)
            );

            Color orange = new Color(0.95f, 0.45f, 0.05f);

            CreateCube("Naranja_01", crown + new Vector3(0.95f, 0.10f, -0.55f), new Vector3(0.28f, 0.28f, 0.28f), orange);
            CreateCube("Naranja_02", crown + new Vector3(-0.95f, 0.25f, 0.45f), new Vector3(0.28f, 0.28f, 0.28f), orange);
            CreateCube("Naranja_03", crown + new Vector3(0.35f, -0.20f, 0.95f), new Vector3(0.28f, 0.28f, 0.28f), orange);

            CreateCube("Naranja_04", crown + new Vector3(-0.35f, 0.15f, -0.95f), new Vector3(0.28f, 0.28f, 0.28f), orange);
            CreateCube("Naranja_05", crown + new Vector3(0.75f, -0.15f, 0.55f), new Vector3(0.28f, 0.28f, 0.28f), orange);
            CreateCube("Naranja_06", crown + new Vector3(-0.75f, -0.10f, -0.55f), new Vector3(0.28f, 0.28f, 0.28f), orange);

            CreateCube("Naranja_07", crown + new Vector3(0.00f, 0.35f, 1.00f), new Vector3(0.28f, 0.28f, 0.28f), orange);
            CreateCube("Naranja_08", crown + new Vector3(1.00f, 0.20f, 0.00f), new Vector3(0.28f, 0.28f, 0.28f), orange);
            CreateCube("Naranja_09", crown + new Vector3(-1.00f, 0.20f, 0.00f), new Vector3(0.28f, 0.28f, 0.28f), orange);

            CreateCube("Naranja_10", crown + new Vector3(0.00f, -0.25f, -1.00f), new Vector3(0.28f, 0.28f, 0.28f), orange);
            CreateCube("Naranja_11", crown + new Vector3(0.55f, 0.40f, 0.55f), new Vector3(0.28f, 0.28f, 0.28f), orange);
            CreateCube("Naranja_12", crown + new Vector3(-0.55f, 0.40f, -0.55f), new Vector3(0.28f, 0.28f, 0.28f), orange);
        }

        private void CreateZonePortals()
        {
            CreateZonePortal(
                "Portal_Madrid",
                new Vector3(-18f, 0f, -18f),
                OpenWorldThemeId.Madrid70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.95f, 0.75f, 0.20f)
            );

            CreateZonePortal(
                "Portal_Barcelona",
                new Vector3(18f, 0f, -18f),
                OpenWorldThemeId.Barcelona70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.20f, 0.55f, 0.85f)
            );

            if (ModuleZGameState.AndaluciaUnlocked)
            {
                CreateZonePortal(
                    "Portal_Andalucia",
                    new Vector3(18f, 0f, -18f),
                    OpenWorldThemeId.Andalucia70s,
                    new Vector3(0f, 0.1f, -4f),
                    new Color(0.18f, 0.45f, 0.75f)
                );
            }
        }

        private void CreateZonePortal(
            string name,
            Vector3 position,
            OpenWorldThemeId targetTheme,
            Vector3 spawnPosition,
            Color color)
        {
            GameObject portalRoot = new GameObject(name);

            portalRoot.transform.position = new Vector3(position.x, 0f, position.z);

            OpenWorldPortalVisualBuilder.BuildPortalVisual(
                portalRoot.transform,
                GetPortalDisplayName(targetTheme),
                color
            );

            BoxCollider collider =
                portalRoot.AddComponent<BoxCollider>();

            collider.isTrigger = true;
            collider.center = new Vector3(0f, 1f, 0f);
            collider.size = new Vector3(2.5f, 2.5f, 2.5f);

            OpenWorldZonePortal zonePortal =
                portalRoot.AddComponent<OpenWorldZonePortal>();

            zonePortal.targetTheme = targetTheme;
            zonePortal.spawnPosition = spawnPosition;
        }

        private string GetPortalDisplayName(OpenWorldThemeId theme)
        {
            switch (theme)
            {
                case OpenWorldThemeId.Madrid70s:
                    return "Madrid";

                case OpenWorldThemeId.Barcelona70s:
                    return "Barcelona";

                case OpenWorldThemeId.Valencia70s:
                    return "Valencia";

                case OpenWorldThemeId.Andalucia70s:
                    return "Andalucía";

                default:
                    return "Destino";
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

        private void CreateCityDecorationPreset()
        {
            ModuleZCityDecorationBuilder.BuildPreset(
                ModuleZCityDecorationPresets.MadridPlaza(),
                ModuleZCityDecorationTheme.Madrid70s()
            );
        }
    }
}