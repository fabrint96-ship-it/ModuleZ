using ModuleZ.Game.Animation;
using ModuleZ.UI.HUD;
using ModuleZ.Game.NPCs;
using UnityEngine;

namespace ModuleZ.Game.Interaction
{
    public class ModuleZDialogueInteractable : ModuleZInteractable
    {
        public string speakerName = "NPC";

        [TextArea]
        public string[] dialogueLines;

        [SerializeField] private float dialogueCooldown = 0.45f;

        private int currentLineIndex;
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

            if (dialogueLines == null || dialogueLines.Length == 0)
            {
                ShowLine("...");
                return;
            }

            ShowLine(dialogueLines[currentLineIndex]);

            Invoke(nameof(ResumePatrol), 3f);

            currentLineIndex++;

            if (currentLineIndex >= dialogueLines.Length)
                currentLineIndex = 0;
        }

        private void ResumePatrol()
        {
            ModuleZNPCPatrolController patrol = GetComponent<ModuleZNPCPatrolController>();
            if (patrol != null)
                patrol.SetPaused(false);
        }

        private void ShowLine(string line)
        {
            string finalLine = speakerName + ": " + line;

            Debug.Log("[Module Z Dialogue] " + finalLine);

            if (OpenWorldMessageHUD.Instance != null)
                OpenWorldMessageHUD.Instance.ShowDialogue(finalLine, 3f);
        }
    }
}