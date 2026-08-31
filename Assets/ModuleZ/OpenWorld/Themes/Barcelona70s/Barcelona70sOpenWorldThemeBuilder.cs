using ModuleZ.OpenWorld.Builders;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Barcelona70s
{
    public class Barcelona70sOpenWorldThemeBuilder : MonoBehaviour
    {
        public void Build()
        {
            gameObject.AddComponent<Barcelona70sOpenWorldLightingBuilder>().Build();
            gameObject.AddComponent<Barcelona70sOpenWorldGroundBuilder>().Build();
            gameObject.AddComponent<Barcelona70sOpenWorldBuildingBuilder>().Build();
            gameObject.AddComponent<Barcelona70sOpenWorldPropsBuilder>().Build();
            gameObject.AddComponent<OpenWorldRoadPropsBuilder>().Build();
            gameObject.AddComponent<OpenWorldBoundaryWallBuilder>().Build();
            gameObject.AddComponent<Barcelona70sOpenWorldNPCBuilder>().Build();

            Debug.Log("[Module Z] Theme OpenWorld Barcelona años 70 construido.");
        }
    }
}