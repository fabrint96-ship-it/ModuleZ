using ModuleZ.OpenWorld.Encounters;

namespace ModuleZ.Game.DuelTransition
{
    public readonly struct DuelResult
    {
        public DuelOutcome Outcome { get; }
        public ModuleZRivalId RivalId { get; }

        public DuelResult(
            DuelOutcome outcome,
            ModuleZRivalId rivalId)
        {
            Outcome = outcome;
            RivalId = rivalId;
        }
    }
}
