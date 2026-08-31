using System.Drawing;
using UnityEngine;

namespace ModuleZ.Duel3D.Visuals
{
    public static class Duel3DPieceVisualBuilder
    {
        public static GameObject CreateCube(
            string name,
            Transform parent,
            Vector3 position,
            float size,
            Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent);
            cube.transform.position = position;
            cube.transform.localScale = Vector3.one * size;

            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material = material;

            return cube;
        }

        public static GameObject CreatePreviewCube(
            string name,
            Transform parent,
            Vector3 position,
            float size,
            Material material,
            bool valid)
        {
            GameObject cube = CreateCube(name, parent, position, size, material);

            cube.transform.localScale = Vector3.one * (valid ? size : size * 0.95f);

            return cube;
        }

        public static GameObject CreatePlacedCube(
            string name,
            Transform parent,
            Vector3 position,
            float size,
            Material material)
        {
            return CreateCube(name, parent, position, size, material);
        }

        private static Material CreateOutlineMaterial(bool valid)
        {
            Shader shader = Shader.Find("Standard");

            if (shader == null)
            {
                shader = Shader.Find("Diffuse");
            }

            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            Material material = new Material(shader);

            material.color = valid
                ? new UnityEngine.Color(0.8f, 1f, 0.8f, 0.45f)
                : new UnityEngine.Color(1f, 0.35f, 0.35f, 0.45f);

            return material;
        }

        private static Material CreateHighlightMaterial()
        {
            Shader shader = Shader.Find("Standard");

            if (shader == null)
            {
                shader = Shader.Find("Diffuse");
            }

            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }

            Material material = new Material(shader);

            material.color = new UnityEngine.Color(1f, 1f, 1f, 0.28f);

            return material;
        }
    }
}