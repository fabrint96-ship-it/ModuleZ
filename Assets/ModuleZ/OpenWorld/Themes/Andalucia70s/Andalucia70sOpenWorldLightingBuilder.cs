using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Andalucia70s
{
    public class Andalucia70sOpenWorldLightingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateSun();
            ConfigureAmbient();
            ConfigureFog();

            Debug.Log("[Module Z] Iluminación OpenWorld Andalucía años 70 creada.");
        }

        private void CreateSun()
        {
            GameObject sun = new GameObject("Sun_Andalucia_70s");

            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.45f;
            light.color = new Color(1f, 0.93f, 0.72f);

            sun.transform.rotation = Quaternion.Euler(38f, -22f, 0f);
        }

        private void ConfigureAmbient()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.62f, 0.54f, 0.42f);
        }

        private void ConfigureFog()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.78f, 0.70f, 0.56f);
            RenderSettings.fogDensity = 0.0035f;
        }
    }
}