using ModuleZ.OpenWorld.Builders;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Madrid70s
{
    public class Madrid70sOpenWorldThemeBuilder : MonoBehaviour
    {
        public void Build()
        {
            gameObject.AddComponent<Madrid70sLightingBuilder>().Build();
            gameObject.AddComponent<Madrid70sGroundBuilder>().Build();
            gameObject.AddComponent<Madrid70sBuildingBuilder>().Build();
            gameObject.AddComponent<Madrid70sPropsBuilder>().Build();
            gameObject.AddComponent<Madrid70sOpenWorldNPCBuilder>().Build();

            Debug.Log("[Module Z] Theme OpenWorld Madrid años 70 construido.");
        }
    }
}