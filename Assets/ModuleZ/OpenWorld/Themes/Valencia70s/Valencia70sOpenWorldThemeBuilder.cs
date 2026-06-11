using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Valencia70s
{
    public class Valencia70sOpenWorldThemeBuilder : MonoBehaviour
    {
        public void Build()
        {
            gameObject.AddComponent<Valencia70sOpenWorldLightingBuilder>().Build();
            gameObject.AddComponent<Valencia70sOpenWorldGroundBuilder>().Build();
            gameObject.AddComponent<Valencia70sOpenWorldBuildingBuilder>().Build();
            gameObject.AddComponent<Valencia70sOpenWorldPropsBuilder>().Build();
            gameObject.AddComponent<Valencia70sOpenWorldNPCBuilder>().Build();

            Debug.Log("[Module Z] Theme OpenWorld Valencia años 70 construido.");
        }
    }
}