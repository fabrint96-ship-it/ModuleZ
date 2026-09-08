using ModuleZ.Core.Managers;
using ModuleZ.Game.Animation;
using ModuleZ.Game.DuelTransition;
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
        private DuelContext pendingDuelContext;
        private bool hasPendingDuelContext;

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

            Vector3 returnPosition =
                transform.position + new Vector3(0f, 0f, -3f);

            pendingDuelContext = new DuelContext(
                rivalId,
                isRematch,
                returnPosition
            );

            if (!DuelTransitionBridge.Begin(
                    pendingDuelContext,
                    out string failureReason))
            {
                duelStarting = false;
                Debug.LogError(
                    "[ModuleZ] No se pudo iniciar el duelo: " +
                    failureReason
                );
                return;
            }

            hasPendingDuelContext = true;

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
            if (!hasPendingDuelContext)
            {
                Debug.LogError("[ModuleZ] No existe DuelContext pendiente.");
                return;
            }

            DuelTransitionBridge.CompleteAfterDelay(pendingDuelContext);
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
