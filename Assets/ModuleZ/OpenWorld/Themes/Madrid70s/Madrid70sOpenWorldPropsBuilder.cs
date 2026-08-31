using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Builders;
using ModuleZ.OpenWorld.Runtime;
using ModuleZ.OpenWorld.Portals;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Madrid70s
{
    public class Madrid70sOpenWorldPropsBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateCityDecorationPreset();

            CreateFountain();
            CreateMadridUnifiedProps();

            CreateTrashCans();
            CreatePhoneBooth();
            CreateRetroCars();
            CreateZonePortals();

            Debug.Log("[Module Z] Props Madrid años 70 creados.");
        }

        private void CreateMadridUnifiedProps()
        {
            Color wood = new Color(0.42f, 0.28f, 0.15f);
            Color metal = new Color(0.22f, 0.22f, 0.22f);
            Color light = new Color(1f, 0.92f, 0.55f);
            Color pot = new Color(0.55f, 0.30f, 0.16f);
            Color plant = new Color(0.14f, 0.50f, 0.18f);

            GameObject benchN = OpenWorldCityPropsLibrary.CreateBench(transform, new Vector3(0f, 0f, 7.8f), wood, metal);
            benchN.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            GameObject benchS = OpenWorldCityPropsLibrary.CreateBench(transform, new Vector3(0f, 0f, -7.8f), wood, metal);
            benchS.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            GameObject benchW = OpenWorldCityPropsLibrary.CreateBench(transform, new Vector3(-7.8f, 0f, 0f), wood, metal);
            benchW.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            GameObject benchE = OpenWorldCityPropsLibrary.CreateBench(transform, new Vector3(7.8f, 0f, 0f), wood, metal);
            benchE.transform.rotation = Quaternion.Euler(0f, -90f, 0f);

            GameObject lampNW = OpenWorldCityPropsLibrary.CreateStreetLamp(transform, new Vector3(-9.5f, 0f, 9.5f), metal, light);
            LookAtPlazaCenter(lampNW);

            GameObject lampNE = OpenWorldCityPropsLibrary.CreateStreetLamp(transform, new Vector3(9.5f, 0f, 9.5f), metal, light);
            LookAtPlazaCenter(lampNE);

            GameObject lampSW = OpenWorldCityPropsLibrary.CreateStreetLamp(transform, new Vector3(-9.5f, 0f, -9.5f), metal, light);
            LookAtPlazaCenter(lampSW);

            GameObject lampSE = OpenWorldCityPropsLibrary.CreateStreetLamp(transform, new Vector3(9.5f, 0f, -9.5f), metal, light);
            LookAtPlazaCenter(lampSE);

            OpenWorldCityPropsLibrary.CreateFlowerPot(transform, new Vector3(-5.5f, 0f, 5.5f), pot, plant);
            OpenWorldCityPropsLibrary.CreateFlowerPot(transform, new Vector3(5.5f, 0f, 5.5f), pot, plant);
            OpenWorldCityPropsLibrary.CreateFlowerPot(transform, new Vector3(-5.5f, 0f, -5.5f), pot, plant);
            OpenWorldCityPropsLibrary.CreateFlowerPot(transform, new Vector3(5.5f, 0f, -5.5f), pot, plant);

            GameObject sign = OpenWorldCityPropsLibrary.CreateSign(
                transform,
                new Vector3(-11.5f, 0f, 6f),
                "Cartel_Plaza_Madrid",
                metal,
                new Color(0.75f, 0.62f, 0.28f)
            );
            sign.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            OpenWorldCityPropsLibrary.CreateCrate(transform, new Vector3(-13f, 0f, -11f), new Color(0.45f, 0.28f, 0.15f));
            OpenWorldCityPropsLibrary.CreateCrate(transform, new Vector3(-12.2f, 0f, -11f), new Color(0.45f, 0.28f, 0.15f));
        }

        private void CreateTrashCans()
        {
            CreateCube("Papelera_70s", new Vector3(-8f, 0.45f, 6f), new Vector3(0.5f, 0.9f, 0.5f), new Color(0.10f, 0.25f, 0.12f));
            CreateCube("Papelera_70s", new Vector3(8f, 0.45f, 6f), new Vector3(0.5f, 0.9f, 0.5f), new Color(0.10f, 0.25f, 0.12f));
        }

        private void CreatePhoneBooth()
        {
            Vector3 pos = new Vector3(-10.5f, 0f, -4.5f);

            Color red = new Color(0.75f, 0.08f, 0.05f);
            Color darkRed = new Color(0.55f, 0.05f, 0.05f);
            Material glass = CreateTransparentMaterial(new Color(0.45f, 0.75f, 1f, 0.18f));

            // Postes
            CreateCube("Cabina_Poste_FL", pos + new Vector3(-0.6f, 0f, -0.6f), new Vector3(0.12f, 2.4f, 0.12f), red);
            CreateCube("Cabina_Poste_FR", pos + new Vector3(0.6f, 0f, -0.6f), new Vector3(0.12f, 2.4f, 0.12f), red);
            CreateCube("Cabina_Poste_BL", pos + new Vector3(-0.6f, 0f, 0.6f), new Vector3(0.12f, 2.4f, 0.12f), red);
            CreateCube("Cabina_Poste_BR", pos + new Vector3(0.6f, 0f, 0.6f), new Vector3(0.12f, 2.4f, 0.12f), red);

            // Marco inferior y superior
            CreateCube("Cabina_Base", pos, new Vector3(1.35f, 0.12f, 1.35f), darkRed);
            CreateCube("Cabina_Techo", pos + new Vector3(0f, 2.4f, 0f), new Vector3(1.4f, 0.18f, 1.4f), darkRed);

            // Cristales separados, sin bloque sólido detrás
            GameObject frontal = CreateCube(
                "Cristal_Frontal",
                pos + new Vector3(0f, 0.45f, -0.66f),
                new Vector3(1.22f, 1.8f, 0.04f),
                Color.white
            );

            frontal.GetComponent<Renderer>().material = glass;
            frontal.GetComponent<Renderer>().material = glass;

            GameObject izq = CreateCube(
                "Cristal_Izq",
                pos + new Vector3(-0.66f, 0.45f, 0f),
                new Vector3(0.04f, 1.8f, 1.22f),
                Color.white
            );

            izq.GetComponent<Renderer>().material = glass;

            GameObject der = CreateCube(
                "Cristal_Der",
                pos + new Vector3(0.66f, 0.45f, 0f),
                new Vector3(0.04f, 1.8f, 1.22f),
                Color.white
            );

            der.GetComponent<Renderer>().material = glass;

            // Teléfono interior visible
            CreateCube("Telefono_Interior", pos + new Vector3(0f, 0.9f, -0.25f), new Vector3(0.35f, 0.5f, 0.08f), Color.black);
        }

        private Material CreateTransparentMaterial(Color color)
        {
            Material material =
                new Material(Shader.Find("Standard"));

            material.SetFloat("_Mode", 3);

            material.SetInt(
                "_SrcBlend",
                (int)UnityEngine.Rendering.BlendMode.SrcAlpha
            );

            material.SetInt(
                "_DstBlend",
                (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha
            );

            material.SetInt("_ZWrite", 0);

            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

            material.renderQueue = 3000;

            material.color = color;

            return material;
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

        private GameObject CreateCube(
            string name,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            obj.name = name;

            obj.transform.position = new Vector3(
                position.x,
                position.y + scale.y * 0.5f,
                position.z
            );

            obj.transform.localScale = scale;

            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material.color = color;

            return obj;
        }

        private void CreateZonePortals()
        {
            CreateZonePortal(
                "Portal_Barcelona",
                new Vector3(-18f, 0f, -18f),
                OpenWorldThemeId.Barcelona70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.20f, 0.55f, 0.85f)
            );

            CreateZonePortal(
                "Portal_Valencia",
                new Vector3(18f, 0f, -18f),
                OpenWorldThemeId.Valencia70s,
                new Vector3(0f, 0.1f, -4f),
                new Color(0.95f, 0.55f, 0.15f)
            );

            if (ModuleZGameState.AndaluciaUnlocked)
            {
                CreateZonePortal(
                    "Portal_Andalucia",
                    new Vector3(18f, 0f, 18f),
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

            portalRoot.transform.position = position;

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

        private void LookAtPlazaCenter(GameObject obj)
        {
            Vector3 direction = Vector3.zero - obj.transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            obj.transform.rotation =
                Quaternion.LookRotation(direction.normalized, Vector3.up);
        }
    }
}