using UnityEngine;

namespace ModuleZ.UI.MainMenu
{
    public class ModuleZMainMenuCameraAnimator : MonoBehaviour
    {
        [SerializeField] private Vector3 lookTarget = new Vector3(0f, 1.6f, 0f);
        [SerializeField] private float orbitRadius = 10f;
        [SerializeField] private float height = 5.2f;
        [SerializeField] private float orbitSpeed = 0.12f;

        private float angle;

        private void Update()
        {
            angle += orbitSpeed * Time.deltaTime;

            float x = Mathf.Sin(angle) * orbitRadius;
            float z = -Mathf.Cos(angle) * orbitRadius;

            transform.position = new Vector3(x, height, z);
            transform.LookAt(lookTarget);
        }
    }
}