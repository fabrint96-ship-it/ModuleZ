using UnityEngine;

namespace ModuleZ.Duel.Themes.Barcelona70s
{
    public class Barcelona70sDuelThemeBuilder : MonoBehaviour
    {
        public void Build()
        {
            gameObject.AddComponent<Barcelona70sDuelLightingBuilder>().Build();
            gameObject.AddComponent<Barcelona70sDuelArenaBuilder>().Build();
            gameObject.AddComponent<Barcelona70sDuelPropsBuilder>().Build();
            gameObject.AddComponent<Barcelona70sDuelOpponentBuilder>().Build();

            Debug.Log("[Module Z] Theme Duel Barcelona años 70 construido.");
        }
    }
}