using ModuleZ.OpenWorld.Encounters;

namespace ModuleZ.Duel3D.AI
{
    public static class Duel3DRivalProfileLibrary
    {
        public static Duel3DRivalProfile Get(
            ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return CreateMadrid();

                case ModuleZRivalId.Barcelona:
                    return CreateBarcelona();

                case ModuleZRivalId.Valencia:
                    return CreateValencia();

                case ModuleZRivalId.Andalucia:
                    return CreateAndalucia();

                default:
                    return CreateMadrid();
            }
        }

        private static Duel3DRivalProfile CreateMadrid()
        {
            return new Duel3DRivalProfile(
                ModuleZRivalId.Madrid,
                "Miguel de Madrid",

                aggressiveness: 0.50f,
                defensiveBias: 0.50f,
                blockChance: 0.45f,
                mistakeChance: 0.18f,
                reactionDelay: 1.40f,
                lookAheadDepth: 1
            );
        }

        private static Duel3DRivalProfile CreateBarcelona()
        {
            return new Duel3DRivalProfile(
                ModuleZRivalId.Barcelona,
                "Jordi de Barcelona",

                aggressiveness: 0.80f,
                defensiveBias: 0.30f,
                blockChance: 0.35f,
                mistakeChance: 0.12f,
                reactionDelay: 1.10f,
                lookAheadDepth: 2
            );
        }

        private static Duel3DRivalProfile CreateValencia()
        {
            return new Duel3DRivalProfile(
                ModuleZRivalId.Valencia,
                "Vicent de Valencia",

                aggressiveness: 0.35f,
                defensiveBias: 0.85f,
                blockChance: 0.75f,
                mistakeChance: 0.10f,
                reactionDelay: 1.20f,
                lookAheadDepth: 2
            );
        }

        private static Duel3DRivalProfile CreateAndalucia()
        {
            return new Duel3DRivalProfile(
                ModuleZRivalId.Andalucia,
                "Antonio de Andalucía",

                aggressiveness: 0.90f,
                defensiveBias: 0.90f,
                blockChance: 0.90f,
                mistakeChance: 0.04f,
                reactionDelay: 0.75f,
                lookAheadDepth: 3
            );
        }
    }
}