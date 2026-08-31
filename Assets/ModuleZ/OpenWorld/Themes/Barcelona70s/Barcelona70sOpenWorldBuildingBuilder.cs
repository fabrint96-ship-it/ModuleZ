using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Barcelona70s
{
    public class Barcelona70sOpenWorldBuildingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateBuildings();
            Debug.Log("[Module Z] Edificios Barcelona años 70 creados.");
        }

        private void CreateBuildings()
        {
            CreateBlock("Barcelona_Norte_A", new Vector3(-8f, 0f, 16f), new Vector3(8f, 6f, 4.5f), new Color(0.84f, 0.78f, 0.68f), false);
            CreateBlock("Barcelona_Norte_B", new Vector3(4f, 0f, 16f), new Vector3(9f, 7f, 4.5f), new Color(0.76f, 0.70f, 0.60f), false);

            CreateBlock("Barcelona_Sur_A", new Vector3(-7f, 0f, -16f), new Vector3(8f, 6f, 4.5f), new Color(0.82f, 0.74f, 0.64f), true);
            CreateBlock("Barcelona_Sur_B", new Vector3(6f, 0f, -16f), new Vector3(9f, 7f, 4.5f), new Color(0.88f, 0.80f, 0.70f), true);

            CreateBlockSide("Barcelona_Oeste_A", new Vector3(-16f, 0f, -5f), new Vector3(4.5f, 6f, 8f), new Color(0.78f, 0.70f, 0.60f), true);
            CreateBlockSide("Barcelona_Oeste_B", new Vector3(-16f, 0f, 7f), new Vector3(4.5f, 7f, 8f), new Color(0.86f, 0.78f, 0.68f), true);

            CreateBlockSide("Barcelona_Este_A", new Vector3(16f, 0f, -4f), new Vector3(4.5f, 6f, 8f), new Color(0.80f, 0.72f, 0.62f), false);
            CreateBlockSide("Barcelona_Este_B", new Vector3(16f, 0f, 8f), new Vector3(4.5f, 7f, 8f), new Color(0.74f, 0.68f, 0.58f), false);
        }

        private void CreateBlock(string name, Vector3 basePos, Vector3 scale, Color color, bool frontPositiveZ)
        {
            GameObject root = new GameObject(name);
            root.transform.position = basePos;

            CreateCube(root.transform, "Cuerpo", new Vector3(0f, scale.y * 0.5f, 0f), scale, color);
            CreateCube(root.transform, "Azotea", new Vector3(0f, scale.y + 0.12f, 0f), new Vector3(scale.x + 0.25f, 0.25f, scale.z + 0.25f), new Color(0.25f, 0.18f, 0.12f));

            float frontZ = frontPositiveZ ? scale.z * 0.5f + 0.06f : -scale.z * 0.5f - 0.06f;
            float rotY = frontPositiveZ ? 0f : 180f;

            CreateFlat(root.transform, "Puerta", new Vector3(0f, 1.1f, frontZ), new Vector3(1.1f, 2.2f, 0.08f), rotY, new Color(0.12f, 0.07f, 0.04f));

            CreateFlat(root.transform, "Ventana_1", new Vector3(-2.5f, 3.1f, frontZ), new Vector3(0.8f, 0.75f, 0.08f), rotY, new Color(0.35f, 0.55f, 0.70f));
            CreateFlat(root.transform, "Ventana_2", new Vector3(2.5f, 3.1f, frontZ), new Vector3(0.8f, 0.75f, 0.08f), rotY, new Color(0.35f, 0.55f, 0.70f));

            if (scale.y > 6f)
            {
                CreateFlat(root.transform, "Ventana_3", new Vector3(-2.5f, 4.6f, frontZ), new Vector3(0.8f, 0.75f, 0.08f), rotY, new Color(0.35f, 0.55f, 0.70f));
                CreateFlat(root.transform, "Ventana_4", new Vector3(2.5f, 4.6f, frontZ), new Vector3(0.8f, 0.75f, 0.08f), rotY, new Color(0.35f, 0.55f, 0.70f));
            }

            CreateFlat(root.transform, "Detalle_Mosaico", new Vector3(0f, 2.55f, frontZ), new Vector3(5.5f, 0.25f, 0.09f), rotY, new Color(0.20f, 0.35f, 0.75f));
        }

        private void CreateBlockSide(string name, Vector3 basePos, Vector3 scale, Color color, bool frontPositiveX)
        {
            GameObject root = new GameObject(name);
            root.transform.position = basePos;

            CreateCube(root.transform, "Cuerpo", new Vector3(0f, scale.y * 0.5f, 0f), scale, color);
            CreateCube(root.transform, "Azotea", new Vector3(0f, scale.y + 0.12f, 0f), new Vector3(scale.x + 0.25f, 0.25f, scale.z + 0.25f), new Color(0.25f, 0.18f, 0.12f));

            float frontX = frontPositiveX ? scale.x * 0.5f + 0.06f : -scale.x * 0.5f - 0.06f;
            float rotY = frontPositiveX ? 90f : -90f;

            CreateFlat(root.transform, "Puerta", new Vector3(frontX, 1.1f, 0f), new Vector3(1.1f, 2.2f, 0.08f), rotY, new Color(0.12f, 0.07f, 0.04f));
            CreateFlat(root.transform, "Ventana_1", new Vector3(frontX, 3.1f, -2f), new Vector3(0.8f, 0.75f, 0.08f), rotY, new Color(0.35f, 0.55f, 0.70f));
            CreateFlat(root.transform, "Ventana_2", new Vector3(frontX, 3.1f, 2f), new Vector3(0.8f, 0.75f, 0.08f), rotY, new Color(0.35f, 0.55f, 0.70f));
            CreateFlat(root.transform, "Detalle_Mosaico", new Vector3(frontX, 2.55f, 0f), new Vector3(5f, 0.25f, 0.09f), rotY, new Color(0.20f, 0.35f, 0.75f));
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