using ModuleZ.Duel3D.Board;
using UnityEngine;

namespace ModuleZ.Duel3D.Pieces
{
    public static class Duel3DPiecePlacement
    {
        public static bool CanPlacePiece(
            Duel3DBoardGrid board,
            Vector3Int origin,
            int rotationIndex)
        {
            if (board == null)
                return false;

            Vector3Int[] cells = ZPiece3DShape.GetCells(origin, rotationIndex);

            for (int i = 0; i < cells.Length; i++)
            {
                if (!board.IsInside(cells[i]))
                    return false;

                if (!board.IsEmpty(cells[i]))
                    return false;
            }

            return true;
        }

        public static bool PlacePiece(
            Duel3DBoardGrid board,
            Vector3Int origin,
            int rotationIndex,
            Duel3DCellOwner owner)
        {
            if (!CanPlacePiece(board, origin, rotationIndex))
                return false;

            Vector3Int[] cells = ZPiece3DShape.GetCells(origin, rotationIndex);

            for (int i = 0; i < cells.Length; i++)
                board.SetCell(cells[i], owner);

            return true;
        }

        public static Vector3Int[] GetPreviewCells(Vector3Int origin, int rotationIndex)
        {
            return ZPiece3DShape.GetCells(origin, rotationIndex);
        }

        public static bool TouchesForbiddenCells(
            Duel3DBoardGrid board,
            Vector3Int[] pieceCells,
            Vector3Int[] forbiddenCells)
        {
            if (board == null || pieceCells == null || forbiddenCells == null)
                return false;

            Vector3Int[] directions =
            {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(0, 0, 1),
                new Vector3Int(0, 0, -1)
            };

            for (int i = 0; i < forbiddenCells.Length; i++)
            {
                if (!board.IsInside(forbiddenCells[i]))
                    continue;

                if (board.GetCell(forbiddenCells[i]) == Duel3DCellOwner.Empty)
                    continue;

                for (int j = 0; j < pieceCells.Length; j++)
                {
                    if (pieceCells[j] == forbiddenCells[i])
                        return true;

                    for (int d = 0; d < directions.Length; d++)
                    {
                        if (pieceCells[j] + directions[d] == forbiddenCells[i])
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool CanPlacePiece(
    Duel3DBoardGrid board,
    Vector3Int origin,
    ZPiece3DRotationState rotation)
        {
            if (board == null)
                return false;

            Vector3Int[] cells = ZPiece3DShape.GetCells(origin, rotation);

            for (int i = 0; i < cells.Length; i++)
            {
                if (!board.IsInside(cells[i]))
                    return false;

                if (!board.IsEmpty(cells[i]))
                    return false;
            }

            return true;
        }

        public static bool PlacePiece(
            Duel3DBoardGrid board,
            Vector3Int origin,
            ZPiece3DRotationState rotation,
            Duel3DCellOwner owner)
        {
            if (!CanPlacePiece(board, origin, rotation))
                return false;

            Vector3Int[] cells = ZPiece3DShape.GetCells(origin, rotation);

            for (int i = 0; i < cells.Length; i++)
                board.SetCell(cells[i], owner);

            return true;
        }
    }
}