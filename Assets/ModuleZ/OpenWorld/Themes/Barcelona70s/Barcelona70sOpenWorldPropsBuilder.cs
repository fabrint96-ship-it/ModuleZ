using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Builders;
using ModuleZ.OpenWorld.Runtime;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Barcelona70s
{
    public class Barcelona70sOpenWorldPropsBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateCityDecorationPreset();
            CreatePalmTrees();
            CreateFountain();
            CreateZonePortals();

            Debug.Log("[Module Z] Props OpenWorld Barcelona años 70 creados.");
        }

        private void CreatePalmTrees()
        {
            CreatePalmTree(new Vector3(-14f, 1.6f, 0f));
            CreatePalmTree(new Vector3(14f, 1.6f, 0f));
        }

        private void CreatePalmTree(Vector3 position)
        {
            CreateCube("Barcelona_OW_Palmera_Tronco", position, new Vector3(0.45f, 3.2f, 0.45f), new Color(0.45f, 0.25f, 0.10f));

            Vector3 top = position + new Vector3(0f, 1.8f, 0f);

            CreateCube("Barcelona_OW_Palmera_Hoja_A", top + new Vector3(0.8f, 0f, 0f), new Vector3(1.8f, 0.25f, 0.45f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Barcelona_OW_Palmera_Hoja_B", top + new Vector3(-0.8f, 0f, 0f), new Vector3(1.8f, 0.25f, 0.45f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Barcelona_OW_Palmera_Hoja_C", top + new Vector3(0f, 0f, 0.8f), new Vector3(0.45f, 0.25f, 1.8f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Barcelona_OW_Palmera_Hoja_D", top + new Vector3(0f, 0f, -0.8f), new Vector3(0.45f, 0.25f, 1.8f), new Color(0.10f, 0.45f, 0.18f));
        }

        private void CreateFountain()
        {
            CreateCube("Barcelona_OW_Fuente_Base", new Vector3(0f, 0.25f, 0f), new Vector3(4f, 0.5f, 4f), new Color(0.45f, 0.50f, 0.52f));
            CreateCube("Barcelona_OW_Fuente_Agua", new Vector3(0f, 0.65f, 0f), new Vector3(3f, 0.25f, 3f), new Color(0.20f, 0.55f, 0.85f));
            CreateCube("Barcelona_OW_Fuente_Centro", new Vector3(0f, 1.1f, 0f), new Vector3(0.7f, 0.9f, 0.7f), new Color(0.50f, 0.52f, 0.50f));
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
                "Portal_Valencia",
                new Vector3(18f, 0.6f, -18f),
                OpenWorldThemeId.Valencia70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.95f, 0.55f, 0.15f)
            );

            if (ModuleZGameState.AndaluciaUnlocked)
            {
                CreateZonePortal(
                    "Portal_Andalucia",
                    new Vector3(-18f, 0.6f, -18f),
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