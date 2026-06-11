using UnityEngine;

namespace ModuleZ.Duel.Themes.Andalucia70s
{
    public class Andalucia70sDuelThemeBuilder : MonoBehaviour
    {
        public void Build()
        {
            gameObject.AddComponent<Andalucia70sDuelLightingBuilder>().Build();
            gameObject.AddComponent<Andalucia70sDuelArenaBuilder>().Build();
            gameObject.AddComponent<Andalucia70sDuelPropsBuilder>().Build();
            gameObject.AddComponent<Andalucia70sDuelOpponentBuilder>().Build();

            Debug.Log("[Module Z] Theme Duel Andalucía años 70 construido.");
        }
    }
}