using ModuleZ.Game.NPCs;
using ModuleZ.UI.HUD;
using UnityEngine;

namespace ModuleZ.Game.Interaction
{
    public class ModuleZPlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float interactionRadius = 2.5f;

        private ModuleZInteractable currentInteractable;

        private void Update()
        {
            DetectInteractable();

            if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
            {
                ModuleZNPCFacePlayer facePlayer =
                    currentInteractable.GetComponent<ModuleZNPCFacePlayer>();

                if (facePlayer != null)
                    facePlayer.Face(transform);

                currentInteractable.Interact();
            }
        }

        private void DetectInteractable()
        {
            currentInteractable = null;

            ModuleZInteractable[] interactables = FindObjectsOfType<ModuleZInteractable>();

            float closestDistance = interactionRadius;

            foreach (ModuleZInteractable interactable in interactables)
            {
                float distance = Vector3.Distance(
                    transform.position,
                    interactable.transform.position
                );

                if (distance <= closestDistance)
                {
                    closestDistance = distance;
                    currentInteractable = interactable;
                }
            }

            if (OpenWorldMessageHUD.Instance == null)
                return;

            if (OpenWorldMessageHUD.Instance.IsShowingDialogue())
            {
                if (currentInteractable == null)
                    OpenWorldMessageHUD.Instance.HideMessage();

                return;
            }

            if (currentInteractable != null)
                OpenWorldMessageHUD.Instance.ShowPrompt(currentInteractable.interactionText);
            else
                OpenWorldMessageHUD.Instance.HideMessage();
        }
    }
}