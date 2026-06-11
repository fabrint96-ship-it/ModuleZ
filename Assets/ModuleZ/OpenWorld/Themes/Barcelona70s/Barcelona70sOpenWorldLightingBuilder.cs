using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Barcelona70s
{
    public class Barcelona70sOpenWorldLightingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateSun();
            ConfigureAmbient();
            ConfigureFog();

            Debug.Log("[Module Z] Iluminación OpenWorld Barcelona años 70 creada.");
        }

        private void CreateSun()
        {
            GameObject sun = new GameObject("Sun_Barcelona_70s");

            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            light.color = new Color(1f, 0.88f, 0.68f);

            sun.transform.rotation = Quaternion.Euler(42f, -25f, 0f);
        }

        private void ConfigureAmbient()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.48f, 0.42f, 0.36f);
        }

        private void ConfigureFog()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.62f, 0.54f, 0.46f);
            RenderSettings.fogDensity = 0.005f;
        }
    }
}