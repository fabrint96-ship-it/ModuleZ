using ModuleZ.Duel.Rules;
using UnityEngine;

namespace ModuleZ.Duel.Themes.Barcelona70s
{
    public class Barcelona70sDuelOpponentBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateOpponents();

            Debug.Log("[Module Z] Rivales Duel Barcelona años 70 creados.");
        }

        private void CreateOpponents()
        {
            CreateOpponent(
                "Barcelona_Rival_Principal",
                new Vector3(0f, 0f, 11f),
                new Color(0.18f, 0.28f, 0.55f),
                new Color(0.05f, 0.05f, 0.08f)
            );

            CreateOpponent(
                "Barcelona_Rival_Secundario_A",
                new Vector3(-4f, 0f, 9f),
                new Color(0.75f, 0.30f, 0.20f),
                new Color(0.08f, 0.08f, 0.10f)
            );

            CreateOpponent(
                "Barcelona_Rival_Secundario_B",
                new Vector3(4f, 0f, 9f),
                new Color(0.85f, 0.65f, 0.25f),
                new Color(0.10f, 0.08f, 0.06f)
            );
        }

        private GameObject CreateOpponent(
            string npcName,
            Vector3 position,
            Color shirtColor,
            Color pantsColor)
        {
            GameObject root = new GameObject(npcName);
            root.transform.position = position;
            root.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            CreatePart("Cabeza", root.transform, new Vector3(0f, 1.65f, 0f), new Vector3(0.45f, 0.45f, 0.45f), new Color(0.82f, 0.62f, 0.45f));
            CreatePart("Pelo_70s", root.transform, new Vector3(0f, 1.92f, 0f), new Vector3(0.5f, 0.18f, 0.5f), new Color(0.08f, 0.05f, 0.03f));
            CreatePart("Torso", root.transform, new Vector3(0f, 1.05f, 0f), new Vector3(0.65f, 0.75f, 0.35f), shirtColor);
            CreatePart("Brazo_Izq", root.transform, new Vector3(-0.48f, 1.1f, 0f), new Vector3(0.22f, 0.7f, 0.22f), new Color(0.82f, 0.62f, 0.45f));
            CreatePart("Brazo_Der", root.transform, new Vector3(0.48f, 1.1f, 0f), new Vector3(0.22f, 0.7f, 0.22f), new Color(0.82f, 0.62f, 0.45f));
            CreatePart("Pierna_Izq", root.transform, new Vector3(-0.18f, 0.35f, 0f), new Vector3(0.25f, 0.7f, 0.25f), pantsColor);
            CreatePart("Pierna_Der", root.transform, new Vector3(0.18f, 0.35f, 0f), new Vector3(0.25f, 0.7f, 0.25f), pantsColor);

            CreatePart("Ojo_Izq", root.transform, new Vector3(-0.12f, 1.68f, -0.24f), new Vector3(0.06f, 0.06f, 0.03f), Color.black);
            CreatePart("Ojo_Der", root.transform, new Vector3(0.12f, 1.68f, -0.24f), new Vector3(0.06f, 0.06f, 0.03f), Color.black);

            root.AddComponent<DuelOpponentIdleAnimation>();

            return root;
        }

        private GameObject CreatePart(
            string name,
            Transform parent,
            Vector3 localPosition,
            Vector3 scale,
            Color color)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = scale;

            Renderer renderer = part.GetComponent<Renderer>();
            renderer.material.color = color;

            return part;
        }
    }
}