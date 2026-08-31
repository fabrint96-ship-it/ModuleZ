using UnityEngine;

namespace ModuleZ.Duel3D.AI
{
    public static class Duel3DAIDifficultyScaler
    {
        public static Duel3DAISettings CreateScaledSettings(float progress01)
        {
            progress01 = Mathf.Clamp01(progress01);

            Duel3DAISettings ai =
                ScriptableObject.CreateInstance<Duel3DAISettings>();

            ai.searchDepth = progress01 < 0.55f ? 1 : 2;

            if (progress01 > 0.85f)
                ai.searchDepth = 3;

            ai.randomness = Mathf.Lerp(0.65f, 0.04f, progress01);
            ai.reactionDelay = Mathf.Lerp(1.6f, 0.45f, progress01);

            ai.maxMovesEvaluated =
                Mathf.RoundToInt(Mathf.Lerp(80f, 1200f, progress01));

            ai.clearOwnColorWeight =
                Mathf.Lerp(7f, 18f, progress01);

            ai.reduceOwnCubeCountWeight =
                Mathf.Lerp(4f, 14f, progress01);

            ai.blockPlayerWeight =
                Mathf.Lerp(0.5f, 9f, progress01);

            ai.centerControlWeight =
                Mathf.Lerp(0f, 3f, progress01);

            ai.verticalControlWeight =
                Mathf.Lerp(0f, 4f, progress01);

            ai.dangerPenaltyWeight =
                Mathf.Lerp(2f, 10f, progress01);

            ai.useCenterPreference = progress01 >= 0.25f;
            ai.useHeightPreference = progress01 >= 0.45f;
            ai.allowRiskyMoves = progress01 >= 0.85f;

            ai.showChosenMove = false;
            ai.showMoveEvaluation = false;
            ai.showSearchStats = false;

            return ai;
        }

        public static float GetProgressByDuelWins(int duelsWon)
        {
            return Mathf.Clamp01(duelsWon / 20f);
        }

        public static float GetProgressByRivalsDefeated(
            bool madridDefeated,
            bool barcelonaDefeated,
            bool valenciaDefeated,
            bool andaluciaDefeated)
        {
            int defeated = 0;

            if (madridDefeated)
                defeated++;

            if (barcelonaDefeated)
                defeated++;

            if (valenciaDefeated)
                defeated++;

            if (andaluciaDefeated)
                defeated++;

            return Mathf.Clamp01(defeated / 4f);
        }

        public static float CombineProgress(
            float rivalProgress,
            float duelWinProgress,
            float rematchProgress)
        {
            return Mathf.Clamp01(
                rivalProgress * 0.55f +
                duelWinProgress * 0.25f +
                rematchProgress * 0.20f
            );
        }

        public static float GetProgressByRematchesWon(int rematchesWon)
        {
            return Mathf.Clamp01(rematchesWon / 30f);
        }
    }
}