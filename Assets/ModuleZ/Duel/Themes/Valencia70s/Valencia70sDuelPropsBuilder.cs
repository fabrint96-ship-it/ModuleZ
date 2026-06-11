using UnityEngine;

namespace ModuleZ.Duel.Themes.Valencia70s
{
    public class Valencia70sDuelPropsBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateLamps();
            CreateBenches();
            CreatePalmTrees();
            CreatePlanters();
            CreateSigns();

            Debug.Log("[Module Z] Props Duel Valencia años 70 creados.");
        }

        private void CreateLamps()
        {
            CreateLamp(new Vector3(-11f, 1.5f, -14f));
            CreateLamp(new Vector3(11f, 1.5f, -14f));
            CreateLamp(new Vector3(-11f, 1.5f, 14f));
            CreateLamp(new Vector3(11f, 1.5f, 14f));
        }

        private void CreateLamp(Vector3 position)
        {
            CreateCube("Valencia_Farola_Poste", position, new Vector3(0.18f, 3f, 0.18f), new Color(0.08f, 0.07f, 0.05f));
            CreateCube("Valencia_Farola_Luz", position + new Vector3(0f, 1.65f, 0f), new Vector3(0.7f, 0.3f, 0.7f), new Color(1f, 0.85f, 0.45f));
        }

        private void CreateBenches()
        {
            CreateBench(new Vector3(-8f, 0.45f, 16f));
            CreateBench(new Vector3(8f, 0.45f, 16f));
            CreateBench(new Vector3(-8f, 0.45f, -16f));
            CreateBench(new Vector3(8f, 0.45f, -16f));
        }

        private void CreateBench(Vector3 position)
        {
            CreateCube("Valencia_Banco_Asiento", position, new Vector3(3f, 0.25f, 0.75f), new Color(0.42f, 0.22f, 0.08f));
            CreateCube("Valencia_Banco_Respaldo", position + new Vector3(0f, 0.45f, 0.35f), new Vector3(3f, 0.7f, 0.18f), new Color(0.32f, 0.14f, 0.04f));
        }

        private void CreatePalmTrees()
        {
            CreatePalmTree(new Vector3(-12f, 1.6f, 0f));
            CreatePalmTree(new Vector3(12f, 1.6f, 0f));
        }

        private void CreatePalmTree(Vector3 position)
        {
            CreateCube("Valencia_Palmera_Tronco", position, new Vector3(0.45f, 3.2f, 0.45f), new Color(0.45f, 0.25f, 0.10f));

            Vector3 top = position + new Vector3(0f, 1.8f, 0f);

            CreateCube("Valencia_Palmera_Hoja_A", top + new Vector3(0.8f, 0f, 0f), new Vector3(1.8f, 0.25f, 0.45f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Valencia_Palmera_Hoja_B", top + new Vector3(-0.8f, 0f, 0f), new Vector3(1.8f, 0.25f, 0.45f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Valencia_Palmera_Hoja_C", top + new Vector3(0f, 0f, 0.8f), new Vector3(0.45f, 0.25f, 1.8f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Valencia_Palmera_Hoja_D", top + new Vector3(0f, 0f, -0.8f), new Vector3(0.45f, 0.25f, 1.8f), new Color(0.10f, 0.45f, 0.18f));
        }

        private void CreatePlanters()
        {
            CreatePlanter(new Vector3(-11f, 0.45f, -6f));
            CreatePlanter(new Vector3(11f, 0.45f, 6f));
        }

        private void CreatePlanter(Vector3 position)
        {
            CreateCube("Valencia_Jardinera_Base", position, new Vector3(1.6f, 0.6f, 1.6f), new Color(0.65f, 0.38f, 0.18f));
            CreateCube("Valencia_Planta", position + new Vector3(0f, 0.55f, 0f), new Vector3(1.1f, 0.5f, 1.1f), new Color(0.12f, 0.48f, 0.18f));
        }

        private void CreateSigns()
        {
            CreateCube("Cartel_Duelo_Valencia_70s", new Vector3(0f, 2.3f, 17.65f), new Vector3(7f, 1.1f, 0.12f), new Color(0.95f, 0.62f, 0.20f));
            CreateCube("Cartel_Valencia_ModuleZ", new Vector3(0f, 2.3f, -17.65f), new Vector3(7f, 1.1f, 0.12f), new Color(0.25f, 0.50f, 0.75f));
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