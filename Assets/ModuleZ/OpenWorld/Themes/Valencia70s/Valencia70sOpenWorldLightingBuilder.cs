using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Valencia70s
{
    public class Valencia70sOpenWorldLightingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateSun();
            ConfigureAmbient();
            ConfigureFog();

            Debug.Log("[Module Z] Iluminación OpenWorld Valencia años 70 creada.");
        }

        private void CreateSun()
        {
            GameObject sun = new GameObject("Sun_Valencia_70s");

            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.35f;
            light.color = new Color(1f, 0.92f, 0.70f);

            sun.transform.rotation = Quaternion.Euler(40f, -20f, 0f);
        }

        private void ConfigureAmbient()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.48f, 0.36f);
        }

        private void ConfigureFog()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.70f, 0.62f, 0.48f);
            RenderSettings.fogDensity = 0.004f;
        }
    }
}