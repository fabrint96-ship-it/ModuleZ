using ModuleZ.UI.HUD;
using UnityEngine;

namespace ModuleZ.Game.NPCs
{
    public class ModuleZNPCConversationController : MonoBehaviour
    {
        private static float nextGlobalAmbientTime;

        [SerializeField] private string speakerName = "NPC";
        [SerializeField] private string[] ambientLines;

        [SerializeField] private float conversationRadius = 8f;
        [SerializeField] private float playerAvoidRadius = 5f;
        [SerializeField] private float minInterval = 8f;
        [SerializeField] private float maxInterval = 16f;
        [SerializeField] private float conversationDuration = 3f;
        [SerializeField] private float globalCooldown = 4f;

        private float nextConversationTime;
        private bool isConversing;

        private static readonly string[] Greetings =
        {
            "Hola.",
            "Buenas.",
            "¿Qué tal?",
            "Buenos días."
        };

        public void Configure(string name, string[] lines)
        {
            speakerName = name;
            ambientLines = lines;
            ResetTimer();
        }

        private void Update()
        {
            if (
                OpenWorldMessageHUD.Instance != null &&
                !OpenWorldMessageHUD.Instance.CanShowAmbientMessage()
            )
            {
                ResetTimer();
                return;
            }

            if (IsPlayerNearby())
            {
                ResetTimer();
                return;
            }

            if (isConversing)
                return;

            if (Time.time < nextConversationTime)
                return;

            if (Time.time < nextGlobalAmbientTime)
            {
                ResetTimer();
                return;
            }

            if (ambientLines == null || ambientLines.Length == 0)
            {
                ResetTimer();
                return;
            }

            ModuleZNPCConversationController other = FindNearbyConversationNPC();

            if (other == null)
            {
                ResetTimer();
                return;
            }

            StartConversation(other);
        }

        private void StartConversation(ModuleZNPCConversationController other)
        {
            isConversing = true;
            other.isConversing = true;

            nextGlobalAmbientTime = Time.time + globalCooldown;

            PausePatrol(this);
            PausePatrol(other);

            FaceOther(transform, other.transform);
            FaceOther(other.transform, transform);

            string myLine =
                ambientLines[Random.Range(0, ambientLines.Length)];

            string otherLine =
                other.ambientLines[
                    Random.Range(0, other.ambientLines.Length)
                ];

            string greeting =
                Greetings[Random.Range(0, Greetings.Length)];

            string message =
                speakerName + ": " + greeting +
                "\n" +
                other.speakerName + ": " + myLine;

            Debug.Log("[Module Z Ambient NPC] " + message);

            if (
                OpenWorldMessageHUD.Instance != null &&
                OpenWorldMessageHUD.Instance.CanShowAmbientMessage()
            )
            {
                OpenWorldMessageHUD.Instance.ShowAmbientMessage(message, 2.5f);
            }

            Invoke(nameof(EndConversation), conversationDuration);
            other.Invoke(nameof(other.EndConversation), conversationDuration);
        }

        private void EndConversation()
        {
            isConversing = false;
            ResumePatrol(this);
            ResetTimer();
        }

        private void PausePatrol(ModuleZNPCConversationController npc)
        {
            ModuleZNPCPatrolController patrol =
                npc.GetComponent<ModuleZNPCPatrolController>();

            if (patrol != null)
                patrol.SetPaused(true);
        }

        private void ResumePatrol(ModuleZNPCConversationController npc)
        {
            ModuleZNPCPatrolController patrol =
                npc.GetComponent<ModuleZNPCPatrolController>();

            if (patrol != null)
                patrol.SetPaused(false);
        }

        private void FaceOther(Transform source, Transform target)
        {
            Vector3 direction = target.position - source.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            source.rotation = Quaternion.LookRotation(direction.normalized);
        }

        private ModuleZNPCConversationController FindNearbyConversationNPC()
        {
            ModuleZNPCConversationController[] npcs =
                FindObjectsOfType<ModuleZNPCConversationController>();

            foreach (ModuleZNPCConversationController npc in npcs)
            {
                if (npc == this)
                    continue;

                if (npc.isConversing)
                    continue;

                float distance = Vector3.Distance(transform.position, npc.transform.position);

                if (distance <= conversationRadius)
                    return npc;
            }

            return null;
        }

        private void ResetTimer()
        {
            nextConversationTime = Time.time + Random.Range(minInterval, maxInterval);
        }

        private bool IsPlayerNearby()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
                return false;

            float distance = Vector3.Distance(transform.position, player.transform.position);

            return distance <= playerAvoidRadius;
        }
    }
}