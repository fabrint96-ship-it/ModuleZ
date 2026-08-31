using ModuleZ.Core.Managers;
using ModuleZ.UI.HUD;
using UnityEngine;

namespace ModuleZ.Game.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class ModuleZPlayerController : MonoBehaviour
    {
        public bool CanMove { get; set; } = true;

        [Header("Movement - ProjectZ Style")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float backwardSpeedMultiplier = 0.55f;
        [SerializeField] private float rotationSpeed = 135f;
        [SerializeField] private float gravity = -18f;

        private CharacterController controller;
        private Vector3 verticalVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!CanMove)
                return;

            if (ModuleZGameState.IsPaused)
                return;

            if (ModuleZGameState.IsOverlayOpen)
                return;

            if (OpenWorldMessageHUD.Instance != null &&
                OpenWorldMessageHUD.Instance.IsShowingDialogue())
                return;

            RotatePlayer();
            MovePlayer();
            ApplyGravity();
        }

        private void RotatePlayer()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(horizontal) < 0.01f)
                return;

            transform.Rotate(
                Vector3.up,
                horizontal * rotationSpeed * Time.deltaTime
            );
        }

        private void MovePlayer()
        {
            float vertical = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(vertical) < 0.01f)
                return;

            float speed = vertical > 0f
                ? moveSpeed
                : moveSpeed * backwardSpeedMultiplier;

            Vector3 movement =
                transform.forward * vertical * speed;

            controller.Move(movement * Time.deltaTime);
        }

        private void ApplyGravity()
        {
            if (controller.isGrounded && verticalVelocity.y < 0f)
                verticalVelocity.y = -2f;

            verticalVelocity.y += gravity * Time.deltaTime;

            controller.Move(verticalVelocity * Time.deltaTime);
        }
    }
}