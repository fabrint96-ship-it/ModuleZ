using System.Collections;
using UnityEngine;

namespace ModuleZ.Game.Animation
{
    public class ModuleZTalkAnimation : MonoBehaviour
    {
        [SerializeField] private float talkDuration = 2.5f;
        [SerializeField] private float bobSpeed = 8f;
        [SerializeField] private float bobAmount = 0.08f;

        private bool talking;
        private Vector3 startPosition;

        private void Start()
        {
            startPosition = transform.localPosition;
        }

        public void PlayTalkAnimation()
        {
            if (!talking)
                StartCoroutine(TalkRoutine());
        }

        private IEnumerator TalkRoutine()
        {
            talking = true;

            float timer = 0f;

            while (timer < talkDuration)
            {
                timer += Time.deltaTime;

                float offset =
                    Mathf.Sin(Time.time * bobSpeed) * bobAmount;

                transform.localPosition =
                    startPosition + Vector3.up * offset;

                yield return null;
            }

            transform.localPosition = startPosition;
            talking = false;
        }
    }
}