using ModuleZ.Duel.Builders;
using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelPuzzleGoal : MonoBehaviour
    {
        private bool completed;

        private void OnTriggerEnter(Collider other)
        {
            TryComplete(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TryComplete(other);
        }

        private void TryComplete(Collider other)
        {
            if (completed)
                return;

            Transform root = other.transform.root;

            if (!root.CompareTag("ZPiece"))
                return;

            completed = true;

            DuelZPieceController controller = root.GetComponent<DuelZPieceController>();
            if (controller != null)
                controller.LockPiece();

            DuelVictoryEffect effect = root.GetComponent<DuelVictoryEffect>();
            if (effect != null)
                effect.Play();

            if (DuelHUDController.Instance != null)
                DuelHUDController.Instance.ShowMessage("Puzzle Z completado");

            if (DuelResultManager.Instance != null)
                DuelResultManager.Instance.WinDuel();
        }
    }
}