using ModuleZ.Core.Managers;
using ModuleZ.Game.Animation;
using ModuleZ.Game.Interaction;
using ModuleZ.Game.NPCs;
using ModuleZ.OpenWorld.Builders;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Andalucia70s
{
    public class Andalucia70sOpenWorldNPCBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateNPCs();

            Debug.Log("[Module Z] NPCs OpenWorld Andalucía años 70 creados.");
        }

        private void CreateNPCs()
        {
            GameObject antonio = CreateHumanNPC(
                "NPC_Antonio_70s",
                new Vector3(-4f, 0f, 4f),
                new Color(0.90f, 0.80f, 0.55f),
                new Color(0.10f, 0.08f, 0.05f)
            );

            AddProgressDialogue(
                antonio,
                "Antonio",
                ModuleZRivalId.Andalucia,
                "Todavía queda el duelo más difícil de Andalucía.",
                "Has derrotado al rival de Andalucía. Pocos llegan tan lejos."
            );

            AddPatrol(
                antonio,
                1.1f,
                antonio.transform.position,
                antonio.transform.position + new Vector3(3f, 0f, 0f),
                antonio.transform.position + new Vector3(3f, 0f, 3f),
                antonio.transform.position + new Vector3(0f, 0f, 3f)
            );

            AddAmbientConversation(
                antonio,
                "Antonio",
                new string[]
                {
                    "El patio está tranquilo hoy.",
                    "Aquí los duelos se ganan con calma."
                }
            );

            GameObject carmen = CreateHumanNPC(
                "NPC_Carmen_70s",
                new Vector3(4f, 0f, 4f),
                new Color(0.75f, 0.25f, 0.15f),
                new Color(0.08f, 0.08f, 0.08f)
            );

            AddDialogue(
                carmen,
                "Carmen",
                new string[]
                {
                    "Los patios parecen tranquilos, pero esconden buenos duelos.",
                    "La clave no siempre es mover rápido, sino mover bien."
                }
            );

            AddPatrol(
                carmen,
                1.0f,
                carmen.transform.position,
                carmen.transform.position + Vector3.forward * 4f,
                carmen.transform.position + Vector3.right * 2f
            );

            AddAmbientConversation(
                carmen,
                "Carmen",
                new string[]
                {
                    "Las flores alegran la plaza.",
                    "El rival de Andalucía no se vence por casualidad."
                }
            );

            CreateGuideNPC();
            CreateSittingNPCs();
            CreateAndaluciaRival();
        }

        private void CreateAndaluciaRival()
        {
            Vector3 position = new Vector3(0f, 0f, 9f);

            if (!ModuleZGameState.RivalAndaluciaDefeated)
            {
                GameObject rival = CreateHumanNPC(
                    "NPC_Rival_Andalucia",
                    position,
                    new Color(0.90f, 0.80f, 0.55f),
                    new Color(0.10f, 0.08f, 0.05f)
                );

                DuelStarterInteractable duelStarter =
                    rival.AddComponent<DuelStarterInteractable>();

                duelStarter.rivalId = ModuleZRivalId.Andalucia;

                rival.AddComponent<ModuleZTalkAnimation>();
                rival.AddComponent<ModuleZNPCFacePlayer>();
            }
            else
            {
                GameObject defeatedRival = CreateHumanNPC(
                    "NPC_Rival_Andalucia_Derrotado",
                    position,
                    new Color(0.25f, 0.25f, 0.25f),
                    new Color(0.10f, 0.08f, 0.05f)
                );

                MessageInteractable message =
                    defeatedRival.AddComponent<MessageInteractable>();

                message.interactionText = "Pulsa E para hablar";
                message.message = GetDefeatedMessage(ModuleZRivalId.Andalucia);

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
            GameObject lola = CreateHumanNPC(
                "NPC_Lola_Sentada_70s",
                new Vector3(-8f, 0f, 8f),
                new Color(0.85f, 0.70f, 0.45f),
                new Color(0.10f, 0.08f, 0.05f)
            );

            AddDialogue(
                lola,
                "Lola",
                new string[]
                {
                    "Desde este banco se ven todos los movimientos.",
                    "Andalucía recompensa a quien sabe esperar."
                }
            );

            ModuleZNPCStaticPose lolaPose =
                lola.AddComponent<ModuleZNPCStaticPose>();

            lolaPose.Configure(ModuleZNPCStaticPose.PoseType.Sitting);

            GameObject manolo = CreateHumanNPC(
                "NPC_Manolo_Sentado_70s",
                new Vector3(8f, 0f, 8f),
                new Color(0.55f, 0.35f, 0.20f),
                new Color(0.12f, 0.08f, 0.05f)
            );

            AddDialogue(
                manolo,
                "Manolo",
                new string[]
                {
                    "Un buen duelo se gana antes de mover la primera pieza.",
                    "Mira el tablero como si fuera un patio: todo tiene su sitio."
                }
            );

            ModuleZNPCStaticPose manoloPose =
                manolo.AddComponent<ModuleZNPCStaticPose>();

            manoloPose.Configure(ModuleZNPCStaticPose.PoseType.Sitting);
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
                case ModuleZRivalId.Andalucia:
                    return "Buen duelo. Andalucía también reconoce tu victoria.";

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