using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Game.DuelTransition
{
    public readonly struct DuelContext
    {
        public ModuleZRivalId RivalId { get; }
        public bool IsRematch { get; }
        public Vector3 ReturnPosition { get; }

        public DuelContext(
            ModuleZRivalId rivalId,
            bool isRematch,
            Vector3 returnPosition)
        {
            RivalId = rivalId;
            IsRematch = isRematch;
            ReturnPosition = returnPosition;
        }
    }
}
