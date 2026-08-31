using ModuleZ.Core.Managers;
using ModuleZ.Core.SceneLoading;
using ModuleZ.Game.Animation;
using ModuleZ.Game.Interaction;
using ModuleZ.UI.HUD;
using UnityEngine;

namespace ModuleZ.OpenWorld.Encounters
{
    public class DuelStarterInteractable : ModuleZInteractable
    {
        [Header("Rematch")]
        public bool allowRematchWhenDefeated = true;

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

            if (!ModuleZRivalProgression.IsRivalUnlocked(rivalId))
            {
                ShowMessage(
                    ModuleZRivalProgression.GetLockedMessage(rivalId),
                    2.5f
                );

                PlayTalkAnimation();
                return;
            }

            bool isRematch =
                ModuleZRivalProgression.IsRivalDefeated(rivalId);

            if (isRematch && !allowRematchWhenDefeated)
            {
                ShowMessage(GetDefeatedMessage(), 2f);
                PlayTalkAnimation();
                return;
            }

            duelStarting = true;

            PlayTalkAnimation();

            string message = isRematch
                ? "Rematch contra " + GetRivalDisplayName() + "."
                : GetChallengeMessage();

            Debug.Log("[Module Z] " + message);
            ShowMessage(message, 1.5f);

            ModuleZGameState.PendingDuelRival = rivalId;
            Vector3 returnPosition =
                transform.position + new Vector3(0f, 0f, -3f);

            ModuleZDuelSessionState.StartDuel(
                rivalId,
                isRematch,
                returnPosition
            );

            ModuleZGameState.CurrentDuelRival = rivalId;
            ModuleZGameState.CurrentDuelIsRematch = isRematch;
            ModuleZGameState.OpenWorldReturnPosition = returnPosition;

            ModuleZGameState.ReturningFromDuel = true;
            ModuleZGameState.DuelCompleted = false;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;

            Invoke(nameof(StartDuel), 1.5f);

            Debug.Log(
    "[ModuleZ DEBUG] NPC rivalId = " +
    rivalId
);
        }

        private string GetRivalDisplayName()
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Madrid";

                case ModuleZRivalId.Barcelona:
                    return "Barcelona";

                case ModuleZRivalId.Valencia:
                    return "Valencia";

                case ModuleZRivalId.Andalucia:
                    return "Andalucía";

                default:
                    return "Rival";
            }
        }

        private void StartDuel()
        {
            ModuleZGameState.IsPaused = false;
            ModuleZGameState.DuelCompleted = false;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;

            if (ModuleZSceneController.Instance != null)
                ModuleZSceneController.Instance.LoadDuel();
            else
                Debug.LogError("[Module Z] No existe ModuleZSceneController.");
        }

        private string GetChallengeMessage()
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "En Madrid se gana con cabeza. Te reto.";

                case ModuleZRivalId.Barcelona:
                    return "Barcelona está lista para el siguiente duelo Module Z.";

                case ModuleZRivalId.Valencia:
                    return "Valencia sube la dificultad. Demuéstralo.";

                case ModuleZRivalId.Andalucia:
                    return "Has llegado hasta Andalucía. Este será el gran reto.";

                default:
                    return "Te reto a un duelo Module Z.";
            }
        }

        private string GetDefeatedMessage()
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Ya derrotaste al rival de Madrid.";

                case ModuleZRivalId.Barcelona:
                    return "Ya derrotaste al rival de Barcelona.";

                case ModuleZRivalId.Valencia:
                    return "Ya derrotaste al rival de Valencia.";

                case ModuleZRivalId.Andalucia:
                    return ModuleZGameState.MainProgressionCompleted
                        ? "Has completado la progresión principal de Module Z."
                        : "Ya derrotaste al rival de Andalucía.";

                default:
                    return "Ya has derrotado a este rival.";
            }
        }

        private void PlayTalkAnimation()
        {
            ModuleZTalkAnimation talkAnimation = GetComponent<ModuleZTalkAnimation>();

            if (talkAnimation != null)
                talkAnimation.PlayTalkAnimation();
        }

        private void ShowMessage(string message, float duration)
        {
            Debug.Log("[Module Z] " + message);

            if (OpenWorldMessageHUD.Instance != null)
                OpenWorldMessageHUD.Instance.ShowDialogue(message, duration);
        }
    }
}