using ModuleZ.OpenWorld.Encounters;

namespace ModuleZ.Duel3D.AI
{
    public static class Duel3DRivalContextCommentLibrary
    {
        public static string GetPlayerRecoveredComment(
            ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Miguel no esperaba esa recuperación.";

                case ModuleZRivalId.Barcelona:
                    return "Jordi vuelve a estudiar el tablero con atención.";

                case ModuleZRivalId.Valencia:
                    return "Vicent parece reconsiderar su estrategia.";

                case ModuleZRivalId.Andalucia:
                    return "Antonio sonríe. El duelo sigue abierto.";

                default:
                    return "El rival observa tu recuperación.";
            }
        }

        public static string GetOpponentRecoveredComment(
            ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Miguel vuelve a tomar la iniciativa.";

                case ModuleZRivalId.Barcelona:
                    return "Jordi recupera terreno rápidamente.";

                case ModuleZRivalId.Valencia:
                    return "Vicent vuelve a controlar la situación.";

                case ModuleZRivalId.Andalucia:
                    return "Antonio recupera la ventaja con experiencia.";

                default:
                    return "El rival recupera terreno.";
            }
        }
    }
}