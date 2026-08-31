using UnityEngine;

namespace ModuleZ.Duel3D.Pieces
{
    [System.Serializable]
    public class ZPiece3DRotationState
    {
        [Range(0, 3)] public int yaw;
        [Range(0, 3)] public int pitch;
        [Range(0, 3)] public int roll;

        public void RotateYawPositive()
        {
            yaw = Normalize(yaw + 1);
        }

        public void RotateYawNegative()
        {
            yaw = Normalize(yaw - 1);
        }

        public void RotatePitchPositive()
        {
            pitch = Normalize(pitch + 1);
        }

        public void RotatePitchNegative()
        {
            pitch = Normalize(pitch - 1);
        }

        public void RotateRollPositive()
        {
            roll = Normalize(roll + 1);
        }

        public void RotateRollNegative()
        {
            roll = Normalize(roll - 1);
        }

        public void Reset()
        {
            yaw = 0;
            pitch = 0;
            roll = 0;
        }

        public int ToRotationIndex()
        {
            return yaw + pitch * 4 + roll * 16;
        }

        private int Normalize(int value)
        {
            value %= 4;

            if (value < 0)
                value += 4;

            return value;
        }
    }
}