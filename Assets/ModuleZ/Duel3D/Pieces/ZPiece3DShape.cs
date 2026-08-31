using UnityEngine;

namespace ModuleZ.Duel3D.Pieces
{
    public static class ZPiece3DShape
    {
        private static readonly Vector3Int[] BaseShape =
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(2, 0, 1)
        };

        public static Vector3Int[] GetCells(
            Vector3Int origin,
            ZPiece3DRotationState rotation)
        {
            Vector3Int[] rotatedShape = GetRotatedShape(rotation);
            Vector3Int[] result = new Vector3Int[rotatedShape.Length];

            for (int i = 0; i < rotatedShape.Length; i++)
                result[i] = origin + rotatedShape[i];

            return result;
        }

        public static Vector3Int[] GetCells(
            Vector3Int origin,
            int yaw,
            int pitch,
            int roll)
        {
            Vector3Int[] rotatedShape = GetRotatedShape(yaw, pitch, roll);
            Vector3Int[] result = new Vector3Int[rotatedShape.Length];

            for (int i = 0; i < rotatedShape.Length; i++)
                result[i] = origin + rotatedShape[i];

            return result;
        }

        public static Vector3Int[] GetCells(
            Vector3Int origin,
            int rotationIndex)
        {
            Vector3Int[] rotatedShape = GetRotatedShape(rotationIndex);
            Vector3Int[] result = new Vector3Int[rotatedShape.Length];

            for (int i = 0; i < rotatedShape.Length; i++)
                result[i] = origin + rotatedShape[i];

            return result;
        }

        public static Vector3Int[] GetRotatedShape(
            ZPiece3DRotationState rotation)
        {
            if (rotation == null)
                return GetRotatedShape(0, 0, 0);

            return GetRotatedShape(
                rotation.yaw,
                rotation.pitch,
                rotation.roll
            );
        }

        public static Vector3Int[] GetRotatedShape(
            int yaw,
            int pitch,
            int roll)
        {
            yaw = NormalizeQuarterTurns(yaw);
            pitch = NormalizeQuarterTurns(pitch);
            roll = NormalizeQuarterTurns(roll);

            Vector3Int[] result = new Vector3Int[BaseShape.Length];

            for (int i = 0; i < BaseShape.Length; i++)
            {
                Vector3Int p = BaseShape[i];

                for (int y = 0; y < yaw; y++)
                    p = RotateAroundY(p);

                for (int x = 0; x < pitch; x++)
                    p = RotateAroundX(p);

                for (int z = 0; z < roll; z++)
                    p = RotateAroundZ(p);

                result[i] = p;
            }

            return NormalizeShape(result);
        }

        public static Vector3Int[] GetRotatedShape(int rotationIndex)
        {
            int index = Mathf.Abs(rotationIndex);

            int yaw = index % 4;
            int pitch = (index / 4) % 4;
            int roll = (index / 16) % 4;

            return GetRotatedShape(yaw, pitch, roll);
        }

        public static int GetRotationCount()
        {
            return 64;
        }

        private static Vector3Int RotateAroundY(Vector3Int p)
        {
            return new Vector3Int(p.z, p.y, -p.x);
        }

        private static Vector3Int RotateAroundX(Vector3Int p)
        {
            return new Vector3Int(p.x, -p.z, p.y);
        }

        private static Vector3Int RotateAroundZ(Vector3Int p)
        {
            return new Vector3Int(-p.y, p.x, p.z);
        }

        private static Vector3Int[] NormalizeShape(Vector3Int[] shape)
        {
            int minX = shape[0].x;
            int minY = shape[0].y;
            int minZ = shape[0].z;

            for (int i = 1; i < shape.Length; i++)
            {
                minX = Mathf.Min(minX, shape[i].x);
                minY = Mathf.Min(minY, shape[i].y);
                minZ = Mathf.Min(minZ, shape[i].z);
            }

            Vector3Int offset = new Vector3Int(minX, minY, minZ);
            Vector3Int[] normalized = new Vector3Int[shape.Length];

            for (int i = 0; i < shape.Length; i++)
                normalized[i] = shape[i] - offset;

            SortShape(normalized);

            return normalized;
        }

        private static int NormalizeQuarterTurns(int value)
        {
            value %= 4;

            if (value < 0)
                value += 4;

            return value;
        }

        private static void SortShape(Vector3Int[] shape)
        {
            System.Array.Sort(shape, (a, b) =>
            {
                if (a.x != b.x)
                    return a.x.CompareTo(b.x);

                if (a.y != b.y)
                    return a.y.CompareTo(b.y);

                return a.z.CompareTo(b.z);
            });
        }
    }
}