using ModuleZ.Core.Managers;

namespace ModuleZ.OpenWorld.Encounters
{
    public static class ModuleZRivalProgression
    {
        public static bool IsRivalDefeated(ModuleZRivalId rivalId)
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

        public static bool IsRivalUnlocked(ModuleZRivalId rivalId)
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
                    return ModuleZGameState.RivalValenciaDefeated;

                default:
                    return false;
            }
        }

        public static void MarkRivalDefeated(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    ModuleZGameState.RivalMadridDefeated = true;
                    break;

                case ModuleZRivalId.Barcelona:
                    ModuleZGameState.RivalBarcelonaDefeated = true;
                    break;

                case ModuleZRivalId.Valencia:
                    ModuleZGameState.RivalValenciaDefeated = true;
                    break;

                case ModuleZRivalId.Andalucia:
                    ModuleZGameState.RivalAndaluciaDefeated = true;
                    ModuleZGameState.MainProgressionCompleted = true;
                    break;
            }

            UpdateUnlocks();
        }

        public static void UpdateUnlocks()
        {
            ModuleZGameState.AndaluciaUnlocked =
                ModuleZGameState.RivalValenciaDefeated;
        }

        public static string GetLockedMessage(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Barcelona:
                    return "Barcelona está bloqueada. Derrota antes al rival de Madrid.";

                case ModuleZRivalId.Valencia:
                    return "Valencia está bloqueada. Derrota antes al rival de Barcelona.";

                case ModuleZRivalId.Andalucia:
                    return "Andalucía está bloqueada. Derrota antes al rival de Valencia.";

                default:
                    return "Este rival todavía está bloqueado.";
            }
        }

        public static string GetUnlockMessage(ModuleZRivalId defeatedRival)
        {
            switch (defeatedRival)
            {
                case ModuleZRivalId.Madrid:
                    return "Nueva zona desbloqueada: Barcelona";

                case ModuleZRivalId.Barcelona:
                    return "Nueva zona desbloqueada: Valencia";

                case ModuleZRivalId.Valencia:
                    return "Nueva zona desbloqueada: Andalucía";

                case ModuleZRivalId.Andalucia:
                    return "Has completado la progresión principal de Module Z.";

                default:
                    return "";
            }
        }
    }
}