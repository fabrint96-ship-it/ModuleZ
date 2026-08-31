using ModuleZ.OpenWorld.Encounters;

namespace ModuleZ.Duel3D.AI
{
    [System.Serializable]
    public class Duel3DRivalProfile
    {
        public ModuleZRivalId rivalId;

        public string displayName;

        public float aggressiveness;
        public float defensiveBias;
        public float blockChance;
        public float mistakeChance;
        public float reactionDelay;
        public int lookAheadDepth;

        public Duel3DRivalProfile(
            ModuleZRivalId rivalId,
            string displayName,
            float aggressiveness,
            float defensiveBias,
            float blockChance,
            float mistakeChance,
            float reactionDelay,
            int lookAheadDepth)
        {
            this.rivalId = rivalId;
            this.displayName = displayName;
            this.aggressiveness = aggressiveness;
            this.defensiveBias = defensiveBias;
            this.blockChance = blockChance;
            this.mistakeChance = mistakeChance;
            this.reactionDelay = reactionDelay;
            this.lookAheadDepth = lookAheadDepth;
        }
    }
}