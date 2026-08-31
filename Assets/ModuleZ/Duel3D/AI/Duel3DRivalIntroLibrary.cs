using ModuleZ.OpenWorld.Encounters;

namespace ModuleZ.Duel3D.AI
{
    public static class Duel3DRivalIntroLibrary
    {
        public static string GetIntro(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Miguel observa la plaza antes de mover ficha.";

                case ModuleZRivalId.Barcelona:
                    return "Jordi juega rápido, como si conociera cada esquina del tablero.";

                case ModuleZRivalId.Valencia:
                    return "Vicent espera, calcula y busca cerrarte el camino.";

                case ModuleZRivalId.Andalucia:
                    return "Antonio sonríe tranquilo. Este duelo no será fácil.";

                default:
                    return "El rival se prepara para el duelo.";
            }
        }
    }
}