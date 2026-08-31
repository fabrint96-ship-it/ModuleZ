using System.Collections;
using UnityEngine;

namespace ModuleZ.Duel3D.Visuals
{
    public class Duel3DPlaceEffect : MonoBehaviour
    {
        [SerializeField] private float duration = 0.15f;
        [SerializeField] private float overshootMultiplier = 1.15f;

        private Vector3 targetScale;

        private void Awake()
        {
            targetScale = transform.localScale;
            transform.localScale = Vector3.zero;
        }

        public void Play()
        {
            StopAllCoroutines();
            StartCoroutine(PlaceRoutine());
        }

        private IEnumerator PlaceRoutine()
        {
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                float t = Mathf.Clamp01(timer / duration);

                float scaleFactor;

                if (t < 0.7f)
                {
                    scaleFactor = Mathf.Lerp(
                        0f,
                        overshootMultiplier,
                        t / 0.7f
                    );
                }
                else
                {
                    scaleFactor = Mathf.Lerp(
                        overshootMultiplier,
                        1f,
                        (t - 0.7f) / 0.3f
                    );
                }

                transform.localScale =
                    targetScale * scaleFactor;

                yield return null;
            }

            transform.localScale = targetScale;
        }
    }
}