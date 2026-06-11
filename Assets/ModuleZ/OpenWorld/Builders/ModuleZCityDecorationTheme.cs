using UnityEngine;

namespace ModuleZ.OpenWorld.Builders
{
    [System.Serializable]
    public class ModuleZCityDecorationTheme
    {
        public Color benchSeatColor;
        public Color benchBackColor;

        public Color lampPostColor;
        public Color lampLightColor;

        public Color planterBaseColor;
        public Color plantColor;

        public static ModuleZCityDecorationTheme Madrid70s()
        {
            return new ModuleZCityDecorationTheme
            {
                benchSeatColor = new Color(0.30f, 0.15f, 0.06f),
                benchBackColor = new Color(0.24f, 0.10f, 0.04f),
                lampPostColor = new Color(0.15f, 0.15f, 0.15f),
                lampLightColor = new Color(1f, 0.9f, 0.5f),
                planterBaseColor = new Color(0.55f, 0.30f, 0.16f),
                plantColor = new Color(0.10f, 0.45f, 0.18f)
            };
        }

        public static ModuleZCityDecorationTheme Barcelona70s()
        {
            return new ModuleZCityDecorationTheme
            {
                benchSeatColor = new Color(0.35f, 0.18f, 0.08f),
                benchBackColor = new Color(0.28f, 0.12f, 0.05f),
                lampPostColor = new Color(0.05f, 0.05f, 0.06f),
                lampLightColor = new Color(1f, 0.78f, 0.42f),
                planterBaseColor = new Color(0.55f, 0.36f, 0.22f),
                plantColor = new Color(0.12f, 0.45f, 0.18f)
            };
        }

        public static ModuleZCityDecorationTheme Valencia70s()
        {
            return new ModuleZCityDecorationTheme
            {
                benchSeatColor = new Color(0.42f, 0.22f, 0.08f),
                benchBackColor = new Color(0.32f, 0.14f, 0.04f),
                lampPostColor = new Color(0.08f, 0.07f, 0.05f),
                lampLightColor = new Color(1f, 0.85f, 0.45f),
                planterBaseColor = new Color(0.65f, 0.38f, 0.18f),
                plantColor = new Color(0.12f, 0.48f, 0.18f)
            };
        }

        public static ModuleZCityDecorationTheme Andalucia70s()
        {
            return new ModuleZCityDecorationTheme
            {
                benchSeatColor = new Color(0.32f, 0.15f, 0.06f),
                benchBackColor = new Color(0.24f, 0.10f, 0.04f),
                lampPostColor = new Color(0.04f, 0.04f, 0.04f),
                lampLightColor = new Color(1f, 0.80f, 0.45f),
                planterBaseColor = new Color(0.65f, 0.28f, 0.12f),
                plantColor = new Color(0.10f, 0.45f, 0.18f)
            };
        }
    }
}