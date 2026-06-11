using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Builders;
using ModuleZ.OpenWorld.Runtime;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Andalucia70s
{
    public class Andalucia70sOpenWorldPropsBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateCityDecorationPreset();
            CreatePlanters();
            CreateFountain();
            CreateZonePortals();

            Debug.Log("[Module Z] Props OpenWorld Andalucía años 70 creados.");
        }

        private void CreatePlanters()
        {
            CreatePlanter(new Vector3(-13f, 0.45f, -5f));
            CreatePlanter(new Vector3(13f, 0.45f, 5f));
            CreatePlanter(new Vector3(-13f, 0.45f, 5f));
            CreatePlanter(new Vector3(13f, 0.45f, -5f));
        }

        private void CreatePlanter(Vector3 position)
        {
            CreateCube("Andalucia_OW_Maceta_Base", position, new Vector3(1.4f, 0.6f, 1.4f), new Color(0.65f, 0.28f, 0.12f));
            CreateCube("Andalucia_OW_Planta", position + new Vector3(0f, 0.55f, 0f), new Vector3(1.0f, 0.5f, 1.0f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Andalucia_OW_Flor", position + new Vector3(0f, 0.9f, 0f), new Vector3(0.35f, 0.25f, 0.35f), new Color(0.95f, 0.15f, 0.15f));
        }

        private void CreateZonePortals()
        {
            CreateZonePortal(
                "Portal_Madrid",
                new Vector3(0f, 0.6f, -30f),
                OpenWorldThemeId.Madrid70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.95f, 0.75f, 0.20f)
            );

            CreateZonePortal(
                "Portal_Barcelona",
                new Vector3(-18f, 0.6f, -24f),
                OpenWorldThemeId.Barcelona70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.20f, 0.55f, 0.85f)
            );

            CreateZonePortal(
                "Portal_Valencia",
                new Vector3(18f, 0.6f, -24f),
                OpenWorldThemeId.Valencia70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.95f, 0.55f, 0.15f)
            );
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

        private void CreateFountain()
        {
            CreateCube(
                "Fuente_Madrid_Base",
                new Vector3(0f, 0.25f, 0f),
                new Vector3(4f, 0.5f, 4f),
                new Color(0.45f, 0.45f, 0.42f)
            );

            CreateCube(
                "Fuente_Madrid_Agua",
                new Vector3(0f, 0.65f, 0f),
                new Vector3(3f, 0.25f, 3f),
                new Color(0.20f, 0.45f, 0.75f)
            );

            CreateCube(
                "Fuente_Madrid_Centro",
                new Vector3(0f, 1.1f, 0f),
                new Vector3(0.7f, 0.9f, 0.7f),
                new Color(0.50f, 0.50f, 0.48f)
            );
        }
    }
}