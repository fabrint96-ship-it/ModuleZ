using ModuleZ.Core.Managers;
using ModuleZ.Game.Animation;
using ModuleZ.Game.Interaction;
using ModuleZ.Game.NPCs;
using ModuleZ.OpenWorld.Builders;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Valencia70s
{
    public class Valencia70sOpenWorldNPCBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateNPCs();

            Debug.Log("[Module Z] NPCs OpenWorld Valencia años 70 creados.");
        }

        private void CreateNPCs()
        {
            GameObject vendedor = CreateHumanNPC(
                "NPC_Vicente_70s",
                new Vector3(-5f, 0f, 5f),
                new Color(0.90f, 0.65f, 0.35f),
                new Color(0.10f, 0.08f, 0.06f)
            );

            AddProgressDialogue(
                vendedor,
                "Vicente",
                ModuleZRivalId.Valencia,
                "El rival de Valencia todavía no ha sido derrotado.",
                "Ya has vencido al rival de Valencia. Bien jugado."
            );

            AddPatrol(
                vendedor,
                1.1f,
                vendedor.transform.position,
                vendedor.transform.position + new Vector3(3f, 0f, 0f),
                vendedor.transform.position + new Vector3(3f, 0f, 3f),
                vendedor.transform.position + new Vector3(0f, 0f, 3f)
            );

            AddAmbientConversation(
                vendedor,
                "Vicente",
                new string[]
                {
                    "El sol de Valencia ayuda a pensar mejor.",
                    "No corras con la pieza Z; mira primero el tablero."
                }
            );

            GameObject vecino = CreateHumanNPC(
                "NPC_Pablo_70s",
                new Vector3(5f, 0f, 5f),
                new Color(0.25f, 0.50f, 0.75f),
                new Color(0.08f, 0.08f, 0.10f)
            );

            AddDialogue(
                vecino,
                "Pablo",
                new string[]
                {
                    "Dicen que el rival de Valencia domina los movimientos largos.",
                    "Si desbloqueas nuevas zonas, vuelve por aquí."
                }
            );

            AddPatrol(
                vecino,
                1.0f,
                vecino.transform.position,
                vecino.transform.position + Vector3.forward * 4f,
                vecino.transform.position + Vector3.right * 2f
            );

            AddAmbientConversation(
                vecino,
                "Pablo",
                new string[]
                {
                    "Las naranjas de esta plaza dan energía.",
                    "Un buen movimiento vale más que tres rápidos."
                }
            );

            CreateGuideNPC();
            CreateSittingNPCs();
            CreateValenciaRival();
        }

        private void CreateValenciaRival()
        {
            Vector3 position = new Vector3(0f, 0f, 9f);

            if (!ModuleZGameState.RivalValenciaDefeated)
            {
                GameObject rival = CreateHumanNPC(
                    "NPC_Rival_Valencia",
                    position,
                    new Color(0.80f, 0.55f, 0.18f),
                    new Color(0.08f, 0.08f, 0.08f)
                );

                DuelStarterInteractable duelStarter =
                    rival.AddComponent<DuelStarterInteractable>();

                duelStarter.rivalId = ModuleZRivalId.Valencia;

                rival.AddComponent<ModuleZTalkAnimation>();
                rival.AddComponent<ModuleZNPCFacePlayer>();
            }
            else
            {
                GameObject defeatedRival = CreateHumanNPC(
                    "NPC_Rival_Valencia_Derrotado",
                    position,
                    new Color(0.25f, 0.25f, 0.25f),
                    new Color(0.08f, 0.08f, 0.08f)
                );

                MessageInteractable message =
                    defeatedRival.AddComponent<MessageInteractable>();

                message.interactionText = "Pulsa E para hablar";
                message.message = GetDefeatedMessage(ModuleZRivalId.Valencia);

                defeatedRival.AddComponent<ModuleZTalkAnimation>();
                defeatedRival.AddComponent<ModuleZNPCFacePlayer>();
            }
        }

        private void CreateGuideNPC()
        {
            GameObject guide = CreateHumanNPC(
                "NPC_Guia_ModuleZ",
                new Vector3(0f, 0f, -7f),
                new Color(0.20f, 0.45f, 0.75f),
                new Color(0.05f, 0.05f, 0.08f)
            );

            guide.AddComponent<ModuleZGuideInteractable>();
            guide.AddComponent<ModuleZTalkAnimation>();
            guide.AddComponent<ModuleZNPCFacePlayer>();
        }

        private void CreateSittingNPCs()
        {
            GameObject amparo = CreateHumanNPC(
                "NPC_Amparo_Sentada_70s",
                new Vector3(-8f, 0f, 8f),
                new Color(0.65f, 0.40f, 0.22f),
                new Color(0.12f, 0.08f, 0.06f)
            );

            AddDialogue(
                amparo,
                "Amparo",
                new string[]
                {
                    "Valencia se piensa mejor con calma.",
                    "A veces la mejor jugada es esperar."
                }
            );

            ModuleZNPCStaticPose amparoPose =
                amparo.AddComponent<ModuleZNPCStaticPose>();

            amparoPose.Configure(ModuleZNPCStaticPose.PoseType.Sitting);

            GameObject clara = CreateHumanNPC(
                "NPC_Clara_Sentada_70s",
                new Vector3(8f, 0f, 8f),
                new Color(0.95f, 0.55f, 0.20f),
                new Color(0.15f, 0.10f, 0.10f)
            );

            AddDialogue(
                clara,
                "Clara",
                new string[]
                {
                    "Me gusta ver cómo resuelven los puzzles.",
                    "Los mejores duelos tienen paciencia y precisión."
                }
            );

            ModuleZNPCStaticPose claraPose =
                clara.AddComponent<ModuleZNPCStaticPose>();

            claraPose.Configure(ModuleZNPCStaticPose.PoseType.Sitting);
        }

        private void AddDialogue(GameObject npc, string speakerName, string[] lines)
        {
            ModuleZDialogueInteractable dialogue =
                npc.AddComponent<ModuleZDialogueInteractable>();

            dialogue.speakerName = speakerName;
            dialogue.dialogueLines = lines;

            npc.AddComponent<ModuleZTalkAnimation>();
            npc.AddComponent<ModuleZNPCFacePlayer>();
        }

        private void AddProgressDialogue(
            GameObject npc,
            string speakerName,
            ModuleZRivalId relatedRival,
            string beforeDefeatLine,
            string afterDefeatLine)
        {
            ModuleZProgressDialogueInteractable dialogue =
                npc.AddComponent<ModuleZProgressDialogueInteractable>();

            dialogue.speakerName = speakerName;
            dialogue.relatedRival = relatedRival;
            dialogue.beforeDefeatLine = beforeDefeatLine;
            dialogue.afterDefeatLine = afterDefeatLine;

            npc.AddComponent<ModuleZTalkAnimation>();
            npc.AddComponent<ModuleZNPCFacePlayer>();
        }

        private void AddPatrol(
            GameObject npc,
            float speed,
            params Vector3[] points)
        {
            ModuleZNPCPatrolController patrol =
                npc.AddComponent<ModuleZNPCPatrolController>();

            patrol.Configure(points, speed);

            AddWalkAnimation(npc);
        }

        private void AddAmbientConversation(
            GameObject npc,
            string speakerName,
            string[] lines)
        {
            ModuleZNPCConversationController conversation =
                npc.AddComponent<ModuleZNPCConversationController>();

            conversation.Configure(speakerName, lines);
        }

        private string GetDefeatedMessage(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Valencia:
                    return "Bien jugado. Esta vez la victoria es tuya.";

                default:
                    return "Buen duelo. Ya me has ganado.";
            }
        }

        private GameObject CreateHumanNPC(
            string npcName,
            Vector3 position,
            Color shirtColor,
            Color pantsColor)
        {
            GameObject root = new GameObject(npcName);
            root.transform.position = position;

            CreatePart("Cabeza", root.transform, new Vector3(0f, 1.65f, 0f), new Vector3(0.45f, 0.45f, 0.45f), new Color(0.82f, 0.62f, 0.45f));
            CreatePart("Pelo_70s", root.transform, new Vector3(0f, 1.92f, 0f), new Vector3(0.5f, 0.18f, 0.5f), new Color(0.08f, 0.05f, 0.03f));
            CreatePart("Torso", root.transform, new Vector3(0f, 1.05f, 0f), new Vector3(0.65f, 0.75f, 0.35f), shirtColor);
            CreatePart("Brazo_Izq", root.transform, new Vector3(-0.48f, 1.1f, 0f), new Vector3(0.22f, 0.7f, 0.22f), new Color(0.82f, 0.62f, 0.45f));
            CreatePart("Brazo_Der", root.transform, new Vector3(0.48f, 1.1f, 0f), new Vector3(0.22f, 0.7f, 0.22f), new Color(0.82f, 0.62f, 0.45f));
            CreatePart("Pierna_Izq", root.transform, new Vector3(-0.18f, 0.35f, 0f), new Vector3(0.25f, 0.7f, 0.25f), pantsColor);
            CreatePart("Pierna_Der", root.transform, new Vector3(0.18f, 0.35f, 0f), new Vector3(0.25f, 0.7f, 0.25f), pantsColor);

            CreatePart("Ojo_Izq", root.transform, new Vector3(-0.12f, 1.68f, -0.24f), new Vector3(0.06f, 0.06f, 0.03f), Color.black);
            CreatePart("Ojo_Der", root.transform, new Vector3(0.12f, 1.68f, -0.24f), new Vector3(0.06f, 0.06f, 0.03f), Color.black);

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

        private void AddWalkAnimation(GameObject npc)
        {
            ModuleZNPCWalkAnimation walk =
                npc.AddComponent<ModuleZNPCWalkAnimation>();

            walk.Configure(
                npc.transform.Find("Brazo_Izq"),
                npc.transform.Find("Brazo_Der"),
                npc.transform.Find("Pierna_Izq"),
                npc.transform.Find("Pierna_Der")
            );
        }
    }
}