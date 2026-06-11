using ModuleZ.Core.Managers;
using ModuleZ.Game.Animation;
using ModuleZ.UI.HUD;
using ModuleZ.OpenWorld.Runtime;
using UnityEngine;

namespace ModuleZ.Game.Interaction
{
    public class ModuleZGuideInteractable : ModuleZInteractable
    {
        [SerializeField] private float guideCooldown = 0.45f;

        private int currentLineIndex;
        private float nextGuideTime;

        private void Awake()
        {
            interactionText = "Pulsa E para guía";
        }

        public override void Interact()
        {
            if (Time.time < nextGuideTime)
                return;

            nextGuideTime = Time.time + guideCooldown;

            ModuleZTalkAnimation talkAnimation = GetComponent<ModuleZTalkAnimation>();
            if (talkAnimation != null)
                talkAnimation.PlayTalkAnimation();

            string[] lines = GetGuideLines();
            string message = "Guía: " + lines[currentLineIndex];

            currentLineIndex++;

            if (currentLineIndex >= lines.Length)
                currentLineIndex = 0;

            Debug.Log("[Module Z Guide] " + message);

            if (OpenWorldMessageHUD.Instance != null)
                OpenWorldMessageHUD.Instance.ShowDialogue(message, 3f);
        }

        private string[] GetGuideLines()
        {
            return new string[]
            {
                "Te encuentras actualmente en " + GetCurrentZoneName() + ".",
                "Module Z consiste en resolver duelos usando la lógica de la pieza Z.",
                "Has ganado " + ModuleZGameState.DuelsWon + " duelos.",
                "Rivales pendientes: " + GetPendingRivalsCount(),
                GetAndaluciaHint()
            };
        }

        private string GetAndaluciaHint()
        {
            if (ModuleZGameState.AndaluciaUnlocked)
                return "Andalucía ya está desbloqueada.";

            return "Derrota a Madrid, Barcelona y Valencia para desbloquear Andalucía.";
        }

        private int GetPendingRivalsCount()
        {
            int pending = 0;

            if (!ModuleZGameState.RivalMadridDefeated)
                pending++;

            if (!ModuleZGameState.RivalBarcelonaDefeated)
                pending++;

            if (!ModuleZGameState.RivalValenciaDefeated)
                pending++;

            if (ModuleZGameState.AndaluciaUnlocked && !ModuleZGameState.RivalAndaluciaDefeated)
                pending++;

            return pending;
        }

        private string GetCurrentZoneName()
        {
            switch (ModuleZGameState.CurrentOpenWorldTheme)
            {
                case OpenWorldThemeId.Madrid70s:
                    return "Madrid";

                case OpenWorldThemeId.Barcelona70s:
                    return "Barcelona";

                case OpenWorldThemeId.Valencia70s:
                    return "Valencia";

                case OpenWorldThemeId.Andalucia70s:
                    return "Andalucía";

                default:
                    return "Desconocida";
            }
        }
    }
}