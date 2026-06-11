using UnityEngine;

namespace ModuleZ.Game.Camera
{
    public class ModuleZThirdPersonCamera : MonoBehaviour
    {
        private Transform target;

        [Header("Default Camera")]
        [SerializeField] private Vector3 defaultOffset = new Vector3(0f, 3.2f, -4.5f);

        [Header("Smooth")]
        [SerializeField] private float followSmooth = 8f;
        [SerializeField] private float rotationSmooth = 8f;

        [Header("Free Camera")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float freeDistance = 4.5f;
        [SerializeField] private float minPitch = -10f;
        [SerializeField] private float maxPitch = 55f;

        private bool freeCamera;
        private float yaw;
        private float pitch = 20f;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;

            if (target != null)
                yaw = target.eulerAngles.y;
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            HandleInput();

            if (freeCamera)
                UpdateFreeCamera();
            else
                UpdateFollowCamera();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButton(1))
            {
                freeCamera = true;

                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

                pitch = Mathf.Clamp(
                    pitch,
                    minPitch,
                    maxPitch
                );
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                freeCamera = false;

                yaw = target.eulerAngles.y;
                pitch = 20f;
            }
        }

        private void UpdateFollowCamera()
        {
            Vector3 desiredPosition =
                target.position +
                target.rotation * defaultOffset;

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSmooth * Time.deltaTime
            );

            LookAtTarget();
        }

        private void UpdateFreeCamera()
        {
            Quaternion orbitRotation =
                Quaternion.Euler(pitch, yaw, 0f);

            Vector3 desiredPosition =
                target.position +
                orbitRotation * new Vector3(0f, 0f, -freeDistance);

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSmooth * Time.deltaTime
            );

            LookAtTarget();
        }

        private void LookAtTarget()
        {
            Vector3 lookTarget =
                target.position + Vector3.up * 1.6f;

            Quaternion targetRotation =
                Quaternion.LookRotation(
                    lookTarget - transform.position
                );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmooth * Time.deltaTime
            );
        }
    }
}