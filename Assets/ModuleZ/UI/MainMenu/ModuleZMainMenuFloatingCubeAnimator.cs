using UnityEngine;

namespace ModuleZ.UI.MainMenu
{
    public class ModuleZMainMenuFloatingCubeAnimator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private float floatAmplitude = 0.2f;
        [SerializeField] private float floatSpeed = 1.5f;

        private Vector3 startPosition;
        private float offset;

        private void Start()
        {
            startPosition = transform.position;
            offset = Random.Range(0f, 10f);
        }

        private void Update()
        {
            transform.Rotate(
                rotationSpeed * Time.deltaTime,
                rotationSpeed * 0.5f * Time.deltaTime,
                0f
            );

            Vector3 pos = startPosition;

            pos.y += Mathf.Sin(
                Time.time * floatSpeed + offset
            ) * floatAmplitude;

            transform.position = pos;
        }
    }
}