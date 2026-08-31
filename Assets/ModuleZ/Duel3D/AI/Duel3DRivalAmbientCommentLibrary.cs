using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Duel3D.AI
{
    public static class Duel3DRivalAmbientCommentLibrary
    {
        public static string GetRandomComment(ModuleZRivalId rivalId)
        {
            string[] comments = GetComments(rivalId);

            if (comments == null || comments.Length == 0)
                return "";

            return comments[Random.Range(0, comments.Length)];
        }

        private static string[] GetComments(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return new string[]
                    {
                        "Miguel mira el tablero como quien mira la plaza al atardecer.",
                        "Miguel dice: en Madrid conviene pensar antes de moverse.",
                        "Miguel espera tu jugada con calma de barrio."
                    };

                case ModuleZRivalId.Barcelona:
                    return new string[]
                    {
                        "Jordi no pierde tiempo y estudia cada hueco.",
                        "Jordi dice: una buena jugada también tiene estilo.",
                        "Jordi parece buscar presión desde el primer momento."
                    };

                case ModuleZRivalId.Valencia:
                    return new string[]
                    {
                        "Vicent observa el tablero como quien revisa un puesto del mercado.",
                        "Vicent dice: la paciencia también gana duelos.",
                        "Vicent espera el momento justo para cerrarte el paso."
                    };

                case ModuleZRivalId.Andalucia:
                    return new string[]
                    {
                        "Antonio sonríe tranquilo bajo el calor de la plaza.",
                        "Antonio dice: no corras, que el tablero habla solo.",
                        "Antonio parece tener más experiencia de la que muestra."
                    };

                default:
                    return new string[]
                    {
                        "El rival observa el tablero.",
                        "El rival prepara su siguiente jugada."
                    };
            }
        }
    }
}