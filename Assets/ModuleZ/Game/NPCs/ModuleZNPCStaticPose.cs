using UnityEngine;

namespace ModuleZ.Game.NPCs
{
    public class ModuleZNPCStaticPose : MonoBehaviour
    {
        public enum PoseType
        {
            Standing,
            Sitting,
            Leaning
        }

        [SerializeField] private PoseType poseType = PoseType.Standing;

        public void Configure(PoseType pose)
        {
            poseType = pose;
            ApplyPose();
        }

        private void Start()
        {
            ApplyPose();
        }

        private void ApplyPose()
        {
            if (poseType == PoseType.Sitting)
                ApplySittingPose();

            if (poseType == PoseType.Leaning)
                transform.localRotation = Quaternion.Euler(0f, transform.localEulerAngles.y, 8f);
        }

        private void ApplySittingPose()
        {
            Transform leftLeg = transform.Find("Pierna_Izq");
            Transform rightLeg = transform.Find("Pierna_Der");

            if (leftLeg != null)
            {
                leftLeg.localPosition = new Vector3(-0.18f, 0.35f, -0.18f);
                leftLeg.localScale = new Vector3(0.25f, 0.45f, 0.45f);
            }

            if (rightLeg != null)
            {
                rightLeg.localPosition = new Vector3(0.18f, 0.35f, -0.18f);
                rightLeg.localScale = new Vector3(0.25f, 0.45f, 0.45f);
            }
        }
    }
}