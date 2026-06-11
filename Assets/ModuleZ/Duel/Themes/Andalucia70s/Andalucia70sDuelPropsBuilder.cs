using UnityEngine;

namespace ModuleZ.Duel.Themes.Andalucia70s
{
    public class Andalucia70sDuelPropsBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateArches();
            CreateLamps();
            CreatePlanters();
            CreateBenches();
            CreateSigns();

            Debug.Log("[Module Z] Props Duel Andalucía años 70 creados.");
        }

        private void CreateArches()
        {
            CreateArch(new Vector3(-9f, 1.6f, 17.4f));
            CreateArch(new Vector3(0f, 1.6f, 17.4f));
            CreateArch(new Vector3(9f, 1.6f, 17.4f));
        }

        private void CreateArch(Vector3 position)
        {
            Color color = new Color(0.95f, 0.92f, 0.84f);

            CreateCube("Andalucia_Arco_Pilar_Izq", position + new Vector3(-1f, 0f, 0f), new Vector3(0.35f, 2.6f, 0.35f), color);
            CreateCube("Andalucia_Arco_Pilar_Der", position + new Vector3(1f, 0f, 0f), new Vector3(0.35f, 2.6f, 0.35f), color);
            CreateCube("Andalucia_Arco_Superior", position + new Vector3(0f, 1.3f, 0f), new Vector3(2.4f, 0.35f, 0.35f), color);
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
            CreateCube("Andalucia_Farol_Poste", position, new Vector3(0.16f, 3f, 0.16f), new Color(0.04f, 0.04f, 0.04f));
            CreateCube("Andalucia_Farol_Luz", position + new Vector3(0f, 1.65f, 0f), new Vector3(0.65f, 0.35f, 0.65f), new Color(1f, 0.80f, 0.45f));
        }

        private void CreatePlanters()
        {
            CreatePlanter(new Vector3(-12f, 0.45f, -5f));
            CreatePlanter(new Vector3(12f, 0.45f, 5f));
            CreatePlanter(new Vector3(-12f, 0.45f, 5f));
            CreatePlanter(new Vector3(12f, 0.45f, -5f));
        }

        private void CreatePlanter(Vector3 position)
        {
            CreateCube("Andalucia_Maceta_Base", position, new Vector3(1.4f, 0.6f, 1.4f), new Color(0.65f, 0.28f, 0.12f));
            CreateCube("Andalucia_Planta", position + new Vector3(0f, 0.55f, 0f), new Vector3(1.0f, 0.5f, 1.0f), new Color(0.10f, 0.45f, 0.18f));
            CreateCube("Andalucia_Flor", position + new Vector3(0f, 0.9f, 0f), new Vector3(0.35f, 0.25f, 0.35f), new Color(0.95f, 0.15f, 0.15f));
        }

        private void CreateBenches()
        {
            CreateBench(new Vector3(-8f, 0.45f, -16f));
            CreateBench(new Vector3(8f, 0.45f, -16f));
        }

        private void CreateBench(Vector3 position)
        {
            CreateCube("Andalucia_Banco_Asiento", position, new Vector3(3f, 0.25f, 0.75f), new Color(0.32f, 0.15f, 0.06f));
            CreateCube("Andalucia_Banco_Respaldo", position + new Vector3(0f, 0.45f, 0.35f), new Vector3(3f, 0.7f, 0.18f), new Color(0.24f, 0.10f, 0.04f));
        }

        private void CreateSigns()
        {
            CreateCube("Cartel_Duelo_Andalucia_70s", new Vector3(0f, 2.6f, 17.65f), new Vector3(8f, 1.1f, 0.12f), new Color(0.90f, 0.72f, 0.28f));
            CreateCube("Cartel_Andalucia_ModuleZ", new Vector3(0f, 2.6f, -17.65f), new Vector3(8f, 1.1f, 0.12f), new Color(0.18f, 0.45f, 0.75f));
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