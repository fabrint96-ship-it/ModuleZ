using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelVictoryEffect : MonoBehaviour
    {
        [SerializeField] private float duration = 1.2f;
        [SerializeField] private float rotateSpeed = 180f;
        [SerializeField] private float bounceHeight = 0.25f;
        [SerializeField] private float bounceSpeed = 8f;

        private Vector3 startPosition;
        private float endTime;
        private bool playing;

        public void Play()
        {
            startPosition = transform.position;
            endTime = Time.time + duration;
            playing = true;
        }

        private void Update()
        {
            if (!playing)
                return;

            if (Time.time >= endTime)
            {
                playing = false;
                transform.position = startPosition;
                return;
            }

            transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);

            float y = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            transform.position = startPosition + Vector3.up * y;
        }
    }
}