using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModuleZ.Duel.Builders
{
    public class DuelZPieceController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveStep = 1f;
        [SerializeField] private float moveCooldown = 0.12f;
        [SerializeField] private float moveSmooth = 14f;

        [Header("Rotation")]
        [SerializeField] private float rotationStep = 90f;
        [SerializeField] private float rotationSmooth = 14f;

        [Header("Board Limits")]
        [SerializeField] private float minX = -5f;
        [SerializeField] private float maxX = 5f;
        [SerializeField] private float minZ = -4f;
        [SerializeField] private float maxZ = 4f;

        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private float nextMoveTime;
        private bool locked;

        private void Awake()
        {
            targetPosition = transform.position;
            targetRotation = transform.rotation;
        }

        private void Update()
        {
            if (locked)
                return;

            if (ModuleZ.Core.Managers.ModuleZGameState.IsPaused)
                return;

            HandleInput();
            SmoothTransform();
        }

        public void LockPiece()
        {
            locked = true;
        }

        private void HandleInput()
        {
            if (Time.time < nextMoveTime)
                return;

            Vector3 direction = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.UpArrow))
                direction = Vector3.forward;
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                direction = Vector3.back;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                direction = Vector3.left;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                direction = Vector3.right;

            if (direction != Vector3.zero)
            {
                Vector3 nextPosition = targetPosition + direction * moveStep;
                nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
                nextPosition.z = Mathf.Clamp(nextPosition.z, minZ, maxZ);

                targetPosition = nextPosition;
                nextMoveTime = Time.time + moveCooldown;

                if (ModuleZ.Duel.Rules.DuelAudioFeedbackController.Instance != null)
                    ModuleZ.Duel.Rules.DuelAudioFeedbackController.Instance.PlayMovePieceSound();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                targetRotation *= Quaternion.Euler(0f, -rotationStep, 0f);

                if (ModuleZ.Duel.Rules.DuelAudioFeedbackController.Instance != null)
                    ModuleZ.Duel.Rules.DuelAudioFeedbackController.Instance.PlayRotatePieceSound();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                targetRotation *= Quaternion.Euler(0f, rotationStep, 0f);

                if (ModuleZ.Duel.Rules.DuelAudioFeedbackController.Instance != null)
                    ModuleZ.Duel.Rules.DuelAudioFeedbackController.Instance.PlayRotatePieceSound();
            }
        }

        private void SmoothTransform()
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                moveSmooth * Time.deltaTime
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmooth * Time.deltaTime
            );
        }
    }
}