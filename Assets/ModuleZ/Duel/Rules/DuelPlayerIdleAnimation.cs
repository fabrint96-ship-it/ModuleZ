using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelPlayerIdleAnimation : MonoBehaviour
    {
        [SerializeField] private float bounceHeight = 0.06f;
        [SerializeField] private float speed = 3f;

        private Vector3 startPosition;

        private void Awake()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            float y = Mathf.Sin(Time.time * speed) * bounceHeight;
            transform.position = startPosition + Vector3.up * y;
        }
    }
}