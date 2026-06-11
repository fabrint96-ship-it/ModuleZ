using ModuleZ.Core.Managers;
using ModuleZ.Core.SceneLoading;
using ModuleZ.Game.Animation;
using ModuleZ.Game.Interaction;
using ModuleZ.UI.HUD;
using ModuleZ.Duel.Runtime;
using UnityEngine;

namespace ModuleZ.OpenWorld.Encounters
{
    public class DuelStarterInteractable : ModuleZInteractable
    {
        public ModuleZRivalId rivalId = ModuleZRivalId.Madrid;

        private bool duelStarting;

        private void Awake()
        {
            interactionText = "Pulsa E para retar";
        }

        public override void Interact()
        {
            if (duelStarting)
                return;

            duelStarting = true;

            ModuleZTalkAnimation talkAnimation = GetComponent<ModuleZTalkAnimation>();
            if (talkAnimation != null)
                talkAnimation.PlayTalkAnimation();

            string message = GetChallengeMessage();

            Debug.Log("[Module Z] " + message);

            if (OpenWorldMessageHUD.Instance != null)
                OpenWorldMessageHUD.Instance.ShowDialogue(message, 1.5f);

            ModuleZGameState.CurrentDuelRival = rivalId;

            ModuleZGameState.OpenWorldReturnPosition =
                transform.position + new Vector3(0f, 0f, -3f);

            ModuleZGameState.ReturningFromDuel = true;
            ModuleZGameState.CurrentDuelTheme = GetDuelTheme();
            ModuleZGameState.DuelCompleted = false;
            ModuleZGameState.DuelWasCancelled = false;

            Invoke(nameof(StartDuel), 1.5f);
        }

        private string GetChallengeMessage()
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "En Madrid se gana con cabeza. Te reto.";

                case ModuleZRivalId.Barcelona:
                    return "A ver si puedes seguir mi ritmo. Duelo Module Z.";

                case ModuleZRivalId.Valencia:
                    return "Vamos a resolver este puzzle al sol. Te reto.";

                default:
                    return "Te reto a un duelo Module Z.";
            }
        }

        private void StartDuel()
        {
            ModuleZGameState.IsPaused = false;
            ModuleZGameState.DuelCompleted = false;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;

            ModuleZSceneController.Instance.LoadDuel();
        }

        private ModuleZDuelThemeId GetDuelTheme()
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return ModuleZDuelThemeId.Madrid70s;

                case ModuleZRivalId.Barcelona:
                    return ModuleZDuelThemeId.Barcelona70s;

                case ModuleZRivalId.Valencia:
                    return ModuleZDuelThemeId.Valencia70s;

                case ModuleZRivalId.Andalucia:
                    return ModuleZDuelThemeId.Andalucia70s;

                default:
                    return ModuleZDuelThemeId.Madrid70s;
            }
        }
    }
}