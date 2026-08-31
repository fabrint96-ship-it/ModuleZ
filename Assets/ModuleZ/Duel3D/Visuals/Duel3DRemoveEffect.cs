using System.Collections;
using UnityEngine;

namespace ModuleZ.Duel3D.Visuals
{
    public class Duel3DRemoveEffect : MonoBehaviour
    {
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private float blinkSpeed = 18f;

        private Vector3 originalScale;
        private Renderer cubeRenderer;

        private void Awake()
        {
            originalScale = transform.localScale;
            cubeRenderer = GetComponent<Renderer>();
        }

        public void PlayAndDestroy()
        {
            StartCoroutine(RemoveRoutine());
        }

        private IEnumerator RemoveRoutine()
        {
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                float t = timer / duration;
                float scale = Mathf.Lerp(1f, 0f, t);

                transform.localScale = originalScale * scale;

                if (cubeRenderer != null)
                    cubeRenderer.enabled = Mathf.Sin(timer * blinkSpeed) > 0f;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}