using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.OpenWorld.Encounters
{
    public class ModuleZRivalWorldHUDController : MonoBehaviour
    {
        [SerializeField] private float showDistance = 6f;
        [SerializeField] private float animationSpeed = 8f;

        private ModuleZRivalId rivalId;

        private Text statusText;
        private Text nameText;
        private Text descriptionText;
        private Text actionText;

        private Transform player;
        private Canvas canvas;
        private Vector3 hiddenScale;
        private Vector3 visibleScale;
        private bool shouldShow;

        public void Initialize(
            ModuleZRivalId id,
            Text status,
            Text name,
            Text description,
            Text action)
        {
            rivalId = id;
            statusText = status;
            nameText = name;
            descriptionText = description;
            actionText = action;

            canvas = GetComponent<Canvas>();

            visibleScale = transform.localScale;
            hiddenScale = Vector3.zero;

            transform.localScale = hiddenScale;

            if (canvas != null)
                canvas.enabled = false;

            Refresh();
        }

        private void Update()
        {
            FindPlayerIfNeeded();
            UpdateVisibility();
            Animate();
        }

        public void Refresh()
        {
            if (statusText != null)
                statusText.text = ModuleZRivalHUDTextLibrary.GetRivalStatus(rivalId);

            if (nameText != null)
                nameText.text = ModuleZRivalHUDTextLibrary.GetRivalName(rivalId);

            if (descriptionText != null)
                descriptionText.text = ModuleZRivalHUDTextLibrary.GetRivalDescription(rivalId);

            if (actionText != null)
                actionText.text = ModuleZRivalHUDTextLibrary.GetRivalAction(rivalId);
        }

        private void FindPlayerIfNeeded()
        {
            if (player != null)
                return;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
                player = playerObj.transform;
        }

        private void UpdateVisibility()
        {
            if (player == null)
            {
                shouldShow = false;
                return;
            }

            Vector3 hudPos = transform.position;
            Vector3 playerPos = player.position;

            hudPos.y = 0f;
            playerPos.y = 0f;

            float distance = Vector3.Distance(hudPos, playerPos);

            shouldShow = distance <= showDistance;

            if (canvas != null && shouldShow)
                canvas.enabled = true;
        }

        private void Animate()
        {
            Vector3 targetScale =
                shouldShow ? visibleScale : hiddenScale;

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );

            if (!shouldShow && transform.localScale.magnitude < 0.01f)
            {
                transform.localScale = hiddenScale;

                if (canvas != null)
                    canvas.enabled = false;
            }
        }

        private void OnEnable()
        {
            Refresh();
        }

        public static void RefreshAll()
        {
            ModuleZRivalWorldHUDController[] controllers =
                FindObjectsOfType<ModuleZRivalWorldHUDController>();

            for (int i = 0; i < controllers.Length; i++)
                controllers[i].Refresh();
        }
    }
}