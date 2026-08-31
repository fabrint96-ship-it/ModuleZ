using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Madrid70s
{
    public static class Madrid70sBuildingStyleBuilder
    {
        public static GameObject CreateResidentialBuilding(
            Transform parent,
            string name,
            Vector3 position,
            Vector3 scale,
            Color facadeColor,
            bool frontFacesPositiveZ)
        {
            GameObject root = CreateRoot(parent, name, position);

            CreateShell(root.transform, scale, facadeColor);
            CreateBaseTrim(root.transform, scale);
            CreateRoof(root.transform, scale, new Color(0.30f, 0.18f, 0.10f));
            CreateDoor(root.transform, scale, frontFacesPositiveZ, new Color(0.16f, 0.08f, 0.04f));
            CreateResidentialWindows(root.transform, scale, frontFacesPositiveZ);
            CreateSmallBalconies(root.transform, scale, frontFacesPositiveZ);

            return root;
        }

        public static GameObject CreateCommercialBuilding(
            Transform parent,
            string name,
            Vector3 position,
            Vector3 scale,
            Color facadeColor,
            bool frontFacesPositiveZ,
            string signName,
            Color awningColor)
        {
            GameObject root = CreateRoot(parent, name, position);

            CreateShell(root.transform, scale, facadeColor);
            CreateBaseTrim(root.transform, scale);
            CreateRoof(root.transform, scale, new Color(0.28f, 0.16f, 0.09f));

            CreateDoor(root.transform, scale, frontFacesPositiveZ, new Color(0.12f, 0.06f, 0.03f));
            CreateShopWindows(root.transform, scale, frontFacesPositiveZ);
            CreateShopSign(root.transform, scale, frontFacesPositiveZ, signName);
            CreateAwning(root.transform, scale, frontFacesPositiveZ, awningColor);

            return root;
        }

        public static GameObject CreatePublicBuilding(
            Transform parent,
            string name,
            Vector3 position,
            Vector3 scale,
            Color facadeColor,
            bool frontFacesPositiveZ,
            string signName)
        {
            GameObject root = CreateRoot(parent, name, position);

            CreateShell(root.transform, scale, facadeColor);
            CreateBaseTrim(root.transform, scale);
            CreateRoof(root.transform, scale, new Color(0.22f, 0.14f, 0.09f));

            CreateDoor(root.transform, scale, frontFacesPositiveZ, new Color(0.10f, 0.06f, 0.03f));
            CreateTallWindows(root.transform, scale, frontFacesPositiveZ);
            CreateShopSign(root.transform, scale, frontFacesPositiveZ, signName);
            CreateEntranceColumns(root.transform, scale, frontFacesPositiveZ);

            return root;
        }

        private static GameObject CreateRoot(
            Transform parent,
            string name,
            Vector3 position)
        {
            GameObject root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;
            return root;
        }

        private static GameObject CreateShell(
            Transform parent,
            Vector3 scale,
            Color color)
        {
            GameObject shell = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shell.name = "Fachada";
            shell.transform.SetParent(parent, false);
            shell.transform.localPosition = new Vector3(0f, scale.y * 0.5f, 0f);
            shell.transform.localScale = scale;

            Renderer renderer = shell.GetComponent<Renderer>();
            renderer.material.color = color;

            return shell;
        }

        private static void CreateBaseTrim(Transform parent, Vector3 scale)
        {
            CreatePart(
                "Zocalo",
                parent,
                new Vector3(0f, 0.18f, 0f),
                new Vector3(scale.x + 0.08f, 0.35f, scale.z + 0.08f),
                new Color(0.28f, 0.22f, 0.18f)
            );
        }

        private static void CreateRoof(Transform parent, Vector3 scale, Color color)
        {
            CreatePart(
                "Azotea",
                parent,
                new Vector3(0f, scale.y + 0.14f, 0f),
                new Vector3(scale.x + 0.35f, 0.28f, scale.z + 0.35f),
                color
            );

            CreatePart(
                "Cornisa_Frontal",
                parent,
                new Vector3(0f, scale.y - 0.1f, scale.z * 0.5f + 0.08f),
                new Vector3(scale.x + 0.25f, 0.18f, 0.16f),
                color
            );

            CreatePart(
                "Cornisa_Trasera",
                parent,
                new Vector3(0f, scale.y - 0.1f, -scale.z * 0.5f - 0.08f),
                new Vector3(scale.x + 0.25f, 0.18f, 0.16f),
                color
            );
        }

        private static void CreateDoor(
            Transform parent,
            Vector3 scale,
            bool frontFacesPositiveZ,
            Color color)
        {
            float z = GetFrontZ(scale, frontFacesPositiveZ);

            CreatePart(
                "Puerta",
                parent,
                new Vector3(0f, 1.1f, z),
                new Vector3(1.15f, 2.2f, 0.12f),
                color
            );
        }

        private static void CreateResidentialWindows(
            Transform parent,
            Vector3 scale,
            bool frontFacesPositiveZ)
        {
            float z = GetFrontZ(scale, frontFacesPositiveZ);
            float[] xs = { -2.8f, 0f, 2.8f };
            float[] ys = { 3.1f, 4.7f };

            for (int y = 0; y < ys.Length; y++)
            {
                if (ys[y] > scale.y - 0.4f)
                    continue;

                for (int x = 0; x < xs.Length; x++)
                {
                    if (Mathf.Abs(xs[x]) < 0.1f && y == 0)
                        continue;

                    CreateWindow(
                        parent,
                        new Vector3(xs[x], ys[y], z)
                    );
                }
            }
        }

        private static void CreateShopWindows(
            Transform parent,
            Vector3 scale,
            bool frontFacesPositiveZ)
        {
            float z = GetFrontZ(scale, frontFacesPositiveZ);

            CreatePart(
                "Escaparate_Izq",
                parent,
                new Vector3(-2.4f, 1.45f, z),
                new Vector3(1.6f, 1.45f, 0.1f),
                new Color(0.55f, 0.80f, 1f)
            );

            CreatePart(
                "Escaparate_Der",
                parent,
                new Vector3(2.4f, 1.45f, z),
                new Vector3(1.6f, 1.45f, 0.1f),
                new Color(0.55f, 0.80f, 1f)
            );

            float[] xs = { -3.4f, 0f, 3.4f };

            for (int i = 0; i < xs.Length; i++)
            {
                CreateWindow(
                    parent,
                    new Vector3(xs[i], 3.8f, z)
                );
            }
        }

        private static void CreateTallWindows(
            Transform parent,
            Vector3 scale,
            bool frontFacesPositiveZ)
        {
            float z = GetFrontZ(scale, frontFacesPositiveZ);
            float[] xs = { -3.2f, 3.2f };

            for (int i = 0; i < xs.Length; i++)
            {
                CreatePart(
                    "Ventana_Alta",
                    parent,
                    new Vector3(xs[i], 3.1f, z),
                    new Vector3(1.0f, 2.2f, 0.1f),
                    new Color(0.55f, 0.80f, 1f)
                );
            }
        }

        private static void CreateSmallBalconies(
            Transform parent,
            Vector3 scale,
            bool frontFacesPositiveZ)
        {
            float z = GetFrontZ(scale, frontFacesPositiveZ);
            float balconyZ = frontFacesPositiveZ ? z + 0.18f : z - 0.18f;

            float[] xs = { -2.8f, 2.8f };

            for (int i = 0; i < xs.Length; i++)
            {
                CreatePart(
                    "Balcon",
                    parent,
                    new Vector3(xs[i], 2.65f, balconyZ),
                    new Vector3(1.25f, 0.12f, 0.45f),
                    new Color(0.12f, 0.12f, 0.12f)
                );

                CreatePart(
                    "Barandilla",
                    parent,
                    new Vector3(xs[i], 2.95f, balconyZ),
                    new Vector3(1.25f, 0.35f, 0.08f),
                    new Color(0.05f, 0.05f, 0.05f)
                );
            }
        }

        private static void CreateShopSign(
            Transform parent,
            Vector3 scale,
            bool frontFacesPositiveZ,
            string signName)
        {
            float z = GetFrontZ(scale, frontFacesPositiveZ);
            float signZ = frontFacesPositiveZ ? z + 0.08f : z - 0.08f;

            GameObject sign = CreatePart(
                "Cartel_" + signName,
                parent,
                new Vector3(0f, 3.0f, signZ),
                new Vector3(4.8f, 0.55f, 0.14f),
                new Color(0.78f, 0.58f, 0.20f)
            );
        }

        private static void CreateAwning(
            Transform parent,
            Vector3 scale,
            bool frontFacesPositiveZ,
            Color color)
        {
            float z = GetFrontZ(scale, frontFacesPositiveZ);
            float awningZ = frontFacesPositiveZ ? z + 0.18f : z - 0.18f;

            CreatePart(
                "Toldo",
                parent,
                new Vector3(0f, 2.35f, awningZ),
                new Vector3(5.5f, 0.18f, 0.65f),
                color
            );
        }

        private static void CreateEntranceColumns(
            Transform parent,
            Vector3 scale,
            bool frontFacesPositiveZ)
        {
            float z = GetFrontZ(scale, frontFacesPositiveZ);
            float columnZ = frontFacesPositiveZ ? z + 0.15f : z - 0.15f;

            CreatePart(
                "Columna_Izq",
                parent,
                new Vector3(-1.0f, 1.5f, columnZ),
                new Vector3(0.18f, 3.0f, 0.18f),
                new Color(0.85f, 0.78f, 0.62f)
            );

            CreatePart(
                "Columna_Der",
                parent,
                new Vector3(1.0f, 1.5f, columnZ),
                new Vector3(0.18f, 3.0f, 0.18f),
                new Color(0.85f, 0.78f, 0.62f)
            );
        }

        private static void CreateWindow(Transform parent, Vector3 position)
        {
            CreatePart(
                "Ventana",
                parent,
                position,
                new Vector3(0.85f, 0.75f, 0.09f),
                new Color(0.45f, 0.65f, 0.78f)
            );

            CreatePart(
                "Marco_Ventana",
                parent,
                position + new Vector3(0f, -0.45f, 0.02f),
                new Vector3(0.95f, 0.08f, 0.11f),
                new Color(0.12f, 0.12f, 0.12f)
            );
        }

        private static GameObject CreatePart(
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 localScale,
            Color color)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;

            Renderer renderer = part.GetComponent<Renderer>();
            renderer.material.color = color;

            return part;
        }

        private static float GetFrontZ(
            Vector3 scale,
            bool frontFacesPositiveZ)
        {
            return frontFacesPositiveZ
                ? scale.z * 0.5f + 0.06f
                : -scale.z * 0.5f - 0.06f;
        }
    }
}