using UnityEngine;

namespace ModuleZ.Duel3D.Board
{
    public enum Duel3DCellOwner
    {
        Empty,
        Player,
        Opponent
    }

    public class Duel3DBoardGrid
    {
        private readonly Duel3DCellOwner[,,] cells;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }

        public Duel3DBoardGrid(int width, int height, int depth)
        {
            Width = Mathf.Max(1, width);
            Height = Mathf.Max(1, height);
            Depth = Mathf.Max(1, depth);

            cells = new Duel3DCellOwner[Width, Height, Depth];

            Clear();
        }

        public void Clear()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        cells[x, y, z] = Duel3DCellOwner.Empty;
                    }
                }
            }
        }

        public bool IsInside(Vector3Int position)
        {
            return IsInside(position.x, position.y, position.z);
        }

        public bool IsInside(int x, int y, int z)
        {
            return
                x >= 0 && x < Width &&
                y >= 0 && y < Height &&
                z >= 0 && z < Depth;
        }

        public Duel3DCellOwner GetCell(Vector3Int position)
        {
            return GetCell(position.x, position.y, position.z);
        }

        public Duel3DCellOwner GetCell(int x, int y, int z)
        {
            if (!IsInside(x, y, z))
                return Duel3DCellOwner.Empty;

            return cells[x, y, z];
        }

        public bool IsEmpty(Vector3Int position)
        {
            return IsInside(position) && GetCell(position) == Duel3DCellOwner.Empty;
        }

        public bool SetCell(Vector3Int position, Duel3DCellOwner owner)
        {
            return SetCell(position.x, position.y, position.z, owner);
        }

        public bool SetCell(int x, int y, int z, Duel3DCellOwner owner)
        {
            if (!IsInside(x, y, z))
                return false;

            cells[x, y, z] = owner;
            return true;
        }

        public bool RemoveCell(Vector3Int position)
        {
            return SetCell(position, Duel3DCellOwner.Empty);
        }

        public int CountCells(Duel3DCellOwner owner)
        {
            int count = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        if (cells[x, y, z] == owner)
                            count++;
                    }
                }
            }

            return count;
        }

        public Duel3DBoardGrid Clone()
        {
            Duel3DBoardGrid clone = new Duel3DBoardGrid(Width, Height, Depth);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        clone.SetCell(x, y, z, cells[x, y, z]);
                    }
                }
            }

            return clone;
        }
    }
}