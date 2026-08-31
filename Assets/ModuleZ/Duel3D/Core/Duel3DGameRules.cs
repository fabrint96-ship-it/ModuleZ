namespace ModuleZ.Duel3D.Core
{
    public static class Duel3DGameRules
    {
        // =====================================================
        // TABLERO OFICIAL
        // =====================================================

        public const int BoardWidth = 8;
        public const int BoardHeight = 5;
        public const int BoardDepth = 8;

        // =====================================================
        // PIEZA Z
        // =====================================================

        public const int CubesPerPiece = 4;

        // =====================================================
        // ELIMINACIÓN
        // =====================================================

        public const int ExactLineSizeToRemove = 3;

        // =====================================================
        // PARTIDA
        // =====================================================

        public const float MatchDurationSeconds = 180f;

        // =====================================================
        // CÁMARA
        // =====================================================

        public const float CameraPosX = 0f;
        public const float CameraPosY = 5.5f;
        public const float CameraPosZ = -7.5f;

        public const float CameraRotX = 38f;
        public const float CameraRotY = 0f;
        public const float CameraRotZ = 0f;

        public const float CameraFieldOfView = 55f;

        // =====================================================
        // JUGABILIDAD
        // =====================================================

        public const bool UseGravity = false;

        public const bool AllowFloatingPieces = true;

        public const bool AllowDiagonalConnections = false;

        public const bool AllowPieceOverlap = false;

        // =====================================================
        // IA
        // =====================================================

        public const int MadridDifficulty = 1;
        public const int BarcelonaDifficulty = 2;
        public const int ValenciaDifficulty = 3;
        public const int AndaluciaDifficulty = 4;

        // =====================================================
        // VICTORIA
        // =====================================================

        public static bool IsImmediateVictory(int cubeCount)
        {
            return cubeCount <= 0;
        }

        public static bool ShouldRemoveLine(int lineSize)
        {
            return lineSize == ExactLineSizeToRemove;
        }

        public static bool PlayerWinsByTime(
            int playerCubes,
            int opponentCubes)
        {
            return playerCubes < opponentCubes;
        }

        public static bool OpponentWinsByTime(
            int playerCubes,
            int opponentCubes)
        {
            return opponentCubes < playerCubes;
        }

        public static bool IsDraw(
            int playerCubes,
            int opponentCubes)
        {
            return playerCubes == opponentCubes;
        }
    }
}