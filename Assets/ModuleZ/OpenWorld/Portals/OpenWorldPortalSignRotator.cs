using UnityEngine;

namespace ModuleZ.OpenWorld.Portals
{
    public class OpenWorldPortalSignRotator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 35f;

        private void Update()
        {
            transform.Rotate(
                0f,
                rotationSpeed * Time.deltaTime,
                0f,
                Space.Self
            );
        }
    }
}