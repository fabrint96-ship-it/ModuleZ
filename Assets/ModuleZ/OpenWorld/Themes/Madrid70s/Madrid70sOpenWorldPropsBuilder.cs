using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Builders;
using ModuleZ.OpenWorld.Runtime;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Madrid70s
{
    public class Madrid70sPropsBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateCityDecorationPreset();
            CreateFountain();
            CreateTrashCans();
            CreatePhoneBooth();
            CreateRetroCars();
            CreatePosters();
            CreateZonePortals();

            Debug.Log("[Module Z] Props Madrid años 70 creados.");
        }

        private void CreateTrashCans()
        {
            CreateCube("Papelera_70s", new Vector3(-8f, 0.45f, 6f), new Vector3(0.5f, 0.9f, 0.5f), new Color(0.10f, 0.25f, 0.12f));
            CreateCube("Papelera_70s", new Vector3(8f, 0.45f, 6f), new Vector3(0.5f, 0.9f, 0.5f), new Color(0.10f, 0.25f, 0.12f));
        }

        private void CreatePhoneBooth()
        {
            Vector3 pos = new Vector3(-13f, 1.2f, -6f);

            CreateCube("Cabina_Telefonica_70s_Base", pos, new Vector3(1.2f, 2.4f, 1.2f), new Color(0.75f, 0.08f, 0.05f));
            CreateCube("Cabina_Telefonica_70s_Cristal", pos + new Vector3(0f, 0.2f, -0.62f), new Vector3(0.9f, 1.5f, 0.05f), new Color(0.35f, 0.60f, 0.70f));
            CreateCube("Telefono_Interior", pos + new Vector3(0f, 0.1f, -0.55f), new Vector3(0.4f, 0.5f, 0.08f), new Color(0.05f, 0.05f, 0.05f));
        }

        private void CreateRetroCars()
        {
            CreateRetroCar(new Vector3(-14f, 0.45f, -16f), new Color(0.55f, 0.12f, 0.08f));
            CreateRetroCar(new Vector3(14f, 0.45f, 16f), new Color(0.10f, 0.22f, 0.48f));
        }

        private void CreateRetroCar(Vector3 position, Color color)
        {
            CreateCube("Coche_Retro_70s_Cuerpo", position, new Vector3(3f, 0.7f, 1.5f), color);
            CreateCube("Coche_Retro_70s_Techo", position + new Vector3(0f, 0.55f, 0f), new Vector3(1.6f, 0.6f, 1.1f), color * 0.8f);
            CreateCube("Rueda_Del_Izq", position + new Vector3(-0.9f, -0.35f, -0.75f), new Vector3(0.45f, 0.45f, 0.2f), Color.black);
            CreateCube("Rueda_Del_Der", position + new Vector3(0.9f, -0.35f, -0.75f), new Vector3(0.45f, 0.45f, 0.2f), Color.black);
            CreateCube("Rueda_Tras_Izq", position + new Vector3(-0.9f, -0.35f, 0.75f), new Vector3(0.45f, 0.45f, 0.2f), Color.black);
            CreateCube("Rueda_Tras_Der", position + new Vector3(0.9f, -0.35f, 0.75f), new Vector3(0.45f, 0.45f, 0.2f), Color.black);
        }

        private void CreatePosters()
        {
            CreateCube("Cartel_Bar_70s", new Vector3(0f, 3.2f, 18.95f), new Vector3(4f, 1f, 0.08f), new Color(0.85f, 0.70f, 0.35f));
            CreateCube("Cartel_Tienda_70s", new Vector3(0f, 3.2f, -18.95f), new Vector3(4f, 1f, 0.08f), new Color(0.25f, 0.45f, 0.75f));
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

        private void CreateZonePortals()
        {
            CreateZonePortal(
                "Portal_Barcelona",
                new Vector3(-18f, 0.6f, -18f),
                OpenWorldThemeId.Barcelona70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.20f, 0.55f, 0.85f)
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
                    new Vector3(0f, 0.6f, -24f),
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
            GameObject portal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            portal.name = name;
            portal.transform.position = position;
            portal.transform.localScale = new Vector3(2f, 1.2f, 2f);

            Renderer renderer = portal.GetComponent<Renderer>();
            renderer.material.color = color;

            BoxCollider collider = portal.GetComponent<BoxCollider>();
            collider.isTrigger = true;

            OpenWorldZonePortal zonePortal = portal.AddComponent<OpenWorldZonePortal>();
            zonePortal.targetTheme = targetTheme;
            zonePortal.spawnPosition = spawnPosition;
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