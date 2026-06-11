using UnityEngine;

namespace ModuleZ.Duel.Themes.Andalucia70s
{
    public class Andalucia70sDuelArenaBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateArenaBase();
            CreateArenaBorder();
            CreatePlayerZone();
            CreateOpponentZone();
            CreatePuzzleZone();
            CreatePatioPattern();

            Debug.Log("[Module Z] Arena Duel Andalucía años 70 creada.");
        }

        private void CreateArenaBase()
        {
            CreateCube(
                "Andalucia_Arena_Base",
                new Vector3(0f, -0.1f, 0f),
                new Vector3(28f, 0.2f, 36f),
                new Color(0.88f, 0.84f, 0.72f)
            );
        }

        private void CreateArenaBorder()
        {
            Color wallColor = new Color(0.92f, 0.90f, 0.82f);

            CreateCube("Andalucia_Muro_Norte", new Vector3(0f, 1f, 18f), new Vector3(28f, 2f, 0.6f), wallColor);
            CreateCube("Andalucia_Muro_Sur", new Vector3(0f, 1f, -18f), new Vector3(28f, 2f, 0.6f), wallColor);
            CreateCube("Andalucia_Muro_Este", new Vector3(14f, 1f, 0f), new Vector3(0.6f, 2f, 36f), wallColor);
            CreateCube("Andalucia_Muro_Oeste", new Vector3(-14f, 1f, 0f), new Vector3(0.6f, 2f, 36f), wallColor);
        }

        private void CreatePlayerZone()
        {
            CreateCube(
                "Andalucia_PlayerZone",
                new Vector3(0f, 0.03f, -11f),
                new Vector3(12f, 0.08f, 6f),
                new Color(0.20f, 0.42f, 0.75f)
            );
        }

        private void CreateOpponentZone()
        {
            CreateCube(
                "Andalucia_OpponentZone",
                new Vector3(0f, 0.03f, 11f),
                new Vector3(12f, 0.08f, 6f),
                new Color(0.78f, 0.32f, 0.18f)
            );
        }

        private void CreatePuzzleZone()
        {
            CreateCube(
                "Andalucia_PuzzleZone",
                new Vector3(0f, 0.04f, 0f),
                new Vector3(14f, 0.1f, 12f),
                new Color(0.96f, 0.91f, 0.78f)
            );
        }

        private void CreatePatioPattern()
        {
            Color tileColor = new Color(0.18f, 0.45f, 0.75f);

            for (int x = -5; x <= 5; x += 2)
            {
                for (int z = -4; z <= 4; z += 2)
                {
                    CreateCube(
                        "Andalucia_Azulejo",
                        new Vector3(x, 0.13f, z),
                        new Vector3(0.65f, 0.025f, 0.65f),
                        tileColor
                    );
                }
            }
        }

        private GameObject CreateCube(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.position = position;
            cube.transform.localScale = scale;

            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material.color = color;

            return cube;
        }
    }
}