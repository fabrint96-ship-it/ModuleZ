using UnityEngine;
using ModuleZ.Duel.Rules;
using ModuleZ.Core.Managers;
using ModuleZ.Duel.Runtime;

namespace ModuleZ.Duel.Builders
{
    public class DuelPuzzleZBuilder : MonoBehaviour
    {
        private DuelThemeData themeData;

        public void Build()
        {
            themeData = DuelThemeDatabase.GetThemeData(ModuleZGameState.CurrentDuelTheme);

            CreatePuzzleBoard();
            CreateGoalZone();
            CreateZPiece();

            Debug.Log("[Module Z] Puzzle Z básico de duelo creado.");
        }

        private void CreatePuzzleBoard()
        {
            CreateCube(
                "Duel_Puzzle_Board",
                new Vector3(0f, 0.08f, 0f),
                new Vector3(10f, 0.12f, 8f),
                new Color(0.30f, 0.28f, 0.24f)
            );
        }

        private void CreateGoalZone()
        {
            GameObject goal = CreateCube(
                "Duel_Puzzle_Goal",
                new Vector3(3f, 0.2f, 2f),
                new Vector3(2f, 0.15f, 2f),
                themeData.accentColor
            );

            BoxCollider collider = goal.GetComponent<BoxCollider>();
            collider.isTrigger = true;

            goal.AddComponent<DuelPuzzleGoal>();
        }

        private void CreateZPiece()
        {
            GameObject root = new GameObject("Duel_Z_Piece");
            root.transform.position = Vector3.zero;
            root.tag = "ZPiece";

            Rigidbody rb = root.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            Color zColor = themeData.accentColor;

            CreateCube("Duel_Z_Block_01", root.transform, new Vector3(-2f, 0.75f, -1f), Vector3.one, zColor);
            CreateCube("Duel_Z_Block_02", root.transform, new Vector3(-1f, 0.75f, -1f), Vector3.one, zColor);
            CreateCube("Duel_Z_Block_03", root.transform, new Vector3(-1f, 0.75f, 0f), Vector3.one, zColor);
            CreateCube("Duel_Z_Block_04", root.transform, new Vector3(0f, 0.75f, 0f), Vector3.one, zColor);

            root.AddComponent<DuelZPieceController>();
            root.AddComponent<DuelVictoryEffect>();
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

        private GameObject CreateCube(string name, Transform parent, Vector3 localPosition, Vector3 scale, Color color)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = localPosition;
            cube.transform.localScale = scale;

            Renderer renderer = cube.GetComponent<Renderer>();
            renderer.material.color = color;

            return cube;
        }
    }
}