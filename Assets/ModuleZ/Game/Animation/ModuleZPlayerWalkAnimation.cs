using UnityEngine;

namespace ModuleZ.Game.Animation
{
    public class ModuleZPlayerWalkAnimation : MonoBehaviour
    {
        [SerializeField] private float swingSpeed = 8f;
        [SerializeField] private float swingAmount = 22f;
        [SerializeField] private float bodyBobSpeed = 8f;
        [SerializeField] private float bodyBobAmount = 0.04f;

        private Transform leftArm;
        private Transform rightArm;
        private Transform leftLeg;
        private Transform rightLeg;
        private Transform torso;

        private Vector3 lastPosition;
        private Vector3 torsoStartLocalPosition;

        public void Configure(
            Transform lArm,
            Transform rArm,
            Transform lLeg,
            Transform rLeg,
            Transform torsoTransform)
        {
            leftArm = lArm;
            rightArm = rArm;
            leftLeg = lLeg;
            rightLeg = rLeg;
            torso = torsoTransform;

            if (torso != null)
                torsoStartLocalPosition = torso.localPosition;
        }

        private void Start()
        {
            lastPosition = transform.position;

            if (torso != null)
                torsoStartLocalPosition = torso.localPosition;
        }

        private void Update()
        {
            float movement = Vector3.Distance(transform.position, lastPosition);
            lastPosition = transform.position;

            if (movement < 0.001f)
            {
                ResetAnimation();
                return;
            }

            AnimateWalk();
        }

        private void AnimateWalk()
        {
            float swing = Mathf.Sin(Time.time * swingSpeed) * swingAmount;
            float bob = Mathf.Abs(Mathf.Sin(Time.time * bodyBobSpeed)) * bodyBobAmount;

            if (leftArm != null)
                leftArm.localRotation = Quaternion.Euler(swing, 0f, 0f);

            if (rightArm != null)
                rightArm.localRotation = Quaternion.Euler(-swing, 0f, 0f);

            if (leftLeg != null)
                leftLeg.localRotation = Quaternion.Euler(-swing, 0f, 0f);

            if (rightLeg != null)
                rightLeg.localRotation = Quaternion.Euler(swing, 0f, 0f);

            if (torso != null)
                torso.localPosition = torsoStartLocalPosition + Vector3.up * bob;
        }

        private void ResetAnimation()
        {
            if (leftArm != null)
                leftArm.localRotation = Quaternion.identity;

            if (rightArm != null)
                rightArm.localRotation = Quaternion.identity;

            if (leftLeg != null)
                leftLeg.localRotation = Quaternion.identity;

            if (rightLeg != null)
                rightLeg.localRotation = Quaternion.identity;

            if (torso != null)
                torso.localPosition = Vector3.Lerp(
                    torso.localPosition,
                    torsoStartLocalPosition,
                    10f * Time.deltaTime
                );
        }
    }
}