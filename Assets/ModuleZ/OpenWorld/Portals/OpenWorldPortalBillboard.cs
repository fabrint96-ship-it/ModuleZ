using UnityEngine;

namespace ModuleZ.OpenWorld.Portals
{
    public class OpenWorldPortalBillboard : MonoBehaviour
    {
        private void LateUpdate()
        {
            Camera cam = Camera.main;

            if (cam == null)
                return;

            transform.rotation = Quaternion.LookRotation(
                transform.position - cam.transform.position
            );
        }
    }
}