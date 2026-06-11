using UnityEngine;

namespace ModuleZ.Duel.Themes.Madrid70s
{
    public class Madrid70sDuelArenaBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateArenaBase();
            CreateArenaBorder();
            CreatePlayerZone();
            CreateOpponentZone();
            CreatePuzzleZone();

            Debug.Log("[Module Z] Arena Duel Madrid años 70 creada.");
        }

        private void CreateArenaBase()
        {
            CreateCube(
                "Arena_Base_Adoquinada",
                new Vector3(0f, -0.1f, 0f),
                new Vector3(26f, 0.2f, 34f),
                new Color(0.48f, 0.45f, 0.40f)
            );
        }

        private void CreateArenaBorder()
        {
            CreateCube("Arena_Muro_Norte", new Vector3(0f, 0.6f, 17f), new Vector3(26f, 1.2f, 0.6f), new Color(0.35f, 0.30f, 0.25f));
            CreateCube("Arena_Muro_Sur", new Vector3(0f, 0.6f, -17f), new Vector3(26f, 1.2f, 0.6f), new Color(0.35f, 0.30f, 0.25f));
            CreateCube("Arena_Muro_Este", new Vector3(13f, 0.6f, 0f), new Vector3(0.6f, 1.2f, 34f), new Color(0.35f, 0.30f, 0.25f));
            CreateCube("Arena_Muro_Oeste", new Vector3(-13f, 0.6f, 0f), new Vector3(0.6f, 1.2f, 34f), new Color(0.35f, 0.30f, 0.25f));
        }

        private void CreatePlayerZone()
        {
            CreateCube(
                "Zona_Player",
                new Vector3(0f, 0.02f, -10f),
                new Vector3(10f, 0.08f, 6f),
                new Color(0.22f, 0.35f, 0.55f)
            );
        }

        private void CreateOpponentZone()
        {
            CreateCube(
                "Zona_Rivales",
                new Vector3(0f, 0.02f, 10f),
                new Vector3(10f, 0.08f, 6f),
                new Color(0.55f, 0.22f, 0.18f)
            );
        }

        private void CreatePuzzleZone()
        {
            CreateCube(
                "Zona_Puzzle_Z",
                new Vector3(0f, 0.04f, 0f),
                new Vector3(12f, 0.1f, 10f),
                new Color(0.40f, 0.38f, 0.34f)
            );

            CreateZPreview(new Vector3(0f, 0.25f, 0f));
        }

        private void CreateZPreview(Vector3 origin)
        {
            Color zColor = new Color(0.95f, 0.75f, 0.20f);

            CreateCube("PuzzleZ_Block_01", origin + new Vector3(-1f, 0f, 1f), Vector3.one, zColor);
            CreateCube("PuzzleZ_Block_02", origin + new Vector3(0f, 0f, 1f), Vector3.one, zColor);
            CreateCube("PuzzleZ_Block_03", origin + new Vector3(0f, 0f, 0f), Vector3.one, zColor);
            CreateCube("PuzzleZ_Block_04", origin + new Vector3(1f, 0f, 0f), Vector3.one, zColor);
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