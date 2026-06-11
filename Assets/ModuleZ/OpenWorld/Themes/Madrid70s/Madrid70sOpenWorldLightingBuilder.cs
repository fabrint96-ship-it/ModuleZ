using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Madrid70s
{
    public class Madrid70sLightingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateSun();
            ConfigureAmbient();
            ConfigureFog();

            Debug.Log("[Module Z] Iluminación Madrid años 70 creada.");
        }

        private void CreateSun()
        {
            GameObject sun = new GameObject("Sun_Madrid_70s");

            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(1f, 0.92f, 0.78f);

            sun.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
        }

        private void ConfigureAmbient()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.45f, 0.42f, 0.36f);
        }

        private void ConfigureFog()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.56f, 0.50f, 0.42f);
            RenderSettings.fogDensity = 0.008f;
        }
    }
}