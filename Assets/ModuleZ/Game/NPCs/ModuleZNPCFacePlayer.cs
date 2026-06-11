using UnityEngine;

namespace ModuleZ.Game.NPCs
{
    public class ModuleZNPCFacePlayer : MonoBehaviour
    {
        public void Face(Transform player)
        {
            if (player == null)
                return;

            Vector3 direction = player.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
                return;

            transform.rotation = Quaternion.LookRotation(-direction.normalized);
        }
    }
}