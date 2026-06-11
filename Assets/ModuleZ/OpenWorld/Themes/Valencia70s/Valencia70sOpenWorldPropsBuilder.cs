using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Builders;
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
            CreatePalmTree(new Vector3(-16f, 1.6f, 0f));
            CreatePalmTree(new Vector3(16f, 1.6f, 0f));
        }

        private void CreatePalmTree(Vector3 position)
        {
            CreateCube("Valencia_OW_Palmera_Tronco", position, new Vector3(0.45f, 3.2f, 0.45f), new Color(0.45f, 0.25f, 0.10f));

            Vector3 top = position + new Vector3(0f, 1.8f, 0f);

            CreateCube("Valencia_OW_Palmera_Hoja_A", top + new Vector3(0.8f, 0f, 0f), new Vector3(1.8f, 0.25f, 0.45f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Valencia_OW_Palmera_Hoja_B", top + new Vector3(-0.8f, 0f, 0f), new Vector3(1.8f, 0.25f, 0.45f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Valencia_OW_Palmera_Hoja_C", top + new Vector3(0f, 0f, 0.8f), new Vector3(0.45f, 0.25f, 1.8f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Valencia_OW_Palmera_Hoja_D", top + new Vector3(0f, 0f, -0.8f), new Vector3(0.45f, 0.25f, 1.8f), new Color(0.10f, 0.45f, 0.18f));
        }

        private void CreateOrangeTrees()
        {
            CreateOrangeTree(new Vector3(-10f, 1.2f, 14f));
            CreateOrangeTree(new Vector3(10f, 1.2f, 14f));
            CreateOrangeTree(new Vector3(-10f, 1.2f, -14f));
            CreateOrangeTree(new Vector3(10f, 1.2f, -14f));
        }

        private void CreateOrangeTree(Vector3 position)
        {
            CreateCube("Valencia_OW_Naranjo_Tronco", position, new Vector3(0.35f, 2.4f, 0.35f), new Color(0.42f, 0.22f, 0.08f));
            CreateCube("Valencia_OW_Naranjo_Copa", position + new Vector3(0f, 1.35f, 0f), new Vector3(1.8f, 1.2f, 1.8f), new Color(0.10f, 0.42f, 0.16f));
            CreateCube("Valencia_OW_Naranja_A", position + new Vector3(0.45f, 1.35f, -0.35f), new Vector3(0.25f, 0.25f, 0.25f), new Color(0.95f, 0.45f, 0.05f));
            CreateCube("Valencia_OW_Naranja_B", position + new Vector3(-0.35f, 1.55f, 0.35f), new Vector3(0.25f, 0.25f, 0.25f), new Color(0.95f, 0.45f, 0.05f));
        }

        private void CreateZonePortals()
        {
            CreateZonePortal(
                "Portal_Madrid",
                new Vector3(0f, 0.6f, -24f),
                OpenWorldThemeId.Madrid70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.95f, 0.75f, 0.20f)
            );

            CreateZonePortal(
                "Portal_Barcelona",
                new Vector3(-18f, 0.6f, -18f),
                OpenWorldThemeId.Barcelona70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.20f, 0.55f, 0.85f)
            );

            if (ModuleZGameState.AndaluciaUnlocked)
            {
                CreateZonePortal(
                    "Portal_Andalucia",
                    new Vector3(18f, 0.6f, -18f),
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
            GameObject portal = CreateCube(name, position, new Vector3(2f, 1.2f, 2f), color);

            BoxCollider collider = portal.GetComponent<BoxCollider>();
            collider.isTrigger = true;

            OpenWorldZonePortal zonePortal = portal.AddComponent<OpenWorldZonePortal>();
            zonePortal.targetTheme = targetTheme;
            zonePortal.spawnPosition = spawnPosition;
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