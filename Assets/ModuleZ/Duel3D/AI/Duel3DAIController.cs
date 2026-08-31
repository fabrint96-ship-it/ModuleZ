using System.Collections.Generic;
using ModuleZ.Duel3D.Board;
using ModuleZ.Duel3D.Pieces;
using ModuleZ.Duel3D.Rules;
using UnityEngine;

namespace ModuleZ.Duel3D.AI
{
    public class Duel3DAIController
    {
        private struct AIMove
        {
            public Vector3Int origin;
            public int rotationIndex;
            public float score;
        }

        private readonly Duel3DAISettings settings;
        private readonly Duel3DRivalProfile profile;

        public Duel3DAIController(Duel3DAISettings aiSettings)
        {
            settings = aiSettings;
        }

        public Duel3DAIController(
            Duel3DAISettings aiSettings,
            Duel3DRivalProfile rivalProfile)
        {
            settings = aiSettings;
            profile = rivalProfile;
        }

        public bool TryFindBestMove(
            Duel3DBoardGrid board,
            Vector3Int[] forbiddenCells,
            out Vector3Int bestOrigin,
            out int bestRotation)
        {
            bestOrigin = Vector3Int.zero;
            bestRotation = 0;

            if (board == null)
                return false;

            Duel3DAISettings activeSettings =
                settings != null ? settings : Duel3DAISettings.CreateMadridAI();

            List<AIMove> moves = GenerateValidMoves(
                board,
                forbiddenCells,
                activeSettings
            );

            if (moves.Count == 0)
                return false;

            AIMove bestMove = moves[0];

            for (int i = 1; i < moves.Count; i++)
            {
                if (moves[i].score > bestMove.score)
                    bestMove = moves[i];
            }

            if (activeSettings.randomness > 0f)
                bestMove = ApplyRandomness(moves, bestMove, activeSettings.randomness);

            bestOrigin = bestMove.origin;
            bestRotation = bestMove.rotationIndex;

            if (activeSettings.showChosenMove)
            {
                Debug.Log(
                    $"[ModuleZ] IA Duel3D elige {bestOrigin} rot={bestRotation} score={bestMove.score}"
                );
            }

            return true;
        }

        private List<AIMove> GenerateValidMoves(
            Duel3DBoardGrid board,
            Vector3Int[] forbiddenCells,
            Duel3DAISettings activeSettings)
        {
            List<AIMove> moves = new List<AIMove>();

            int rotationCount = ZPiece3DShape.GetRotationCount();
            int evaluated = 0;

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    for (int z = 0; z < board.Depth; z++)
                    {
                        Vector3Int origin = new Vector3Int(x, y, z);

                        for (int r = 0; r < rotationCount; r++)
                        {
                            if (!Duel3DPiecePlacement.CanPlacePiece(board, origin, r))
                                continue;

                            Vector3Int[] candidateCells =
                                Duel3DPiecePlacement.GetPreviewCells(origin, r);

                            if (Duel3DPiecePlacement.TouchesForbiddenCells(
                                board,
                                candidateCells,
                                forbiddenCells))
                            {
                                continue;
                            }

                            float score = EvaluateMove(board, origin, r, activeSettings);

                            moves.Add(new AIMove
                            {
                                origin = origin,
                                rotationIndex = r,
                                score = score
                            });

                            evaluated++;

                            if (evaluated >= activeSettings.maxMovesEvaluated)
                                return moves;
                        }
                    }
                }
            }

