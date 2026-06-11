using UnityEngine;

namespace ModuleZ.Duel.Themes.Barcelona70s
{
    public class Barcelona70sDuelLightingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateSun();
            ConfigureEnvironment();

            Debug.Log("[Module Z] Iluminación Duel Barcelona años 70 creada.");
        }

        private void CreateSun()
        {
            GameObject sun = new GameObject("Duel_Sun_Barcelona_70s");

            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            light.color = new Color(1f, 0.88f, 0.68f);

            sun.transform.rotation = Quaternion.Euler(42f, -25f, 0f);
        }

        private void ConfigureEnvironment()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.48f, 0.42f, 0.36f);

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogColor = new Color(0.62f, 0.54f, 0.46f);
            RenderSettings.fogDensity = 0.005f;
        }
    }
}