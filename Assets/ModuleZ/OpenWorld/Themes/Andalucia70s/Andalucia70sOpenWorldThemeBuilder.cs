using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Andalucia70s
{
    public class Andalucia70sOpenWorldThemeBuilder : MonoBehaviour
    {
        public void Build()
        {
            gameObject.AddComponent<Andalucia70sOpenWorldLightingBuilder>().Build();
            gameObject.AddComponent<Andalucia70sOpenWorldGroundBuilder>().Build();
            gameObject.AddComponent<Andalucia70sOpenWorldBuildingBuilder>().Build();
            gameObject.AddComponent<Andalucia70sOpenWorldPropsBuilder>().Build();
            gameObject.AddComponent<Andalucia70sOpenWorldNPCBuilder>().Build();

            Debug.Log("[Module Z] Theme OpenWorld Andalucía años 70 construido.");
        }
    }
}