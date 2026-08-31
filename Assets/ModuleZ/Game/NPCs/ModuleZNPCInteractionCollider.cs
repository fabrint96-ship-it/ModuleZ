using UnityEngine;

namespace ModuleZ.Game.NPCs
{
    [RequireComponent(typeof(BoxCollider))]
    public class ModuleZNPCInteractionCollider : MonoBehaviour
    {
        [Header("Collider")]
        [SerializeField]
        private Vector3 center =
            new Vector3(0f, 0.9f, 0f);

        [SerializeField]
        private Vector3 size =
            new Vector3(1.2f, 2.0f, 1.2f);

        private void Awake()
        {
            ConfigureCollider();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ConfigureCollider();
        }
#endif

        private void ConfigureCollider()
        {
            BoxCollider collider =
                GetComponent<BoxCollider>();

            collider.isTrigger = true;
            collider.center = center;
            collider.size = size;
        }
    }
}