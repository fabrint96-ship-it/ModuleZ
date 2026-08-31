using ModuleZ.Duel3D.Board;
using UnityEngine;

namespace ModuleZ.Duel3D.Visuals
{
    public static class Duel3DForbiddenCellsPreview
    {
        public static void Build(
            Transform parent,
            Duel3DBoardGrid board,
            Vector3Int[] lastPieceCells,
            System.Func<Vector3Int, Vector3> gridToWorld,
            float cellSize,
            Material material)
        {
            if (parent == null || board == null || lastPieceCells == null || gridToWorld == null)
                return;

            for (int i = 0; i < lastPieceCells.Length; i++)
            {
                Vector3Int cell = lastPieceCells[i];

                if (!board.IsInside(cell))
                    continue;

                if (board.GetCell(cell) == Duel3DCellOwner.Empty)
                    continue;

                GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                marker.name = "ForbiddenCellMarker";
                marker.transform.SetParent(parent);
                marker.transform.position = gridToWorld(cell);
                marker.transform.localScale = Vector3.one * (cellSize * 1.05f);

                Renderer renderer = marker.GetComponent<Renderer>();
                renderer.material = material;
            }
        }
    }
}