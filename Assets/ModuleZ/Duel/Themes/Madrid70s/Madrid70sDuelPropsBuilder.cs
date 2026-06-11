using UnityEngine;

namespace ModuleZ.Duel.Themes.Madrid70s
{
    public class Madrid70sDuelPropsBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateLamps();
            CreateSpectatorBenches();
            CreateMadridSigns();

            Debug.Log("[Module Z] Props Duel Madrid años 70 creados.");
        }

        private void CreateLamps()
        {
            CreateLamp(new Vector3(-10f, 1.5f, -13f));
            CreateLamp(new Vector3(10f, 1.5f, -13f));
            CreateLamp(new Vector3(-10f, 1.5f, 13f));
            CreateLamp(new Vector3(10f, 1.5f, 13f));
        }

        private void CreateLamp(Vector3 position)
        {
            CreateCube("Duel_Farola_Poste_70s", position, new Vector3(0.18f, 3f, 0.18f), new Color(0.07f, 0.07f, 0.07f));
            CreateCube("Duel_Farola_Luz_70s", position + new Vector3(0f, 1.65f, 0f), new Vector3(0.65f, 0.3f, 0.65f), new Color(1f, 0.82f, 0.45f));
        }

        private void CreateSpectatorBenches()
        {
            CreateBench(new Vector3(-8f, 0.45f, 15f));
            CreateBench(new Vector3(8f, 0.45f, 15f));
            CreateBench(new Vector3(-8f, 0.45f, -15f));
            CreateBench(new Vector3(8f, 0.45f, -15f));
        }

        private void CreateBench(Vector3 position)
        {
            CreateCube("Duel_Banco_Asiento_70s", position, new Vector3(3f, 0.25f, 0.7f), new Color(0.30f, 0.15f, 0.06f));
            CreateCube("Duel_Banco_Respaldo_70s", position + new Vector3(0f, 0.45f, 0.35f), new Vector3(3f, 0.7f, 0.18f), new Color(0.25f, 0.12f, 0.05f));
        }

        private void CreateMadridSigns()
        {
            CreateCube("Cartel_Duelo_Madrid_70s", new Vector3(0f, 2.2f, 16.65f), new Vector3(6f, 1.1f, 0.12f), new Color(0.85f, 0.70f, 0.35f));
            CreateCube("Cartel_ModuleZ", new Vector3(0f, 2.2f, -16.65f), new Vector3(6f, 1.1f, 0.12f), new Color(0.20f, 0.35f, 0.60f));
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