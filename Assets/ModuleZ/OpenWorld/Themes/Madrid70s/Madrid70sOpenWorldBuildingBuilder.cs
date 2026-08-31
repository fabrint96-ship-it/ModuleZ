using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Madrid70s
{
    public class Madrid70sOpenWorldBuildingBuilder : MonoBehaviour
    {
        public void Build()
        {
            Debug.Log("### MADRID BUILDINGS BUILD EJECUTADO ###");

            CreateBuildings();

            Debug.Log("[Module Z] Edificios Madrid años 70 creados.");
        }

        private void CreateBuildings()
        {
            CreateHouse("Casa_Oeste_01", new Vector3(-17f, 0f, -8f), new Vector3(6f, 5f, 4.5f), new Color(0.78f, 0.66f, 0.50f), true);
            CreateHouse("Casa_Oeste_02", new Vector3(-17f, 0f, 0f), new Vector3(6f, 5.5f, 4.5f), new Color(0.72f, 0.62f, 0.48f), true);
            CreateHouse("Casa_Oeste_03", new Vector3(-17f, 0f, 8f), new Vector3(6f, 5f, 4.5f), new Color(0.82f, 0.70f, 0.55f), true);

            CreateHouse("Casa_Este_01", new Vector3(17f, 0f, -8f), new Vector3(6f, 5f, 4.5f), new Color(0.75f, 0.66f, 0.48f), false);
            CreateHouse("Casa_Este_02", new Vector3(17f, 0f, 0f), new Vector3(6f, 5.5f, 4.5f), new Color(0.68f, 0.62f, 0.45f), false);
            CreateHouse("Casa_Este_03", new Vector3(17f, 0f, 8f), new Vector3(6f, 5f, 4.5f), new Color(0.82f, 0.72f, 0.52f), false);

            CreateShop("Bar_Madrid", new Vector3(-5.5f, 0f, 17f), new Vector3(8f, 4.5f, 4.5f), new Color(0.64f, 0.42f, 0.25f), false, new Color(0.65f, 0.16f, 0.12f));
            CreateShop("Tienda_Madrid", new Vector3(5.5f, 0f, 17f), new Vector3(8f, 4.5f, 4.5f), new Color(0.56f, 0.45f, 0.28f), false, new Color(0.15f, 0.30f, 0.65f));

            CreatePublicBuilding("Ayuntamiento_Madrid", new Vector3(0f, 0f, -17f), new Vector3(11f, 5.2f, 4.5f), new Color(0.80f, 0.74f, 0.60f), true);
        }

        private void CreateHouse(string name, Vector3 basePos, Vector3 scale, Color color, bool facePlazaFromWest)
        {
            GameObject root = new GameObject(name);
            root.transform.position = basePos;

            CreateCube(root.transform, "Cuerpo", new Vector3(0f, scale.y * 0.5f, 0f), scale, color);
            CreateCube(root.transform, "Tejado", new Vector3(0f, scale.y + 0.15f, 0f), new Vector3(scale.x + 0.3f, 0.3f, scale.z + 0.3f), new Color(0.28f, 0.15f, 0.08f));

            float frontX = facePlazaFromWest ? scale.x * 0.5f + 0.06f : -scale.x * 0.5f - 0.06f;
            float rotY = facePlazaFromWest ? 90f : -90f;

            CreateFlat(root.transform, "Puerta", new Vector3(frontX, 1.1f, 0f), new Vector3(1.1f, 2.2f, 0.1f), rotY, new Color(0.16f, 0.08f, 0.04f));

            CreateFlat(root.transform, "Ventana_A", new Vector3(frontX, 3.1f, -1.5f), new Vector3(0.8f, 0.75f, 0.08f), rotY, new Color(0.12f, 0.18f, 0.22f));
            CreateFlat(root.transform, "Ventana_B", new Vector3(frontX, 3.1f, 1.5f), new Vector3(0.8f, 0.75f, 0.08f), rotY, new Color(0.12f, 0.18f, 0.22f));

            if (scale.y > 5.2f)
            {
                CreateFlat(root.transform, "Ventana_C", new Vector3(frontX, 4.5f, -1.5f), new Vector3(0.8f, 0.75f, 0.08f), rotY, new Color(0.12f, 0.18f, 0.22f));
                CreateFlat(root.transform, "Ventana_D", new Vector3(frontX, 4.5f, 1.5f), new Vector3(0.8f, 0.75f, 0.08f), rotY, new Color(0.12f, 0.18f, 0.22f));
            }
        }

        private void CreateShop(string name, Vector3 basePos, Vector3 scale, Color color, bool frontPositiveZ, Color awningColor)
        {
            GameObject root = new GameObject(name);
            root.transform.position = basePos;

            CreateCube(root.transform, "Cuerpo", new Vector3(0f, scale.y * 0.5f, 0f), scale, color);
            CreateCube(root.transform, "Tejado", new Vector3(0f, scale.y + 0.15f, 0f), new Vector3(scale.x + 0.3f, 0.3f, scale.z + 0.3f), new Color(0.25f, 0.13f, 0.08f));

            float frontZ = frontPositiveZ ? scale.z * 0.5f + 0.06f : -scale.z * 0.5f - 0.06f;
            float rotY = frontPositiveZ ? 0f : 180f;

            CreateFlat(root.transform, "Puerta", new Vector3(0f, 1.1f, frontZ), new Vector3(1.1f, 2.2f, 0.1f), rotY, new Color(0.15f, 0.07f, 0.03f));
            CreateFlat(root.transform, "Escaparate_Izq", new Vector3(-2.3f, 1.4f, frontZ), new Vector3(1.5f, 1.4f, 0.1f), rotY, new Color(0.35f, 0.55f, 0.68f));
            CreateFlat(root.transform, "Escaparate_Der", new Vector3(2.3f, 1.4f, frontZ), new Vector3(1.5f, 1.4f, 0.1f), rotY, new Color(0.35f, 0.55f, 0.68f));
            CreateFlat(root.transform, "Toldo", new Vector3(0f, 2.35f, frontZ), new Vector3(5.8f, 0.25f, 0.65f), rotY, awningColor);
            CreateFlat(root.transform, "Cartel", new Vector3(0f, 3.2f, frontZ), new Vector3(4.5f, 0.55f, 0.12f), rotY, new Color(0.85f, 0.65f, 0.22f));
        }

        private void CreatePublicBuilding(string name, Vector3 basePos, Vector3 scale, Color color, bool frontPositiveZ)
        {
            GameObject root = new GameObject(name);
            root.transform.position = basePos;

            CreateCube(root.transform, "Cuerpo", new Vector3(0f, scale.y * 0.5f, 0f), scale, color);
            CreateCube(root.transform, "Tejado", new Vector3(0f, scale.y + 0.15f, 0f), new Vector3(scale.x + 0.4f, 0.3f, scale.z + 0.4f), new Color(0.22f, 0.13f, 0.08f));

            float frontZ = frontPositiveZ ? scale.z * 0.5f + 0.06f : -scale.z * 0.5f - 0.06f;
            float rotY = frontPositiveZ ? 0f : 180f;

            CreateFlat(root.transform, "Puerta", new Vector3(0f, 1.15f, frontZ), new Vector3(1.2f, 2.3f, 0.1f), rotY, new Color(0.12f, 0.06f, 0.03f));
            CreateFlat(root.transform, "Columna_Izq", new Vector3(-1.2f, 1.6f, frontZ), new Vector3(0.18f, 3.2f, 0.18f), rotY, new Color(0.90f, 0.82f, 0.65f));
            CreateFlat(root.transform, "Columna_Der", new Vector3(1.2f, 1.6f, frontZ), new Vector3(0.18f, 3.2f, 0.18f), rotY, new Color(0.90f, 0.82f, 0.65f));
            CreateFlat(root.transform, "Cartel_Ayto", new Vector3(0f, 3.5f, frontZ), new Vector3(4.2f, 0.55f, 0.12f), rotY, new Color(0.85f, 0.65f, 0.22f));
        }

        private GameObject CreateCube(Transform parent, string name, Vector3 localPos, Vector3 localScale, Color color)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = localScale;
            obj.GetComponent<Renderer>().material.color = color;
            return obj;
        }

        private GameObject CreateFlat(Transform parent, string name, Vector3 localPos, Vector3 localScale, float rotationY, Color color)
        {
            GameObject obj = CreateCube(parent, name, localPos, localScale, color);
            obj.transform.localRotation = Quaternion.Euler(0f, rotationY, 0f);
            return obj;
        }
    }
}