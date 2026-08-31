using ModuleZ.Core.Managers;

namespace ModuleZ.OpenWorld.Encounters
{
    public static class ModuleZRivalHUDTextLibrary
    {
        public static string GetRivalName(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Miguel de Madrid";

                case ModuleZRivalId.Barcelona:
                    return "Jordi de Barcelona";

                case ModuleZRivalId.Valencia:
                    return "Vicent de Valencia";

                case ModuleZRivalId.Andalucia:
                    return "Antonio de Andalucía";

                default:
                    return "Rival";
            }
        }

        public static string GetRivalDescription(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Equilibrado, observa antes de mover.";

                case ModuleZRivalId.Barcelona:
                    return "Rápido, agresivo y con mucho estilo.";

                case ModuleZRivalId.Valencia:
                    return "Paciente, defensivo y calculador.";

                case ModuleZRivalId.Andalucia:
                    return "Tranquilo, experto y difícil de vencer.";

                default:
                    return "Rival preparado para el duelo.";
            }
        }

        public static string GetRivalStatus(ModuleZRivalId rivalId)
        {
            if (IsDefeated(rivalId))
                return "DERROTADO";

            if (!IsUnlocked(rivalId))
                return "BLOQUEADO";

            return "DISPONIBLE";
        }

        public static string GetRivalAction(ModuleZRivalId rivalId)
        {
            if (IsDefeated(rivalId))
                return "Pulsa E para rematch";

            if (!IsUnlocked(rivalId))
                return "Derrota a los rivales anteriores";

            return "Pulsa E para duelo";
        }

        private static bool IsDefeated(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return ModuleZGameState.RivalMadridDefeated;

                case ModuleZRivalId.Barcelona:
                    return ModuleZGameState.RivalBarcelonaDefeated;

                case ModuleZRivalId.Valencia:
                    return ModuleZGameState.RivalValenciaDefeated;

                case ModuleZRivalId.Andalucia:
                    return ModuleZGameState.RivalAndaluciaDefeated;

                default:
                    return false;
            }
        }

        private static bool IsUnlocked(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return true;

                case ModuleZRivalId.Barcelona:
                    return ModuleZGameState.RivalMadridDefeated;

                case ModuleZRivalId.Valencia:
                    return ModuleZGameState.RivalBarcelonaDefeated;

                case ModuleZRivalId.Andalucia:
                    return ModuleZGameState.AndaluciaUnlocked;

                default:
                    return false;
            }
        }
    }
}