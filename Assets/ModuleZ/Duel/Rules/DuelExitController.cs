using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelExitController : MonoBehaviour
    {
        private bool exiting;

        private void Update()
        {
            if (exiting)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                exiting = true;

                if (DuelResultManager.Instance != null)
                    DuelResultManager.Instance.AbandonDuel();
            }
        }
    }
}