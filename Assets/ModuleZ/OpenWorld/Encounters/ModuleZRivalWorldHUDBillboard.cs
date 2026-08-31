using UnityEngine;

namespace ModuleZ.OpenWorld.Encounters
{
    public class ModuleZRivalWorldHUDBillboard : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Camera.main == null)
                return;

            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 direction = cameraPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            transform.rotation = Quaternion.LookRotation(-direction.normalized, Vector3.up);
        }
    }
}