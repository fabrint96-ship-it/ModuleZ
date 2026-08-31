using ModuleZ.OpenWorld.Encounters;

namespace ModuleZ.Duel3D.AI
{
    public static class Duel3DRivalVictoryLibrary
    {
        public static string GetPlayerVictoryMessage(
            ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Miguel asiente. Has jugado mejor esta vez.";

                case ModuleZRivalId.Barcelona:
                    return "Jordi sonríe. Buena partida.";

                case ModuleZRivalId.Valencia:
                    return "Vicent observa el tablero. Merecida victoria.";

                case ModuleZRivalId.Andalucia:
                    return "Antonio ríe. Hoy la pieza Z te ha favorecido.";

                default:
                    return "Has ganado el duelo.";
            }
        }

        public static string GetPlayerDefeatMessage(
            ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Miguel gana el duelo de la plaza.";

                case ModuleZRivalId.Barcelona:
                    return "Jordi encontró el movimiento perfecto.";

                case ModuleZRivalId.Valencia:
                    return "Vicent cerró todos los caminos.";

                case ModuleZRivalId.Andalucia:
                    return "Antonio domina el tablero con experiencia.";

                default:
                    return "Has perdido el duelo.";
            }
        }

        public static string GetDrawMessage(
            ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Miguel sonríe. Ha sido un duelo equilibrado.";

                case ModuleZRivalId.Barcelona:
                    return "Jordi acepta el empate con respeto.";

                case ModuleZRivalId.Valencia:
                    return "Vicent observa el tablero en silencio.";

                case ModuleZRivalId.Andalucia:
                    return "Antonio dice que volveréis a enfrentaros.";

                default:
                    return "El duelo termina en empate.";
            }
        }
    }
}