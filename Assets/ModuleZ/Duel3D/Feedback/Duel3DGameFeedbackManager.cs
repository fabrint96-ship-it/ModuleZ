using ModuleZ.Duel3D.Audio;
using ModuleZ.Duel3D.Rules;
using ModuleZ.Duel3D.UI;
using ModuleZ.Duel3D.Visuals;
using UnityEngine;

namespace ModuleZ.Duel3D.Feedback
{
    public class Duel3DGameFeedbackManager : MonoBehaviour
    {
        private Duel3DFeedbackController visualFeedback;
        private Duel3DAudioFeedbackController audioFeedback;
        private Duel3DResultVisualController resultVisual;
        private Duel3DHUDController hud;

        public void Initialize(
            Duel3DHUDController hudController,
            Duel3DResultVisualController resultVisualController)
        {
            hud = hudController;
            resultVisual = resultVisualController;

            visualFeedback = gameObject.AddComponent<Duel3DFeedbackController>();
            audioFeedback = gameObject.AddComponent<Duel3DAudioFeedbackController>();
        }

        public void PlayPlace()
        {
            visualFeedback?.PlayPlaceFeedback();
            audioFeedback?.PlayPlace();
        }

        public void PlayRemove()
        {
            visualFeedback?.PlayRemoveFeedback();
            audioFeedback?.PlayRemove();

            hud?.ShowActionMessage("Cubos eliminados", 1.2f);
        }

        public void PlayInvalid(string message = "Movimiento inválido")
        {
            visualFeedback?.PlayInvalidMoveFeedback();
            audioFeedback?.PlayInvalid();

            hud?.ShowActionMessage(message, 1.5f);
        }

        public void PlayTurnChanged(bool playerTurn)
        {
            visualFeedback?.PlayTurnChangedFeedback();
            audioFeedback?.PlayTurn();

            hud?.ShowActionMessage(
                playerTurn ? "Turno del Player" : "Turno del Oponente",
                0.9f
            );
        }

        public void PlayResult(
            Duel3DMatchResult result,
            Transform cubesRoot)
        {
            resultVisual?.PlayResult(result, cubesRoot);

            switch (result)
            {
                case Duel3DMatchResult.PlayerWin:
                    audioFeedback?.PlayVictory();
                    hud?.ShowActionMessage("Victoria", 4f);
                    break;

                case Duel3DMatchResult.OpponentWin:
                    audioFeedback?.PlayDefeat();
                    hud?.ShowActionMessage("Derrota", 4f);
                    break;

                case Duel3DMatchResult.Draw:
                    audioFeedback?.PlayDefeat();
                    hud?.ShowActionMessage("Empate", 4f);
                    break;
            }
        }
    }
}