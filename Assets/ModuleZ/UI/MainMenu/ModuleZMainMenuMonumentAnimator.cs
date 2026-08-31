using UnityEngine;

namespace ModuleZ.UI.MainMenu
{
    public class ModuleZMainMenuMonumentAnimator : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 18f;
        [SerializeField] private float floatAmplitude = 0.25f;
        [SerializeField] private float floatSpeed = 1.2f;

        private Vector3 startPosition;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            transform.Rotate(
                0f,
                rotationSpeed * Time.deltaTime,
                0f,
                Space.World
            );

            Vector3 position = startPosition;

            position.y += Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

            transform.position = position;
        }
    }
}