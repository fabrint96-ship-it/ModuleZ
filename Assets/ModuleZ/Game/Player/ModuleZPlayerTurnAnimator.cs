using UnityEngine;

namespace ModuleZ.Game.Player
{
    public class ModuleZPlayerTurnAnimator : MonoBehaviour
    {
        [Header("Turn Animation")]
        [SerializeField] private float turnDetectThreshold = 35f;
        [SerializeField] private float tiltAmount = 7f;
        [SerializeField] private float animationSpeed = 10f;

        private float lastYRotation;
        private float currentTilt;

        private Transform visualRoot;

        private void Start()
        {
            lastYRotation = transform.eulerAngles.y;
            visualRoot = transform;
        }

        private void Update()
        {
            float currentY = transform.eulerAngles.y;
            float delta = Mathf.DeltaAngle(lastYRotation, currentY);

            float targetTilt = 0f;

            if (Mathf.Abs(delta) > turnDetectThreshold * Time.deltaTime)
            {
                targetTilt = delta > 0f ? -tiltAmount : tiltAmount;
            }

            currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * animationSpeed);

            visualRoot.localRotation = Quaternion.Euler(0f, transform.localEulerAngles.y, currentTilt);

            lastYRotation = currentY;
        }
    }
}