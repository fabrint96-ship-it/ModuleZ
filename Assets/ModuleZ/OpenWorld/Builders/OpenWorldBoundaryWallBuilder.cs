using UnityEngine;

namespace ModuleZ.OpenWorld.Builders
{
    public class OpenWorldBoundaryWallBuilder : MonoBehaviour
    {
        public void Build()
        {
            Color wallColor = new Color(0.32f, 0.26f, 0.18f);

            CreateCube("BoundaryWall_North", new Vector3(0f, 1.5f, 31f), new Vector3(64f, 3f, 0.5f), wallColor);
            CreateCube("BoundaryWall_South", new Vector3(0f, 1.5f, -31f), new Vector3(64f, 3f, 0.5f), wallColor);
            CreateCube("BoundaryWall_East", new Vector3(31f, 1.5f, 0f), new Vector3(0.5f, 3f, 64f), wallColor);
            CreateCube("BoundaryWall_West", new Vector3(-31f, 1.5f, 0f), new Vector3(0.5f, 3f, 64f), wallColor);
        }

        private GameObject CreateCube(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(transform, false);
            obj.transform.position = position;
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().material.color = color;
            return obj;
        }
    }
}