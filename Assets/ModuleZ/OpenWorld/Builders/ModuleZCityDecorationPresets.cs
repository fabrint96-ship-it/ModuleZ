using UnityEngine;

namespace ModuleZ.OpenWorld.Builders
{
    public static class ModuleZCityDecorationPresets
    {
        public static ModuleZCityDecorationPreset MadridPlaza()
        {
            return new ModuleZCityDecorationPreset
            {
                benchPositions = new Vector3[]
                {
                    new Vector3(-8f, 0.35f, 8.25f),
                    new Vector3(8f, 0.35f, 8.25f)
                },

                lampPositions = new Vector3[]
                {
                    new Vector3(-10f, 0f, 10f),
                    new Vector3(10f, 0f, 10f)
                },

                planterPositions = new Vector3[]
                {
                    new Vector3(-5f, 0.45f, 6f),
                    new Vector3(5f, 0.45f, 6f)
                },

                signs = new CitySignData[]
                {
                    new CitySignData
                    {
                        name = "Cartel_Madrid_Entrada",
                        text = "MADRID 70s",
                        position = new Vector3(0f, 2.5f, 25.5f),
                        scale = new Vector3(8f, 1.1f, 0.12f),
                        color = new Color(0.95f, 0.75f, 0.20f)
                    },

                    new CitySignData
                    {
                        name = "Cartel_Madrid_Plaza",
                        text = "PLAZA CENTRAL",
                        position = new Vector3(0f, 2.0f, 5f),
                        scale = new Vector3(5f, 0.8f, 0.12f),
                        color = new Color(0.90f, 0.85f, 0.40f)
                    },

                    new CitySignData
                    {
                        name = "Cartel_Madrid_Duelo",
                        text = "ARENA DE DUELO",
                        position = new Vector3(0f, 2.0f, -18f),
                        scale = new Vector3(6f, 0.8f, 0.12f),
                        color = new Color(0.90f, 0.35f, 0.25f)
                    }
                }
            };
        }

        public static ModuleZCityDecorationPreset BarcelonaPlaza()
        {
            return new ModuleZCityDecorationPreset
            {
                benchPositions = new Vector3[]
                {
                    new Vector3(-7f, 0.35f, 7f),
                    new Vector3(7f, 0.35f, 7f)
                },

                lampPositions = new Vector3[]
                {
                    new Vector3(-10f, 0f, 10f),
                    new Vector3(10f, 0f, 10f)
                },

                planterPositions = new Vector3[]
                {
                    new Vector3(-4f, 0.45f, 4f),
                    new Vector3(4f, 0.45f, 4f)
                },

                signs = new CitySignData[]
                {
                    new CitySignData
                    {
                        name = "Cartel_Barcelona_Entrada",
                        text = "BARCELONA 70s",
                        position = new Vector3(0f, 2.5f, 25.5f),
                        scale = new Vector3(8f, 1.1f, 0.12f),
                        color = new Color(0.20f, 0.55f, 0.85f)
                    },

                    new CitySignData
                    {
                        name = "Cartel_Barcelona_Plaza",
                        text = "RAMBLA CENTRAL",
                        position = new Vector3(0f, 2.0f, 5f),
                        scale = new Vector3(5f, 0.8f, 0.12f),
                        color = new Color(0.35f, 0.65f, 0.95f)
                    },

                    new CitySignData
                    {
                        name = "Cartel_Barcelona_Duelo",
                        text = "Z DUEL ARENA",
                        position = new Vector3(0f, 2.0f, -18f),
                        scale = new Vector3(6f, 0.8f, 0.12f),
                        color = new Color(0.95f, 0.50f, 0.20f)
                    }
                }
            };
        }

        public static ModuleZCityDecorationPreset ValenciaPlaza()
        {
            return new ModuleZCityDecorationPreset
            {
                benchPositions = new Vector3[]
                {
                    new Vector3(-7f, 0.35f, 8f),
                    new Vector3(7f, 0.35f, 8f),
                    new Vector3(-7f, 0.35f, -8f),
                    new Vector3(7f, 0.35f, -8f)
                },

                lampPositions = new Vector3[]
                {
                    new Vector3(-12f, 0f, -12f),
                    new Vector3(12f, 0f, -12f),
                    new Vector3(-12f, 0f, 12f),
                    new Vector3(12f, 0f, 12f)
                },

                planterPositions = new Vector3[]
                {
                    new Vector3(-6f, 0.45f, 6f),
                    new Vector3(6f, 0.45f, 6f)
                },

                signs = new CitySignData[]
                {
                    new CitySignData
                    {
                        name = "Cartel_Valencia_Entrada",
                        text = "VALENCIA 70s",
                        position = new Vector3(0f, 2.6f, 27.5f),
                        scale = new Vector3(8f, 1.1f, 0.12f),
                        color = new Color(0.95f, 0.62f, 0.20f)
                    },

                    new CitySignData
                    {
                        name = "Cartel_Valencia_Plaza",
                        text = "PLAZA DEL SOL",
                        position = new Vector3(0f, 2.0f, 6f),
                        scale = new Vector3(5f, 0.8f, 0.12f),
                        color = new Color(1.00f, 0.75f, 0.25f)
                    },

                    new CitySignData
                    {
                        name = "Cartel_Valencia_Duelo",
                        text = "ARENA Z VALENCIA",
                        position = new Vector3(0f, 2.0f, -20f),
                        scale = new Vector3(6f, 0.8f, 0.12f),
                        color = new Color(0.95f, 0.40f, 0.15f)
                    }
                }
            };
        }

        public static ModuleZCityDecorationPreset AndaluciaPlaza()
        {
            return new ModuleZCityDecorationPreset
            {
                benchPositions = new Vector3[]
                {
                    new Vector3(-8f, 0.35f, -10f),
                    new Vector3(8f, 0.35f, -10f),
                    new Vector3(-8f, 0.35f, 10f),
                    new Vector3(8f, 0.35f, 10f)
                },

                lampPositions = new Vector3[]
                {
                    new Vector3(-12f, 0f, -12f),
                    new Vector3(12f, 0f, -12f),
                    new Vector3(-12f, 0f, 12f),
                    new Vector3(12f, 0f, 12f)
                },

                planterPositions = new Vector3[]
                {
                    new Vector3(-13f, 0.45f, -5f),
                    new Vector3(13f, 0.45f, 5f),
                    new Vector3(-13f, 0.45f, 5f),
                    new Vector3(13f, 0.45f, -5f)
                },

                signs = new CitySignData[]
                {
                    new CitySignData
                    {
                        name = "Cartel_Andalucia_Entrada",
                        text = "ANDALUCIA 70s",
                        position = new Vector3(0f, 2.6f, -23.5f),
                        scale = new Vector3(8f, 1.1f, 0.12f),
                        color = new Color(0.90f, 0.72f, 0.28f)
                    },

                    new CitySignData
                    {
                        name = "Cartel_Andalucia_Plaza",
                        text = "PATIO CENTRAL",
                        position = new Vector3(0f, 2.0f, 8f),
                        scale = new Vector3(5f, 0.8f, 0.12f),
                        color = new Color(0.95f, 0.82f, 0.35f)
                    },

                    new CitySignData
                    {
                        name = "Cartel_Andalucia_Duelo",
                        text = "ARENA FINAL",
                        position = new Vector3(0f, 2.0f, -20f),
                        scale = new Vector3(6f, 0.8f, 0.12f),
                        color = new Color(0.90f, 0.25f, 0.15f)
                    }
                }
            };
        }
    }
}