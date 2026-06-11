using UnityEngine;

namespace ModuleZ.OpenWorld.Builders
{
    [System.Serializable]
    public class ModuleZCityDecorationPreset
    {
        public Vector3[] benchPositions;
        public Vector3[] lampPositions;
        public Vector3[] planterPositions;
        public CitySignData[] signs;
    }

    [System.Serializable]
    public class CitySignData
    {
        public string name;
        public string text;
        public Vector3 position;
        public Vector3 scale;
        public Color color;
    }
}