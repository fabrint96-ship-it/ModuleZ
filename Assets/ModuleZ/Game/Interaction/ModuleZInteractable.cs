using ModuleZ.Game.NPCs;
using UnityEngine;

namespace ModuleZ.Game.Interaction
{
    public abstract class ModuleZInteractable : MonoBehaviour
    {
        public string interactionText = "Pulsa E para interactuar";

        public abstract void Interact();
    }
}