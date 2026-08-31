using UnityEngine;

namespace ModuleZ.Duel3D.AI
{
    [CreateAssetMenu(
        fileName = "Duel3DAISettings",
        menuName = "ModuleZ/Duel3D/AI Settings"
    )]
    public class Duel3DAISettings : ScriptableObject
    {
        [Header("Difficulty")]
        [Range(1, 5)]
        public int searchDepth = 1;

        [Range(0f, 1f)]
        public float randomness = 0.25f;

        [Range(0.1f, 5f)]
        public float reactionDelay = 1f;

        [Header("Search Limits")]
        [Range(10, 5000)]
        public int maxMovesEvaluated = 300;

        [Header("Scoring")]
        public float clearOwnColorWeight = 12f;

        public float reduceOwnCubeCountWeight = 8f;

        public float blockPlayerWeight = 5f;

        public float centerControlWeight = 1f;

        public float verticalControlWeight = 2f;

        public float dangerPenaltyWeight = 6f;

        [Header("Advanced")]
        public bool useCenterPreference = true;

        public bool useHeightPreference = true;

        public bool allowRiskyMoves = false;

        [Header("Debug")]
        public bool showChosenMove = false;

        public bool showMoveEvaluation = false;

        public bool showSearchStats = false;

        public static Duel3DAISettings CreateMadridAI()
        {
            Duel3DAISettings ai =
                ScriptableObject.CreateInstance<Duel3DAISettings>();

            ai.searchDepth = 1;
            ai.randomness = 0.60f;
            ai.reactionDelay = 1.5f;
            ai.maxMovesEvaluated = 80;

            ai.clearOwnColorWeight = 8f;
            ai.reduceOwnCubeCountWeight = 5f;
            ai.blockPlayerWeight = 1f;

            ai.centerControlWeight = 0f;
            ai.verticalControlWeight = 0f;

            return ai;
        }

        public static Duel3DAISettings CreateBarcelonaAI()
        {
            Duel3DAISettings ai =
                ScriptableObject.CreateInstance<Duel3DAISettings>();

            ai.searchDepth = 1;
            ai.randomness = 0.35f;
            ai.reactionDelay = 1.2f;
            ai.maxMovesEvaluated = 150;

            ai.clearOwnColorWeight = 10f;
            ai.reduceOwnCubeCountWeight = 7f;
            ai.blockPlayerWeight = 3f;

            ai.centerControlWeight = 1f;
            ai.verticalControlWeight = 1f;

            return ai;
        }

        public static Duel3DAISettings CreateValenciaAI()
        {
            Duel3DAISettings ai =
                ScriptableObject.CreateInstance<Duel3DAISettings>();

            ai.searchDepth = 2;
            ai.randomness = 0.20f;
            ai.reactionDelay = 1.0f;
            ai.maxMovesEvaluated = 300;

            ai.clearOwnColorWeight = 12f;
            ai.reduceOwnCubeCountWeight = 8f;
            ai.blockPlayerWeight = 5f;

            ai.centerControlWeight = 2f;
            ai.verticalControlWeight = 3f;

            return ai;
        }

        public static Duel3DAISettings CreateAndaluciaAI()
        {
            Duel3DAISettings ai =
                ScriptableObject.CreateInstance<Duel3DAISettings>();

            ai.searchDepth = 3;
            ai.randomness = 0.05f;
            ai.reactionDelay = 0.6f;
            ai.maxMovesEvaluated = 1000;

            ai.clearOwnColorWeight = 16f;
            ai.reduceOwnCubeCountWeight = 12f;
            ai.blockPlayerWeight = 8f;

            ai.centerControlWeight = 3f;
            ai.verticalControlWeight = 4f;

            ai.allowRiskyMoves = true;

            return ai;
        }
    }
}