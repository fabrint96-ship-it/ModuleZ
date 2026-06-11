using UnityEngine;

namespace ModuleZ.Duel.Themes.Madrid70s
{
    public class Madrid70sDuelThemeBuilder : MonoBehaviour
    {
        public void Build()
        {
            gameObject.AddComponent<Madrid70sDuelLightingBuilder>().Build();
            gameObject.AddComponent<Madrid70sDuelArenaBuilder>().Build();
            gameObject.AddComponent<Madrid70sDuelPropsBuilder>().Build();
            gameObject.AddComponent<Madrid70sDuelOpponentBuilder>().Build();

            Debug.Log("[Module Z] Theme Duel Madrid años 70 construido.");
        }
    }
}