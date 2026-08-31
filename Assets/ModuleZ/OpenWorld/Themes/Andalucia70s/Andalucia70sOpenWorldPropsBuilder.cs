using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Builders;
using ModuleZ.OpenWorld.Runtime;
using ModuleZ.OpenWorld.Portals;
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
            CreatePlanter(new Vector3(-7f, 0f, -5f));
            CreatePlanter(new Vector3(7f, 0f, 5f));
            CreatePlanter(new Vector3(-7f, 0f, 5f));
            CreatePlanter(new Vector3(7f, 0f, -5f));
        }

        private void CreatePlanter(Vector3 basePosition)
        {
            CreateCube(
                "Andalucia_OW_Maceta_Base",
                basePosition + new Vector3(0f, 0.3f, 0f),
                new Vector3(1.4f, 0.6f, 1.4f),
                new Color(0.65f, 0.28f, 0.12f)
            );

            CreateCube(
                "Andalucia_OW_Planta",
                basePosition + new Vector3(0f, 0.85f, 0f),
                new Vector3(1.0f, 0.5f, 1.0f),
                new Color(0.10f, 0.45f, 0.18f)
            );

            CreateCube(
                "Andalucia_OW_Flor",
                basePosition + new Vector3(0f, 1.15f, 0f),
                new Vector3(0.35f, 0.25f, 0.35f),
                new Color(0.95f, 0.15f, 0.15f)
            );
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

            CreateZonePortal(
                "Portal_Valencia",
                new Vector3(18f, 0f, 18f),
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