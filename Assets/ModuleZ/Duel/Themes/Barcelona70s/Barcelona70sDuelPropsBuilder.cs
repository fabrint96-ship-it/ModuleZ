using UnityEngine;

namespace ModuleZ.Duel.Themes.Barcelona70s
{
    public class Barcelona70sDuelPropsBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateLamps();
            CreateBenches();
            CreatePlanters();
            CreateSigns();

            Debug.Log("[Module Z] Props Duel Barcelona años 70 creados.");
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
            CreateCube("Barcelona_Farola_Poste", position, new Vector3(0.18f, 3f, 0.18f), new Color(0.05f, 0.05f, 0.06f));
            CreateCube("Barcelona_Farola_Cabezal", position + new Vector3(0f, 1.65f, 0f), new Vector3(0.9f, 0.28f, 0.9f), new Color(1f, 0.78f, 0.42f));
        }

        private void CreateBenches()
        {
            CreateBench(new Vector3(-8f, 0.45f, 15f));
            CreateBench(new Vector3(8f, 0.45f, 15f));
        }

        private void CreateBench(Vector3 position)
        {
            CreateCube("Barcelona_Banco_Asiento", position, new Vector3(3f, 0.25f, 0.75f), new Color(0.35f, 0.18f, 0.08f));
            CreateCube("Barcelona_Banco_Respaldo", position + new Vector3(0f, 0.45f, 0.35f), new Vector3(3f, 0.7f, 0.18f), new Color(0.28f, 0.12f, 0.05f));
        }

        private void CreatePlanters()
        {
            CreatePlanter(new Vector3(-11f, 0.45f, 0f));
            CreatePlanter(new Vector3(11f, 0.45f, 0f));
        }

        private void CreatePlanter(Vector3 position)
        {
            CreateCube("Barcelona_Jardinera_Base", position, new Vector3(1.5f, 0.6f, 1.5f), new Color(0.55f, 0.36f, 0.22f));
            CreateCube("Barcelona_Planta", position + new Vector3(0f, 0.55f, 0f), new Vector3(1.1f, 0.5f, 1.1f), new Color(0.12f, 0.45f, 0.18f));
        }

        private void CreateSigns()
        {
            CreateCube("Cartel_Duelo_Barcelona_70s", new Vector3(0f, 2.2f, 16.65f), new Vector3(7f, 1.1f, 0.12f), new Color(0.95f, 0.68f, 0.25f));
            CreateCube("Cartel_Barcelona_ModuleZ", new Vector3(0f, 2.2f, -16.65f), new Vector3(7f, 1.1f, 0.12f), new Color(0.18f, 0.45f, 0.75f));
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