using UnityEngine;

namespace ModuleZ.Duel3D.Core
{
    [CreateAssetMenu(
        fileName = "Duel3DMatchConfig",
        menuName = "ModuleZ/Duel3D/Match Config")]
    public class Duel3DMatchConfig : ScriptableObject
    {
        [Header("Board")]
        public int boardWidth = 8;
        public int boardHeight = 6;
        public int boardDepth = 8;

        [Header("Match")]
        public float matchDurationSeconds = 180f;

        [Header("Rules")]
        public int cubesToRemovePerLine = 5;
        public bool allowDiagonalMatches = false;
        public bool useLastPieceRestriction = true;

        [Header("Camera")]
        public bool useOrbitCamera = true;
        public float cameraDistance = 8f;
        public float cameraMinDistance = 4f;
        public float cameraMaxDistance = 14f;

        [Header("Difficulty")]
        [Range(0f, 1f)]
        public float aiProgress01 = 0f;

        public bool overrideAIScaling = false;

        [Header("Visual")]
        public bool showAIDebug = true;
        public bool showControls = true;
        public bool showForbiddenCells = true;

        [Header("Theme")]
        public Color playerColor =
            new Color(0.1f, 0.9f, 0.25f);

        public Color opponentColor =
            new Color(0.9f, 0.12f, 0.1f);

        public Color boardColor =
            new Color(0.12f, 0.12f, 0.14f);

        public Color forbiddenColor =
            new Color(1f, 0.65f, 0.05f, 0.25f);

        public Color boundsColor =
            new Color(0.45f, 0.55f, 0.65f, 0.55f);

        public static Duel3DMatchConfig CreateDefault()
        {
            Duel3DMatchConfig config =
                ScriptableObject.CreateInstance<Duel3DMatchConfig>();

            return config;
        }

        public static Duel3DMatchConfig CreateEasy()
        {
            Duel3DMatchConfig config = CreateDefault();

            config.aiProgress01 = 0.15f;
            config.matchDurationSeconds = 240f;

            return config;
        }

        public static Duel3DMatchConfig CreateNormal()
        {
            Duel3DMatchConfig config = CreateDefault();

            config.aiProgress01 = 0.50f;
            config.matchDurationSeconds = 180f;

            return config;
        }

        public static Duel3DMatchConfig CreateHard()
        {
            Duel3DMatchConfig config = CreateDefault();

            config.aiProgress01 = 0.85f;
            config.matchDurationSeconds = 150f;

            return config;
        }
    }
}