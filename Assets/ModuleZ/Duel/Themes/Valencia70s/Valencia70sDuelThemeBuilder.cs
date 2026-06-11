using UnityEngine;

namespace ModuleZ.Duel.Themes.Valencia70s
{
    public class Valencia70sDuelThemeBuilder : MonoBehaviour
    {
        public void Build()
        {
            gameObject.AddComponent<Valencia70sDuelLightingBuilder>().Build();
            gameObject.AddComponent<Valencia70sDuelArenaBuilder>().Build();
            gameObject.AddComponent<Valencia70sDuelPropsBuilder>().Build();
            gameObject.AddComponent<Valencia70sDuelOpponentBuilder>().Build();

            Debug.Log("[Module Z] Theme Duel Valencia años 70 construido.");
        }
    }
}