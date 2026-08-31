using UnityEngine;

namespace ModuleZ.Duel3D.Visuals
{
    public static class Duel3DBoardBoundsBuilder
    {
        public static void BuildBounds(
            Transform parent,
            int width,
            int height,
            int depth,
            float cellSize,
            Material material)
        {
            float boardWidth = width * cellSize;
            float boardDepth = depth * cellSize;
            float boardHeight = height * cellSize;

            float minX = -boardWidth * 0.5f;
            float maxX = boardWidth * 0.5f;
            float minZ = -boardDepth * 0.5f;
            float maxZ = boardDepth * 0.5f;
            float minY = 0f;
            float maxY = boardHeight;

            BuildVertical(parent, new Vector3(minX, boardHeight * 0.5f, minZ), boardHeight, cellSize, material);
            BuildVertical(parent, new Vector3(maxX, boardHeight * 0.5f, minZ), boardHeight, cellSize, material);
            BuildVertical(parent, new Vector3(minX, boardHeight * 0.5f, maxZ), boardHeight, cellSize, material);
            BuildVertical(parent, new Vector3(maxX, boardHeight * 0.5f, maxZ), boardHeight, cellSize, material);

            BuildHorizontal(parent, new Vector3(0f, maxY, minZ), boardWidth, true, cellSize, material);
            BuildHorizontal(parent, new Vector3(0f, maxY, maxZ), boardWidth, true, cellSize, material);
            BuildHorizontal(parent, new Vector3(minX, maxY, 0f), boardDepth, false, cellSize, material);
            BuildHorizontal(parent, new Vector3(maxX, maxY, 0f), boardDepth, false, cellSize, material);

            BuildHorizontal(parent, new Vector3(0f, minY, minZ), boardWidth, true, cellSize, material);
            BuildHorizontal(parent, new Vector3(0f, minY, maxZ), boardWidth, true, cellSize, material);
            BuildHorizontal(parent, new Vector3(minX, minY, 0f), boardDepth, false, cellSize, material);
            BuildHorizontal(parent, new Vector3(maxX, minY, 0f), boardDepth, false, cellSize, material);
        }

        private static void BuildVertical(
            Transform parent,
            Vector3 position,
            float height,
            float cellSize,
            Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "Duel3D_Bounds_Vertical";
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.transform.localScale = new Vector3(
                cellSize * 0.06f,
                height,
                cellSize * 0.06f
            );

            obj.GetComponent<Renderer>().material = material;
        }

        private static void BuildHorizontal(
            Transform parent,
            Vector3 position,
            float length,
            bool alongX,
            float cellSize,
            Material material)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = "Duel3D_Bounds_Horizontal";
            obj.transform.SetParent(parent);
            obj.transform.position = position;

            obj.transform.localScale = alongX
                ? new Vector3(length, cellSize * 0.06f, cellSize * 0.06f)
                : new Vector3(cellSize * 0.06f, cellSize * 0.06f, length);

            obj.GetComponent<Renderer>().material = material;
        }
    }
}