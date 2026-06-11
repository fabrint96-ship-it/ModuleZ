using UnityEngine;

namespace ModuleZ.Duel.Themes.Valencia70s
{
    public class Valencia70sDuelLightingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateSun();
            ConfigureEnvironment();

            Debug.Log("[Module Z] Iluminación Duel Valencia años 70 creada.");
        }

        private void CreateSun()
        {
            GameObject sun = new GameObject("Duel_Sun_Valencia_70s");

            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.35f;
            light.color = new Color(1f, 0.92f, 0.70f);

            sun.transform.rotation = Quaternion.Euler(40f, -20f, 0f);
        }

        private void ConfigureEnvironment()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.48f, 0.36f);

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.70f, 0.62f, 0.48f);
            RenderSettings.fogDensity = 0.004f;
        }
    }
}