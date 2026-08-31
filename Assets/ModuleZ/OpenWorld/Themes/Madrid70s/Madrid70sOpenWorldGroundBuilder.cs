using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Madrid70s
{
    public class Madrid70sOpenWorldGroundBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateBaseGround();
            CreateMainPlaza();
            CreatePlazaTiles();

            Debug.Log("[Module Z] Suelo Madrid años 70 creado.");
        }

        private void CreateBaseGround()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground_Madrid_70s";
            ground.transform.position = new Vector3(0f, -0.1f, 0f);
            ground.transform.localScale = new Vector3(70f, 0.2f, 70f);

            Renderer renderer = ground.GetComponent<Renderer>();
            renderer.material.color = new Color(0.42f, 0.36f, 0.30f);
        }

        private void CreateMainPlaza()
        {
            GameObject plaza = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plaza.name = "Plaza_Central_Adoquines";
            plaza.transform.position = new Vector3(0f, 0.02f, 0f);
            plaza.transform.localScale = new Vector3(20f, 0.08f, 20f);

            Renderer renderer = plaza.GetComponent<Renderer>();
            renderer.material.color = new Color(0.50f, 0.47f, 0.42f);
        }

        private void CreatePlazaTiles()
        {
            for (int x = -9; x <= 9; x += 2)
            {
                for (int z = -9; z <= 9; z += 2)
                {
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    tile.name = "Adoquin_Plaza";
                    tile.transform.position = new Vector3(x, 0.11f, z);
                    tile.transform.localScale = new Vector3(1.8f, 0.04f, 1.8f);

                    Renderer renderer = tile.GetComponent<Renderer>();
                    renderer.material.color = new Color(0.38f, 0.36f, 0.33f);
                }
            }
        }
    }
}