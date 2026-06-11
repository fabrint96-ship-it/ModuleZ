using ModuleZ.Core.Managers;
using ModuleZ.Game.Animation;
using ModuleZ.OpenWorld.Encounters;
using ModuleZ.Game.NPCs;
using ModuleZ.UI.HUD;
using UnityEngine;

namespace ModuleZ.Game.Interaction
{
    public class ModuleZProgressDialogueInteractable : ModuleZInteractable
    {
        public string speakerName = "NPC";
        public ModuleZRivalId relatedRival = ModuleZRivalId.Madrid;

        [TextArea] public string beforeDefeatLine;
        [TextArea] public string afterDefeatLine;

        [SerializeField] private float dialogueCooldown = 0.45f;

        private float nextDialogueTime;

        private void Awake()
        {
            interactionText = "Pulsa E para hablar";
        }

        public override void Interact()
        {
            if (Time.time < nextDialogueTime)
                return;

            nextDialogueTime = Time.time + dialogueCooldown;

            ModuleZNPCPatrolController patrol = GetComponent<ModuleZNPCPatrolController>();
            if (patrol != null)
                patrol.SetPaused(true);

            ModuleZTalkAnimation talkAnimation = GetComponent<ModuleZTalkAnimation>();
            if (talkAnimation != null)
                talkAnimation.PlayTalkAnimation();

            string line = IsRelatedRivalDefeated()
                ? afterDefeatLine
                : beforeDefeatLine;

            string finalLine = speakerName + ": " + line;

            Debug.Log("[Module Z Progress Dialogue] " + finalLine);

            if (OpenWorldMessageHUD.Instance != null)
                OpenWorldMessageHUD.Instance.ShowDialogue(finalLine, 3f);

            Invoke(nameof(ResumePatrol), 3f);
        }

        private bool IsRelatedRivalDefeated()
        {
            switch (relatedRival)
            {
                case ModuleZRivalId.Madrid:
                    return ModuleZGameState.RivalMadridDefeated;

                case ModuleZRivalId.Barcelona:
                    return ModuleZGameState.RivalBarcelonaDefeated;

                case ModuleZRivalId.Valencia:
                    return ModuleZGameState.RivalValenciaDefeated;

                case ModuleZRivalId.Andalucia:
                    return ModuleZGameState.RivalAndaluciaDefeated;

                default:
                    return false;
            }
        }

        private void ResumePatrol()
        {
            ModuleZNPCPatrolController patrol = GetComponent<ModuleZNPCPatrolController>();

            if (patrol != null)
                patrol.SetPaused(false);
        }
    }
}