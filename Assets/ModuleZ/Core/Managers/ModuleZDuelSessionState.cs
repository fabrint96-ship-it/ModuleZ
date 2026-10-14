using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Core.Managers
{
    public static class ModuleZDuelSessionState
    {
        public static bool HasActiveDuel;
        public static ModuleZRivalId RivalId;
        public static bool IsRematch;
        public static Vector3 ReturnPosition;
        // Changes whenever session ownership changes, including a repeated
        // payload. A delayed transition may only undo its own publication.
        public static ulong Revision { get; private set; }

        public static void StartDuel(
            ModuleZRivalId rivalId,
            bool isRematch,
            Vector3 returnPosition)
        {
            RivalId = rivalId;
            IsRematch = isRematch;
            ReturnPosition = returnPosition;
            HasActiveDuel = true;
            Revision++;
        }

        public static void Clear()
        {
            HasActiveDuel = false;
            RivalId = default;
            IsRematch = false;
            ReturnPosition = Vector3.zero;
            Revision++;
        }
    }
}
