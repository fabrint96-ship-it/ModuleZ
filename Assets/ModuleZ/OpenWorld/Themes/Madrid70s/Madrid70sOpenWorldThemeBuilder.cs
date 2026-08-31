using ModuleZ.OpenWorld.Builders;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Madrid70s
{
    public class Madrid70sOpenWorldThemeBuilder : MonoBehaviour
    {
        public void Build()
        {
            gameObject.AddComponent<Madrid70sOpenWorldLightingBuilder>().Build();
            gameObject.AddComponent<Madrid70sOpenWorldGroundBuilder>().Build();
            gameObject.AddComponent<Madrid70sOpenWorldBuildingBuilder>().Build();
            gameObject.AddComponent<Madrid70sOpenWorldPropsBuilder>().Build();
            gameObject.AddComponent<OpenWorldRoadPropsBuilder>().Build();
            gameObject.AddComponent<OpenWorldBoundaryWallBuilder>().Build();
            gameObject.AddComponent<Madrid70sOpenWorldNPCBuilder>().Build();

            Debug.Log("[Module Z] Theme OpenWorld Madrid años 70 construido.");
        }
    }
}