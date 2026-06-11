using UnityEngine;

namespace ModuleZ.Duel.Rules
{
    public class DuelTimerController : MonoBehaviour
    {
        [SerializeField] private float duelTime = 45f;

        private bool timerRunning = true;

        private void Update()
        {
            if (!timerRunning)
                return;

            duelTime -= Time.deltaTime;

            if (DuelHUDController.Instance != null)
                DuelHUDController.Instance.ShowTimer(duelTime);

            if (duelTime <= 0f)
            {
                timerRunning = false;

                if (DuelResultManager.Instance != null)
                    DuelResultManager.Instance.LoseDuel();
            }
        }

        public void StopTimer()
        {
            timerRunning = false;
        }

        public void SetTime(float time)
        {
            duelTime = time;
        }
    }
}