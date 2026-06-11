using ModuleZ.UI.HUD;
using UnityEngine;
using ModuleZ.Game.Animation;

namespace ModuleZ.Game.Interaction
{
    public class MessageInteractable : ModuleZInteractable
    {
        [TextArea]
        public string message = "Ya hemos combatido.";

        public override void Interact()
        {
            Debug.Log("[Module Z] " + message);

            ModuleZTalkAnimation talkAnimation = GetComponent<ModuleZTalkAnimation>();
            if (talkAnimation != null)
                talkAnimation.PlayTalkAnimation();

            if (OpenWorldMessageHUD.Instance != null)
                OpenWorldMessageHUD.Instance.ShowDialogue(message, 2.5f);
        }
    }
}