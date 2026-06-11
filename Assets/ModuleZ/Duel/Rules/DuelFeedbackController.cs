using ModuleZ.Core.Managers;
using ModuleZ.Duel.Builders;
using ModuleZ.Duel.Runtime;
using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelFeedbackController : MonoBehaviour
    {
        public static DuelFeedbackController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void PlayVictory()
        {
            DuelZPieceController zPiece = FindObjectOfType<DuelZPieceController>();
            if (zPiece == null)
                return;

            DuelThemeData themeData =
                DuelThemeDatabase.GetThemeData(ModuleZGameState.CurrentDuelTheme);

            DuelVictoryParticles particles = gameObject.AddComponent<DuelVictoryParticles>();
            particles.Play(zPiece.transform.position, themeData.accentColor);
        }

        public void PlayDefeat()
        {
            DuelZPieceController zPiece = FindObjectOfType<DuelZPieceController>();
            if (zPiece == null)
                return;

            DuelThemeData themeData =
                DuelThemeDatabase.GetThemeData(ModuleZGameState.CurrentDuelTheme);

            Color defeatColor = Color.Lerp(themeData.secondaryColor, Color.black, 0.45f);

            DuelDefeatParticles particles = gameObject.AddComponent<DuelDefeatParticles>();
            particles.Play(zPiece.transform.position, defeatColor);
        }

        public void PlayAbandon()
        {
            DuelZPieceController zPiece = FindObjectOfType<DuelZPieceController>();
            if (zPiece == null)
                return;

            DuelThemeData themeData =
                DuelThemeDatabase.GetThemeData(ModuleZGameState.CurrentDuelTheme);

            Color abandonColor = Color.Lerp(themeData.primaryColor, Color.gray, 0.35f);

            DuelAbandonParticles particles = gameObject.AddComponent<DuelAbandonParticles>();
            particles.Play(zPiece.transform.position, abandonColor);
        }
    }
}