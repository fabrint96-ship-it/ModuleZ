using System.Collections.Generic;
using ModuleZ.Duel3D.Board;
using UnityEngine;

namespace ModuleZ.Duel3D.Rules
{
    public static class Duel3DGroupResolver
    {
        private const int RemovePerLine = 5;

        private static readonly Vector3Int[] Directions =
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 0, 1)
        };

        public static int ResolveGroups(Duel3DBoardGrid board)
        {
            List<Vector3Int> removedCells = ResolveGroupsAndReturnCells(board);
            return removedCells.Count;
        }

        public static List<Vector3Int> ResolveGroupsAndReturnCells(Duel3DBoardGrid board)
        {
            HashSet<Vector3Int> cellsToRemove = FindCellsToRemove(board);
            List<Vector3Int> result = new List<Vector3Int>(cellsToRemove);

            for (int i = 0; i < result.Count; i++)
                board.RemoveCell(result[i]);

            return result;
        }

        public static int CountResolvableCells(
            Duel3DBoardGrid board,
            Duel3DCellOwner owner)
        {
            if (board == null || owner == Duel3DCellOwner.Empty)
                return 0;

            return FindCellsToRemove(board, owner).Count;
        }

        private static HashSet<Vector3Int> FindCellsToRemove(
            Duel3DBoardGrid board,
            Duel3DCellOwner filterOwner = Duel3DCellOwner.Empty)
        {
            HashSet<Vector3Int> result = new HashSet<Vector3Int>();

            if (board == null)
                return result;

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    for (int z = 0; z < board.Depth; z++)
                    {
                        Vector3Int start = new Vector3Int(x, y, z);
                        Duel3DCellOwner owner = board.GetCell(start);

                        if (owner == Duel3DCellOwner.Empty)
                            continue;

                        if (filterOwner != Duel3DCellOwner.Empty && owner != filterOwner)
                            continue;

                        for (int d = 0; d < Directions.Length; d++)
                        {
                            AddLineRemoval(
                                board,
                                start,
                                Directions[d],
                                owner,
                                result
                            );
                        }
                    }
                }
            }

            return result;
        }

        private static void AddLineRemoval(
            Duel3DBoardGrid board,
            Vector3Int start,
            Vector3Int direction,
            Duel3DCellOwner owner,
            HashSet<Vector3Int> result)
        {
            Vector3Int previous = start - direction;

            if (board.IsInside(previous) && board.GetCell(previous) == owner)
                return;

            List<Vector3Int> line = new List<Vector3Int>();
            Vector3Int current = start;

            while (board.IsInside(current) && board.GetCell(current) == owner)
            {
                line.Add(current);
                current += direction;
            }

            int groupsOfThree = line.Count / RemovePerLine;
            int removeCount = groupsOfThree * RemovePerLine;

            for (int i = 0; i < removeCount; i++)
                result.Add(line[i]);
        }
    }
}