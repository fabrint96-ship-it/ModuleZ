using UnityEngine;

namespace ModuleZ.Game.Animation
{
    public class ModuleZNPCWalkAnimation : MonoBehaviour
    {
        [SerializeField] private float swingSpeed = 6f;
        [SerializeField] private float swingAmount = 25f;

        private Transform leftArm;
        private Transform rightArm;
        private Transform leftLeg;
        private Transform rightLeg;

        private Vector3 lastPosition;

        public void Configure(
            Transform lArm,
            Transform rArm,
            Transform lLeg,
            Transform rLeg)
        {
            leftArm = lArm;
            rightArm = rArm;
            leftLeg = lLeg;
            rightLeg = rLeg;
        }

        private void Start()
        {
            lastPosition = transform.position;
        }

        private void Update()
        {
            float movement =
                Vector3.Distance(transform.position, lastPosition);

            lastPosition = transform.position;

            if (movement < 0.001f)
            {
                ResetLimbs();
                return;
            }

            float angle =
                Mathf.Sin(Time.time * swingSpeed) * swingAmount;

            if (leftArm != null)
                leftArm.localRotation = Quaternion.Euler(angle, 0f, 0f);

            if (rightArm != null)
                rightArm.localRotation = Quaternion.Euler(-angle, 0f, 0f);

            if (leftLeg != null)
                leftLeg.localRotation = Quaternion.Euler(-angle, 0f, 0f);

            if (rightLeg != null)
                rightLeg.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }

        private void ResetLimbs()
        {
            if (leftArm != null)
                leftArm.localRotation = Quaternion.identity;

            if (rightArm != null)
                rightArm.localRotation = Quaternion.identity;

            if (leftLeg != null)
                leftLeg.localRotation = Quaternion.identity;

            if (rightLeg != null)
                rightLeg.localRotation = Quaternion.identity;
        }
    }
}