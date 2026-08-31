using ModuleZ.Duel3D.Board;
using ModuleZ.Duel3D.Core;
using UnityEngine;

namespace ModuleZ.Duel3D.Rules
{
    public enum Duel3DMatchResult
    {
        None,
        PlayerWin,
        OpponentWin,
        Draw
    }

    public class Duel3DMatchResolver : MonoBehaviour
    {
        [SerializeField]
        private float matchDurationSeconds = Duel3DGameRules.MatchDurationSeconds;

        private float remainingTime;
        private bool matchFinished;
        private bool playerHasPlayed;
        private bool opponentHasPlayed;

        private Duel3DBoardGrid board;

        public float RemainingTime => remainingTime;
        public bool MatchFinished => matchFinished;

        public System.Action<float> OnTimeChanged;
        public System.Action<Duel3DMatchResult> OnMatchFinished;

        public void Initialize(Duel3DBoardGrid duelBoard)
        {
            board = duelBoard;
            remainingTime = matchDurationSeconds;
            matchFinished = false;
            playerHasPlayed = false;
            opponentHasPlayed = false;
        }

        private void Update()
        {
            if (matchFinished)
                return;

            remainingTime -= Time.deltaTime;

            if (remainingTime < 0f)
                remainingTime = 0f;

            OnTimeChanged?.Invoke(remainingTime);

            if (remainingTime <= 0f)
                FinishMatch(EvaluateTimeExpiredWinner());
        }

        public void RegisterPiecePlaced(Duel3DCellOwner owner)
        {
            if (owner == Duel3DCellOwner.Player)
                playerHasPlayed = true;

            if (owner == Duel3DCellOwner.Opponent)
                opponentHasPlayed = true;
        }

        public void EvaluateImmediateVictory()
        {
            if (matchFinished || board == null)
                return;

            int playerCells = board.CountCells(Duel3DCellOwner.Player);
            int opponentCells = board.CountCells(Duel3DCellOwner.Opponent);

            if (playerCells <= 0 && opponentCells <= 0)
            {
                FinishMatch(Duel3DMatchResult.Draw);
                return;
            }

            if (playerCells <= 0)
            {
                FinishMatch(Duel3DMatchResult.PlayerWin);
                return;
            }

            if (opponentCells <= 0)
            {
                FinishMatch(Duel3DMatchResult.OpponentWin);
            }
        }

        public Duel3DMatchResult EvaluateTimeExpiredWinner()
        {
            if (board == null)
                return Duel3DMatchResult.Draw;

            int playerCells = board.CountCells(Duel3DCellOwner.Player);
            int opponentCells = board.CountCells(Duel3DCellOwner.Opponent);

            if (playerCells < opponentCells)
                return Duel3DMatchResult.PlayerWin;

            if (opponentCells < playerCells)
                return Duel3DMatchResult.OpponentWin;

            return Duel3DMatchResult.Draw;
        }

        public void FinishMatch(Duel3DMatchResult result)
        {
            if (matchFinished)
                return;

            matchFinished = true;
            Debug.Log("[ModuleZ] Duel3D Finished -> " + result);
            OnMatchFinished?.Invoke(result);
        }

        public int GetPlayerCubeCount()
        {
            return board == null ? 0 : board.CountCells(Duel3DCellOwner.Player);
        }

        public int GetOpponentCubeCount()
        {
            return board == null ? 0 : board.CountCells(Duel3DCellOwner.Opponent);
        }
    }
}