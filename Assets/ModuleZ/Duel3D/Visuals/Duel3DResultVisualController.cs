using System.Collections;
using ModuleZ.Duel3D.Rules;
using UnityEngine;

namespace ModuleZ.Duel3D.Visuals
{
    public class Duel3DResultVisualController : MonoBehaviour
    {
        [SerializeField] private float pulseDuration = 1.2f;
        [SerializeField] private float pulseScale = 1.18f;

        public void PlayResult(
            Duel3DMatchResult result,
            Transform cubesRoot)
        {
            StopAllCoroutines();
            StartCoroutine(ResultRoutine(result, cubesRoot));
        }

        private IEnumerator ResultRoutine(
            Duel3DMatchResult result,
            Transform cubesRoot)
        {
            if (cubesRoot == null)
                yield break;

            float timer = 0f;

            while (timer < pulseDuration)
            {
                timer += Time.deltaTime;

                float pulse =
                    1f + Mathf.Sin(timer * 18f) * (pulseScale - 1f);

                for (int i = 0; i < cubesRoot.childCount; i++)
                {
                    Transform cube = cubesRoot.GetChild(i);

                    if (ShouldPulseCube(result, cube.name))
                        cube.localScale = Vector3.one * pulse;
                }

                yield return null;
            }

            for (int i = 0; i < cubesRoot.childCount; i++)
            {
                Transform cube = cubesRoot.GetChild(i);

                if (ShouldPulseCube(result, cube.name))
                    cube.localScale = Vector3.one;
            }
        }

        private bool ShouldPulseCube(
            Duel3DMatchResult result,
            string cubeName)
        {
            switch (result)
            {
                case Duel3DMatchResult.PlayerWin:
                    return cubeName.Contains("Player");

                case Duel3DMatchResult.OpponentWin:
                    return cubeName.Contains("Opponent");

                case Duel3DMatchResult.Draw:
                    return true;

                default:
                    return false;
            }
        }
    }
}