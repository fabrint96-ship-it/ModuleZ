using UnityEngine;

namespace ModuleZ.Duel3D.Runtime
{
    public class Duel3DOrbitCameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Vector3 targetPosition = Vector3.zero;

        [Header("Orbit")]
        [SerializeField] private float yaw = 0f;
        [SerializeField] private float pitch = 38f;
        [SerializeField] private float orbitSpeed = 4f;

        [Header("Zoom")]
        [SerializeField] private float distance = 8f;
        [SerializeField] private float minDistance = 4f;
        [SerializeField] private float maxDistance = 14f;
        [SerializeField] private float zoomSpeed = 4f;

        [Header("Limits")]
        [SerializeField] private float minPitch = 20f;
        [SerializeField] private float maxPitch = 75f;

        private Camera controlledCamera;

        public void Initialize(Camera camera, Vector3 target)
        {
            controlledCamera = camera;
            targetPosition = target;

            ApplyCameraPosition();
        }

        private void Update()
        {
            if (controlledCamera == null)
                return;

            HandleOrbit();
            HandleZoom();
            HandleReset();

            ApplyCameraPosition();
        }

        private void HandleOrbit()
        {
            if (!Input.GetMouseButton(1))
                return;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            yaw += mouseX * orbitSpeed;
            pitch -= mouseY * orbitSpeed;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        private void HandleZoom()
        {
            float scroll = Input.mouseScrollDelta.y;

            if (Mathf.Abs(scroll) < 0.01f)
                return;

            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        private void HandleReset()
        {
            if (!Input.GetKeyDown(KeyCode.T))
                return;

            yaw = 0f;
            pitch = 38f;
            distance = 8f;
        }

        private void ApplyCameraPosition()
        {
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 offset = rotation * new Vector3(0f, 0f, -distance);

            controlledCamera.transform.position = targetPosition + offset;
            controlledCamera.transform.LookAt(targetPosition);
        }
    }
}