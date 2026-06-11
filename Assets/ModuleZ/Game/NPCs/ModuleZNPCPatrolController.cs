using UnityEngine;

namespace ModuleZ.Game.NPCs
{
    public class ModuleZNPCPatrolController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1.4f;
        [SerializeField] private float waitTime = 1.2f;
        [SerializeField] private bool randomIdleEnabled = true;
        [SerializeField] private float idleChance = 0.35f;
        [SerializeField] private float minIdleTime = 1f;
        [SerializeField] private float maxIdleTime = 4f;

        private Vector3[] waypoints;
        private int currentWaypointIndex;
        private float waitUntil;
        private bool paused;

        public void Configure(Vector3[] patrolPoints, float speed = 1.4f)
        {
            waypoints = patrolPoints;
            moveSpeed = speed;
            currentWaypointIndex = 0;
        }

        public void SetPaused(bool value)
        {
            paused = value;
        }

        private void Update()
        {
            if (paused)
                return;

            if (waypoints == null || waypoints.Length < 2)
                return;

            if (Time.time < waitUntil)
                return;

            Vector3 target = waypoints[currentWaypointIndex];

            Vector3 direction = target - transform.position;
            direction.y = 0f;

            if (direction.magnitude < 0.1f)
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length)
                    currentWaypointIndex = 0;

                float finalWait = waitTime;

                if (randomIdleEnabled && Random.value <= idleChance)
                {
                    finalWait += Random.Range(minIdleTime, maxIdleTime);
                }

                if (Random.value < 0.5f)
                {
                    transform.rotation = Quaternion.Euler(
                        0f,
                        Random.Range(0f, 360f),
                        0f
                    );
                }

                waitUntil = Time.time + finalWait;
                return;
            }

            Vector3 movement =
                direction.normalized * moveSpeed * Time.deltaTime;

            transform.position += movement;

            if (movement.sqrMagnitude > 0.001f)
                transform.rotation =
                    Quaternion.LookRotation(movement.normalized);
        }
    }
}