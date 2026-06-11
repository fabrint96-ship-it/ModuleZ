using UnityEngine;

namespace ModuleZ.Duel.Themes.Madrid70s
{
    public class Madrid70sDuelLightingBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateSun();
            ConfigureEnvironment();

            Debug.Log("[Module Z] Iluminación Duel Madrid años 70 creada.");
        }

        private void CreateSun()
        {
            GameObject sun = new GameObject("Duel_Sun_Madrid_70s");

            Light light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.color = new Color(1f, 0.90f, 0.75f);

            sun.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
        }

        private void ConfigureEnvironment()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

            RenderSettings.ambientLight =
                new Color(0.42f, 0.38f, 0.32f);

            RenderSettings.fog = true;

            RenderSettings.fogColor =
                new Color(0.55f, 0.50f, 0.44f);

            RenderSettings.fogDensity = 0.006f;
        }
    }
}