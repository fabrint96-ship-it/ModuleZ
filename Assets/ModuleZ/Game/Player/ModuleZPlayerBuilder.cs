using UnityEngine;
using ModuleZ.Game.Interaction;
using ModuleZ.Game.Animation;

namespace ModuleZ.Game.Player
{
    public class ModuleZPlayerBuilder : MonoBehaviour
    {
        public GameObject BuildPlayer(Vector3 position)
        {
            GameObject root = new GameObject("Player_Humano_Cubico");
            root.transform.position = position;

            CharacterController controller = root.AddComponent<CharacterController>();
            root.AddComponent<ModuleZPlayerInteraction>();

            controller.height = 1.8f;
            controller.radius = 0.35f;
            controller.center = new Vector3(0f, 0.9f, 0f);

            CreateBodyPart("Cabeza", root.transform, new Vector3(0f, 1.65f, 0f), new Vector3(0.45f, 0.45f, 0.45f), new Color(0.82f, 0.62f, 0.45f));

            GameObject torso = CreateBodyPart("Torso", root.transform, new Vector3(0f, 1.05f, 0f), new Vector3(0.65f, 0.75f, 0.35f), new Color(0.12f, 0.28f, 0.55f));

            GameObject leftArm = CreateBodyPart("Brazo_Izq", root.transform, new Vector3(-0.48f, 1.1f, 0f), new Vector3(0.22f, 0.7f, 0.22f), new Color(0.82f, 0.62f, 0.45f));

            GameObject rightArm = CreateBodyPart("Brazo_Der", root.transform, new Vector3(0.48f, 1.1f, 0f), new Vector3(0.22f, 0.7f, 0.22f), new Color(0.82f, 0.62f, 0.45f));

            GameObject leftLeg = CreateBodyPart("Pierna_Izq", root.transform, new Vector3(-0.18f, 0.35f, 0f), new Vector3(0.25f, 0.7f, 0.25f), new Color(0.08f, 0.08f, 0.12f));

            GameObject rightLeg = CreateBodyPart("Pierna_Der", root.transform, new Vector3(0.18f, 0.35f, 0f), new Vector3(0.25f, 0.7f, 0.25f), new Color(0.08f, 0.08f, 0.12f));

            root.AddComponent<ModuleZPlayerController>();

            ModuleZPlayerWalkAnimation walk =
                root.AddComponent<ModuleZPlayerWalkAnimation>();

            walk.Configure(
                leftArm.transform,
                rightArm.transform,
                leftLeg.transform,
                rightLeg.transform,
                torso.transform
            );

            return root;
        }

        private GameObject CreateBodyPart(
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