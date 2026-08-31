using UnityEngine;

namespace ModuleZ.Game.NPCs
{
    public class ModuleZNPCPatrolController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 1.4f;
        [SerializeField] private float rotationSpeed = 420f;
        [SerializeField] private float waitTime = 1.2f;
        [SerializeField] private bool randomIdleEnabled = true;
        [SerializeField] private float idleChance = 0.35f;
        [SerializeField] private float minIdleTime = 1f;
        [SerializeField] private float maxIdleTime = 4f;

        [Header("Obstacle Avoidance")]
        [SerializeField] private float obstacleCheckDistance = 0.75f;
        [SerializeField] private float obstacleCheckHeight = 0.6f;
        [SerializeField] private float obstacleWaitTime = 0.7f;
        [SerializeField] private float blockedSkipTime = 2f;
        [SerializeField] private LayerMask obstacleMask = ~0;

        [Header("NPC visual front")]
        [SerializeField] private bool npcFaceLooksToNegativeZ = true;

        private Vector3[] waypoints;
        private int currentWaypointIndex;
        private float waitUntil;
        private bool paused;
        private float blockedSince = -1f;

        public void Configure(Vector3[] patrolPoints, float speed = 1.4f)
        {
            waypoints = patrolPoints;
            moveSpeed = speed;
            currentWaypointIndex = 0;
            waitUntil = 0f;
            blockedSince = -1f;
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

            if (direction.magnitude < 0.12f)
            {
                GoToNextWaypoint();
                return;
            }

            RotateVisualFaceTowards(direction);

            if (IsObstacleAhead(direction))
            {
                HandleObstacleBlocked();
                return;
            }

            blockedSince = -1f;
            MoveTowards(direction);
        }

        private bool IsObstacleAhead(Vector3 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
                return false;

            Vector3 origin =
                transform.position + Vector3.up * obstacleCheckHeight;

            Vector3 dir =
                direction.normalized;

            RaycastHit hit;

            if (Physics.Raycast(
                    origin,
                    dir,
                    out hit,
                    obstacleCheckDistance,
                    obstacleMask,
                    QueryTriggerInteraction.Ignore))
            {
                if (hit.transform == transform)
                    return false;

                if (hit.transform.IsChildOf(transform))
                    return false;

                if (hit.collider.GetComponent<ModuleZNPCInteractionCollider>() != null)
                    return false;

                return true;
            }

            return false;
        }

        private void HandleObstacleBlocked()
        {
            if (blockedSince < 0f)
                blockedSince = Time.time;

            waitUntil = Time.time + obstacleWaitTime;

            if (Time.time - blockedSince >= blockedSkipTime)
            {
                blockedSince = -1f;
                GoToNextWaypoint();
            }
        }

        private void GoToNextWaypoint()
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
                currentWaypointIndex = 0;

            float finalWait = waitTime;

            if (randomIdleEnabled && Random.value <= idleChance)
                finalWait += Random.Range(minIdleTime, maxIdleTime);

            waitUntil = Time.time + finalWait;
        }

        private void RotateVisualFaceTowards(Vector3 direction)
        {
            if (direction.sqrMagnitude < 0.001f)
                return;

            Quaternion lookRotation =
                Quaternion.LookRotation(direction.normalized, Vector3.up);

            if (npcFaceLooksToNegativeZ)
                lookRotation *= Quaternion.Euler(0f, 180f, 0f);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        private void MoveTowards(Vector3 direction)
        {
            Vector3 movement =
                direction.normalized * moveSpeed * Time.deltaTime;

            transform.position += movement;
        }
    }
}