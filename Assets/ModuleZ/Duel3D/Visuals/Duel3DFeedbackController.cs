using System.Collections;
using UnityEngine;

namespace ModuleZ.Duel3D.Visuals
{
    public class Duel3DFeedbackController : MonoBehaviour
    {
        [Header("Camera Shake")]
        [SerializeField] private float shakeDuration = 0.12f;
        [SerializeField] private float shakeStrength = 0.05f;

        private Camera mainCamera;
        private bool shaking;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public void PlayPlaceFeedback()
        {
            Debug.Log("[ModuleZ] Feedback: pieza colocada.");
        }

        public void PlayRemoveFeedback()
        {
            Debug.Log("[ModuleZ] Feedback: cubos eliminados.");
            PlayCameraShake();
        }

        public void PlayInvalidMoveFeedback()
        {
            Debug.Log("[ModuleZ] Feedback: jugada inválida.");
            PlayCameraShake();
        }

        public void PlayTurnChangedFeedback()
        {
            Debug.Log("[ModuleZ] Feedback: cambio de turno.");
        }

        private void PlayCameraShake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera == null || shaking)
                return;

            StartCoroutine(CameraShakeRoutine());
        }

        private IEnumerator CameraShakeRoutine()
        {
            shaking = true;

            Transform cameraTransform = mainCamera.transform;
            Vector3 originalPosition = cameraTransform.localPosition;

            float timer = 0f;

            while (timer < shakeDuration)
            {
                timer += Time.deltaTime;

                Vector3 offset = Random.insideUnitSphere * shakeStrength;
                cameraTransform.localPosition = originalPosition + offset;

                yield return null;
            }

            cameraTransform.localPosition = originalPosition;
            shaking = false;
        }
    }
}