using UnityEngine;

namespace ModuleZ.Duel.Themes.Barcelona70s
{
    public class Barcelona70sDuelArenaBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateArenaBase();
            CreateArenaBorder();
            CreatePlayerZone();
            CreateOpponentZone();
            CreatePuzzleZone();

            Debug.Log("[Module Z] Arena Duel Barcelona años 70 creada.");
        }

        private void CreateArenaBase()
        {
            CreateCube(
                "Barcelona_Arena_Base",
                new Vector3(0f, -0.1f, 0f),
                new Vector3(26f, 0.2f, 34f),
                new Color(0.78f, 0.72f, 0.62f)
            );
        }

        private void CreateArenaBorder()
        {
            Color borderColor = new Color(0.55f, 0.45f, 0.35f);

            CreateCube("Barcelona_Muro_Norte", new Vector3(0f, 0.6f, 17f), new Vector3(26f, 1.2f, 0.6f), borderColor);
            CreateCube("Barcelona_Muro_Sur", new Vector3(0f, 0.6f, -17f), new Vector3(26f, 1.2f, 0.6f), borderColor);
            CreateCube("Barcelona_Muro_Este", new Vector3(13f, 0.6f, 0f), new Vector3(0.6f, 1.2f, 34f), borderColor);
            CreateCube("Barcelona_Muro_Oeste", new Vector3(-13f, 0.6f, 0f), new Vector3(0.6f, 1.2f, 34f), borderColor);
        }

        private void CreatePlayerZone()
        {
            CreateCube(
                "Barcelona_PlayerZone",
                new Vector3(0f, 0.02f, -10f),
                new Vector3(10f, 0.08f, 6f),
                new Color(0.15f, 0.45f, 0.75f)
            );
        }

        private void CreateOpponentZone()
        {
            CreateCube(
                "Barcelona_OpponentZone",
                new Vector3(0f, 0.02f, 10f),
                new Vector3(10f, 0.08f, 6f),
                new Color(0.75f, 0.30f, 0.20f)
            );
        }

        private void CreatePuzzleZone()
        {
            CreateCube(
                "Barcelona_PuzzleZone",
                new Vector3(0f, 0.04f, 0f),
                new Vector3(12f, 0.1f, 10f),
                new Color(0.85f, 0.80f, 0.70f)
            );

            CreateMosaicDecoration();
        }

        private void CreateMosaicDecoration()
        {
            Color mosaicColor = new Color(0.20f, 0.55f, 0.85f);

            for (int x = -4; x <= 4; x += 2)
            {
                CreateCube(
                    "Barcelona_Mosaic",
                    new Vector3(x, 0.12f, 0f),
                    new Vector3(0.8f, 0.02f, 0.8f),
                    mosaicColor
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