            return moves;
        }

        private float EvaluateMove(
            Duel3DBoardGrid board,
            Vector3Int origin,
            int rotationIndex,
            Duel3DAISettings activeSettings)
        {
            float aggressiveness = profile != null ? profile.aggressiveness : 0.5f;
            float defensiveBias = profile != null ? profile.defensiveBias : 0.5f;
            float blockChance = profile != null ? profile.blockChance : 0.5f;

            int opponentBefore = board.CountCells(Duel3DCellOwner.Opponent);
            int playerBefore = board.CountCells(Duel3DCellOwner.Player);

            Duel3DBoardGrid simulated = board.Clone();

            Duel3DPiecePlacement.PlacePiece(
                simulated,
                origin,
                rotationIndex,
                Duel3DCellOwner.Opponent
            );

            int removableOpponent =
                Duel3DGroupResolver.CountResolvableCells(
                    simulated,
                    Duel3DCellOwner.Opponent
                );

            int removablePlayer =
                Duel3DGroupResolver.CountResolvableCells(
                    simulated,
                    Duel3DCellOwner.Player
                );

            Duel3DGroupResolver.ResolveGroups(simulated);

            int opponentAfter = simulated.CountCells(Duel3DCellOwner.Opponent);
            int playerAfter = simulated.CountCells(Duel3DCellOwner.Player);

            float score = 0f;

            score += removableOpponent *
                     activeSettings.clearOwnColorWeight *
                     Mathf.Lerp(0.7f, 1.6f, aggressiveness);

            score += (opponentBefore - opponentAfter) *
                     activeSettings.reduceOwnCubeCountWeight *
                     Mathf.Lerp(0.7f, 1.5f, aggressiveness);

            score += removablePlayer *
                     activeSettings.blockPlayerWeight *
                     Mathf.Lerp(0.4f, 1.5f, blockChance);

            score += (playerAfter - playerBefore) *
                     activeSettings.blockPlayerWeight *
                     Mathf.Lerp(0.4f, 1.4f, defensiveBias);

            if (activeSettings.useCenterPreference)
            {
                score += EvaluateCenterControl(board, origin, rotationIndex) *
                         activeSettings.centerControlWeight *
                         Mathf.Lerp(0.8f, 1.4f, aggressiveness);
            }

            if (activeSettings.useHeightPreference)
            {
                score += EvaluateHeightControl(origin, rotationIndex) *
                         activeSettings.verticalControlWeight *
                         Mathf.Lerp(0.6f, 1.3f, aggressiveness);
            }

            if (!activeSettings.allowRiskyMoves)
            {
                int opponentDanger =
                    Duel3DGroupResolver.CountResolvableCells(
                        simulated,
                        Duel3DCellOwner.Opponent
                    );

                score -= opponentDanger *
                         activeSettings.dangerPenaltyWeight *
                         Mathf.Lerp(0.4f, 1.5f, defensiveBias);
            }

            score += Random.Range(
                -activeSettings.randomness,
                activeSettings.randomness
            ) * 10f;

            return score;
        }

        private AIMove ApplyRandomness(
            List<AIMove> moves,
            AIMove bestMove,
            float randomness)
        {
            if (moves.Count <= 1)
                return bestMove;

            if (Random.value > randomness)
                return bestMove;

            int randomIndex = Random.Range(0, moves.Count);
            return moves[randomIndex];
        }

        private float EvaluateCenterControl(
            Duel3DBoardGrid board,
            Vector3Int origin,
            int rotationIndex)
        {
            Vector3 center = new Vector3(
                (board.Width - 1) * 0.5f,
                (board.Height - 1) * 0.5f,
                (board.Depth - 1) * 0.5f
            );

            Vector3Int[] cells =
                ZPiece3DShape.GetCells(origin, rotationIndex);

            float score = 0f;

            for (int i = 0; i < cells.Length; i++)
            {
                float distance = Vector3.Distance(cells[i], center);
                score += 1f / (1f + distance);
            }

            return score;
        }

        private float EvaluateHeightControl(
            Vector3Int origin,
            int rotationIndex)
        {
            Vector3Int[] cells =
                ZPiece3DShape.GetCells(origin, rotationIndex);

            float score = 0f;

            for (int i = 0; i < cells.Length; i++)
                score += cells[i].y;

            return score / cells.Length;
        }
    }
}