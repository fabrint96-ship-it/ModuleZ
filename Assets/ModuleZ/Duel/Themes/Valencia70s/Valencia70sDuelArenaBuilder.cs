using UnityEngine;

namespace ModuleZ.Duel.Themes.Valencia70s
{
    public class Valencia70sDuelArenaBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateArenaBase();
            CreateArenaBorder();
            CreatePlayerZone();
            CreateOpponentZone();
            CreatePuzzleZone();

            Debug.Log("[Module Z] Arena Duel Valencia años 70 creada.");
        }

        private void CreateArenaBase()
        {
            CreateCube(
                "Valencia_Arena_Base",
                new Vector3(0f, -0.1f, 0f),
                new Vector3(28f, 0.2f, 36f),
                new Color(0.82f, 0.74f, 0.58f)
            );
        }

        private void CreateArenaBorder()
        {
            Color borderColor = new Color(0.68f, 0.42f, 0.18f);

            CreateCube("Valencia_Muro_Norte", new Vector3(0f, 0.7f, 18f), new Vector3(28f, 1.4f, 0.6f), borderColor);
            CreateCube("Valencia_Muro_Sur", new Vector3(0f, 0.7f, -18f), new Vector3(28f, 1.4f, 0.6f), borderColor);
            CreateCube("Valencia_Muro_Este", new Vector3(14f, 0.7f, 0f), new Vector3(0.6f, 1.4f, 36f), borderColor);
            CreateCube("Valencia_Muro_Oeste", new Vector3(-14f, 0.7f, 0f), new Vector3(0.6f, 1.4f, 36f), borderColor);
        }

        private void CreatePlayerZone()
        {
            CreateCube(
                "Valencia_PlayerZone",
                new Vector3(0f, 0.03f, -11f),
                new Vector3(12f, 0.08f, 6f),
                new Color(0.18f, 0.45f, 0.75f)
            );
        }

        private void CreateOpponentZone()
        {
            CreateCube(
                "Valencia_OpponentZone",
                new Vector3(0f, 0.03f, 11f),
                new Vector3(12f, 0.08f, 6f),
                new Color(0.85f, 0.45f, 0.15f)
            );
        }

        private void CreatePuzzleZone()
        {
            CreateCube(
                "Valencia_PuzzleZone",
                new Vector3(0f, 0.04f, 0f),
                new Vector3(14f, 0.1f, 12f),
                new Color(0.92f, 0.86f, 0.72f)
            );

            CreateDecorativeTiles();
        }

        private void CreateDecorativeTiles()
        {
            Color tileColor = new Color(0.95f, 0.55f, 0.15f);

            for (int z = -4; z <= 4; z += 2)
            {
                CreateCube(
                    "Valencia_Tile",
                    new Vector3(0f, 0.12f, z),
                    new Vector3(1f, 0.02f, 1f),
                    tileColor
                );
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