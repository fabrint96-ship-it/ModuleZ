using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelOpponentIdleAnimation : MonoBehaviour
    {
        [SerializeField] private float bounceHeight = 0.08f;
        [SerializeField] private float speed = 3.5f;

        private Vector3 startPosition;
        private float randomOffset;

        private void Awake()
        {
            startPosition = transform.position;
            randomOffset = Random.Range(0f, 10f);
        }

        private void Update()
        {
            float y = Mathf.Sin((Time.time + randomOffset) * speed) * bounceHeight;
            transform.position = startPosition + Vector3.up * y;
        }
    }
}