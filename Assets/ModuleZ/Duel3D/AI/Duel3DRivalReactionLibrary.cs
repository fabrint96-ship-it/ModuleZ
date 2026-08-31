using ModuleZ.OpenWorld.Encounters;

namespace ModuleZ.Duel3D.AI
{
    public static class Duel3DRivalReactionLibrary
    {
        public static string GetPlayerBigMoveReaction(
            ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Miguel levanta una ceja. Buena jugada.";

                case ModuleZRivalId.Barcelona:
                    return "Jordi parece sorprendido por ese movimiento.";

                case ModuleZRivalId.Valencia:
                    return "Vicent observa el tablero con atención.";

                case ModuleZRivalId.Andalucia:
                    return "Antonio asiente lentamente.";
            }

            return "El rival estudia tu jugada.";
        }

        public static string GetOpponentBigMoveReaction(
            ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Miguel parece satisfecho con su movimiento.";

                case ModuleZRivalId.Barcelona:
                    return "Jordi juega con confianza.";

                case ModuleZRivalId.Valencia:
                    return "Vicent ha cerrado varias opciones.";

                case ModuleZRivalId.Andalucia:
                    return "Antonio sonríe como si hubiera previsto esto.";
            }

            return "El rival realiza una jugada importante.";
        }

        public static string GetNearVictoryReaction(
            ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Miguel ve cerca la victoria.";

                case ModuleZRivalId.Barcelona:
                    return "Jordi acelera el ritmo del duelo.";

                case ModuleZRivalId.Valencia:
                    return "Vicent controla gran parte del tablero.";

                case ModuleZRivalId.Andalucia:
                    return "Antonio domina la situación con experiencia.";
            }

            return "El rival toma ventaja.";
        }
    }
}