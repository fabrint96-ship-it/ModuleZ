using ModuleZ.Core.Managers;
using ModuleZ.Core.Theme;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Duel3D.Visuals
{
    public class Duel3DCityArenaBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateBaseArena();
            CreateCityTheme();

            Debug.Log("[ModuleZ] Arena Duel3D España 70s creada.");
        }

        private void CreateBaseArena()
        {
            CreateCube(
                "Duel3D_Arena_Floor",
                new Vector3(0f, -0.08f, 0f),
                new Vector3(18f, 0.16f, 18f),
                ModuleZ70sPalette.WarmPaper
            );

            CreateCube(
                "Duel3D_Arena_Border_North",
                new Vector3(0f, 0.25f, 9.2f),
                new Vector3(18.5f, 0.5f, 0.35f),
                ModuleZ70sPalette.WoodBrown
            );

            CreateCube(
                "Duel3D_Arena_Border_South",
                new Vector3(0f, 0.25f, -9.2f),
                new Vector3(18.5f, 0.5f, 0.35f),
                ModuleZ70sPalette.WoodBrown
            );

            CreateCube(
                "Duel3D_Arena_Border_East",
                new Vector3(9.2f, 0.25f, 0f),
                new Vector3(0.35f, 0.5f, 18.5f),
                ModuleZ70sPalette.WoodBrown
            );

            CreateCube(
                "Duel3D_Arena_Border_West",
                new Vector3(-9.2f, 0.25f, 0f),
                new Vector3(0.35f, 0.5f, 18.5f),
                ModuleZ70sPalette.WoodBrown
            );
        }

        private void CreateCityTheme()
        {
            switch (ModuleZGameState.CurrentDuelRival)
            {
                case ModuleZRivalId.Madrid:
                    CreateMadridArena();
                    break;

                case ModuleZRivalId.Barcelona:
                    CreateBarcelonaArena();
                    break;

                case ModuleZRivalId.Valencia:
                    CreateValenciaArena();
                    break;

                case ModuleZRivalId.Andalucia:
                    CreateAndaluciaArena();
                    break;
            }
        }

        private void CreateMadridArena()
        {
            CreateCube("Madrid_Pared_Plaza", new Vector3(0f, 2.5f, 10.5f), new Vector3(16f, 5f, 0.4f), ModuleZ70sPalette.WarmPaper);
            CreateCube("Madrid_Ventana_Izq", new Vector3(-4f, 3f, 10.25f), new Vector3(1.2f, 1f, 0.12f), ModuleZ70sPalette.FadedBlue);
            CreateCube("Madrid_Ventana_Der", new Vector3(4f, 3f, 10.25f), new Vector3(1.2f, 1f, 0.12f), ModuleZ70sPalette.FadedBlue);

            CreateCube("Madrid_Cabina", new Vector3(-7f, 1.2f, 7.5f), new Vector3(1.1f, 2.4f, 1.1f), ModuleZ70sPalette.CabinaRed);
            CreateCube("Madrid_Banco_Asiento", new Vector3(6f, 0.45f, 7.2f), new Vector3(2f, 0.25f, 0.6f), ModuleZ70sPalette.WoodBrown);
            CreateCube("Madrid_Banco_Respaldo", new Vector3(6f, 0.9f, 7.45f), new Vector3(2f, 0.8f, 0.18f), ModuleZ70sPalette.WoodBrown);
        }

        private void CreateBarcelonaArena()
        {
            CreateCube("Barcelona_Pared_Barrio", new Vector3(0f, 2.5f, 10.5f), new Vector3(16f, 5f, 0.4f), ModuleZ70sPalette.Cream);
            CreateCube("Barcelona_Mosaico_Azul", new Vector3(-3.5f, 2.8f, 10.25f), new Vector3(1.2f, 1.2f, 0.12f), ModuleZ70sPalette.FadedBlue);
            CreateCube("Barcelona_Mosaico_Rojo", new Vector3(0f, 2.8f, 10.25f), new Vector3(1.2f, 1.2f, 0.12f), ModuleZ70sPalette.CabinaRed);
            CreateCube("Barcelona_Mosaico_Crema", new Vector3(3.5f, 2.8f, 10.25f), new Vector3(1.2f, 1.2f, 0.12f), ModuleZ70sPalette.WarmPaper);

            CreatePalmDecoration(new Vector3(-7f, 0f, 7f));
            CreatePalmDecoration(new Vector3(7f, 0f, 7f));
        }

        private void CreateValenciaArena()
        {
            CreateCube("Valencia_Mercado_Fondo", new Vector3(0f, 2.5f, 10.5f), new Vector3(16f, 5f, 0.4f), ModuleZ70sPalette.Cream);
            CreateCube("Valencia_Cartel_Mercado", new Vector3(0f, 3.4f, 10.25f), new Vector3(5f, 0.7f, 0.12f), ModuleZ70sPalette.Orange);

            CreateOrangeTreeDecoration(new Vector3(-6.5f, 0f, 7f));
            CreateOrangeTreeDecoration(new Vector3(6.5f, 0f, 7f));
        }

        private void CreateAndaluciaArena()
        {
            CreateCube("Andalucia_Pared_Blanca", new Vector3(0f, 2.5f, 10.5f), new Vector3(16f, 5f, 0.4f), ModuleZ70sPalette.Cream);
            CreateCube("Andalucia_Friso_Azul", new Vector3(0f, 3.8f, 10.25f), new Vector3(12f, 0.35f, 0.12f), ModuleZ70sPalette.FadedBlue);

            CreateArchDecoration(new Vector3(-4.5f, 0f, 7f));
            CreateArchDecoration(new Vector3(0f, 0f, 7f));
            CreateArchDecoration(new Vector3(4.5f, 0f, 7f));

            CreateFlowerPot(new Vector3(-7f, 0f, 6.5f));
            CreateFlowerPot(new Vector3(7f, 0f, 6.5f));
        }

        private void CreatePalmDecoration(Vector3 basePosition)
        {
            CreateCube("Duel_Palm_Trunk", basePosition + new Vector3(0f, 1.4f, 0f), new Vector3(0.35f, 2.8f, 0.35f), ModuleZ70sPalette.WoodBrown);

            Vector3 top = basePosition + new Vector3(0f, 2.9f, 0f);

            CreateCube("Duel_Palm_Leaf_A", top + new Vector3(0.7f, 0f, 0f), new Vector3(1.5f, 0.2f, 0.35f), ModuleZ70sPalette.OliveGreen);
            CreateCube("Duel_Palm_Leaf_B", top + new Vector3(-0.7f, 0f, 0f), new Vector3(1.5f, 0.2f, 0.35f), ModuleZ70sPalette.OliveGreen);
            CreateCube("Duel_Palm_Leaf_C", top + new Vector3(0f, 0f, 0.7f), new Vector3(0.35f, 0.2f, 1.5f), ModuleZ70sPalette.OliveGreen);
            CreateCube("Duel_Palm_Leaf_D", top + new Vector3(0f, 0f, -0.7f), new Vector3(0.35f, 0.2f, 1.5f), ModuleZ70sPalette.OliveGreen);
        }

        private void CreateOrangeTreeDecoration(Vector3 basePosition)
        {
            CreateCube("Duel_OrangeTree_Trunk", basePosition + new Vector3(0f, 0.9f, 0f), new Vector3(0.3f, 1.8f, 0.3f), ModuleZ70sPalette.WoodBrown);
            CreateCube("Duel_OrangeTree_Crown", basePosition + new Vector3(0f, 2f, 0f), new Vector3(1.5f, 1.1f, 1.5f), ModuleZ70sPalette.OliveGreen);
            CreateCube("Duel_Orange_A", basePosition + new Vector3(0.4f, 2f, -0.25f), new Vector3(0.2f, 0.2f, 0.2f), ModuleZ70sPalette.Orange);
            CreateCube("Duel_Orange_B", basePosition + new Vector3(-0.3f, 2.15f, 0.25f), new Vector3(0.2f, 0.2f, 0.2f), ModuleZ70sPalette.Orange);
        }

        private void CreateArchDecoration(Vector3 basePosition)
        {
            CreateCube("Duel_Arch_Left", basePosition + new Vector3(-0.8f, 1.2f, 0f), new Vector3(0.25f, 2.4f, 0.25f), ModuleZ70sPalette.Cream);
            CreateCube("Duel_Arch_Right", basePosition + new Vector3(0.8f, 1.2f, 0f), new Vector3(0.25f, 2.4f, 0.25f), ModuleZ70sPalette.Cream);
            CreateCube("Duel_Arch_Top", basePosition + new Vector3(0f, 2.5f, 0f), new Vector3(1.9f, 0.25f, 0.25f), ModuleZ70sPalette.Cream);
        }

        private void CreateFlowerPot(Vector3 basePosition)
        {
            CreateCube("Duel_FlowerPot", basePosition + new Vector3(0f, 0.35f, 0f), new Vector3(0.8f, 0.7f, 0.8f), ModuleZ70sPalette.Orange);
            CreateCube("Duel_FlowerPlant", basePosition + new Vector3(0f, 0.9f, 0f), new Vector3(0.6f, 0.5f, 0.6f), ModuleZ70sPalette.OliveGreen);
            CreateCube("Duel_Flower", basePosition + new Vector3(0f, 1.2f, 0f), new Vector3(0.25f, 0.2f, 0.25f), ModuleZ70sPalette.CabinaRed);
        }

        private GameObject CreateCube(
            string name,
            Vector3 position,
            Vector3 scale,
            Color color)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(transform, false);
            obj.transform.position = position;
            obj.transform.localScale = scale;

            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material.color = color;

            return obj;
        }
    }
}