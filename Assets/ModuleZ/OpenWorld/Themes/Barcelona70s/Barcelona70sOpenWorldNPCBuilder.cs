using ModuleZ.Core.Managers;
using ModuleZ.Game.Animation;
using ModuleZ.Game.Interaction;
using ModuleZ.Game.NPCs;
using ModuleZ.OpenWorld.Builders;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Barcelona70s
{
    public class Barcelona70sOpenWorldNPCBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateNPCs();

            Debug.Log("[Module Z] NPCs OpenWorld Barcelona años 70 creados.");
        }

        private void CreateNPCs()
        {
            GameObject vecino = CreateHumanNPC(
                "NPC_Jordi_70s",
                new Vector3(-5f, 0f, 5f),
                new Color(0.18f, 0.28f, 0.55f),
                new Color(0.05f, 0.05f, 0.08f)
            );

            AddProgressDialogue(
                vecino,
                "Jordi",
                ModuleZRivalId.Barcelona,
                "Todavía no has derrotado al rival de Barcelona.",
                "He oído que venciste al rival de Barcelona. Impresionante."
            );

            AddPatrol(
                vecino,
                1.1f,
                vecino.transform.position,
                vecino.transform.position + new Vector3(3f, 0f, 0f),
                vecino.transform.position + new Vector3(3f, 0f, 3f),
                vecino.transform.position + new Vector3(0f, 0f, 3f)
            );

            AddAmbientConversation(
                vecino,
                "Jordi",
                new string[]
                {
                    "Barcelona tiene ritmo propio.",
                    "Aquí los puzzles se resuelven con estilo."
                }
            );

            GameObject comerciante = CreateHumanNPC(
                "NPC_Marc_70s",
                new Vector3(5f, 0f, 5f),
                new Color(0.85f, 0.65f, 0.25f),
                new Color(0.10f, 0.08f, 0.06f)
            );

            AddDialogue(
                comerciante,
                "Marc",
                new string[]
                {
                    "Las plazas siempre esconden buenos rivales.",
                    "Observa los patrones antes de mover la pieza Z."
                }
            );

            AddPatrol(
                comerciante,
                1.0f,
                comerciante.transform.position,
                comerciante.transform.position + Vector3.forward * 4f,
                comerciante.transform.position + Vector3.right * 2f
            );

            AddAmbientConversation(
                comerciante,
                "Marc",
                new string[]
                {
                    "Un buen duelo empieza observando el tablero.",
                    "Las piezas Z cada vez son más conocidas aquí."
                }
            );

            CreateGuideNPC();
            CreateSittingNPCs();
            CreateBarcelonaRival();
        }

        private void CreateBarcelonaRival()
        {
            Vector3 position = new Vector3(0f, 0f, 9f);

            if (!ModuleZGameState.RivalBarcelonaDefeated)
            {
                GameObject rival = CreateHumanNPC(
                    "NPC_Rival_Barcelona",
                    position,
                    new Color(0.18f, 0.28f, 0.55f),
                    new Color(0.05f, 0.05f, 0.08f)
                );

                DuelStarterInteractable duelStarter =
                    rival.AddComponent<DuelStarterInteractable>();

                duelStarter.rivalId = ModuleZRivalId.Barcelona;

                rival.AddComponent<ModuleZTalkAnimation>();
                rival.AddComponent<ModuleZNPCFacePlayer>();
            }
            else
            {
                GameObject defeatedRival = CreateHumanNPC(
                    "NPC_Rival_Barcelona_Derrotado",
                    position,
                    new Color(0.25f, 0.25f, 0.25f),
                    new Color(0.05f, 0.05f, 0.08f)
                );

                MessageInteractable message =
                    defeatedRival.AddComponent<MessageInteractable>();

                message.interactionText = "Pulsa E para hablar";
                message.message = GetDefeatedMessage(ModuleZRivalId.Barcelona);

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
            GameObject nuria = CreateHumanNPC(
                "NPC_Nuria_Sentada_70s",
                new Vector3(-8f, 0f, 8f),
                new Color(0.55f, 0.36f, 0.22f),
                new Color(0.08f, 0.08f, 0.08f)
            );

            AddDialogue(
                nuria,
                "Núria",
                new string[]
                {
                    "Desde aquí se ve toda la plaza.",
                    "En Barcelona, la paciencia también forma parte del duelo."
                }
            );

            ModuleZNPCStaticPose nuriaPose =
                nuria.AddComponent<ModuleZNPCStaticPose>();

            nuriaPose.Configure(ModuleZNPCStaticPose.PoseType.Sitting);

            GameObject laia = CreateHumanNPC(
                "NPC_Laia_Sentada_70s",
                new Vector3(8f, 0f, 8f),
                new Color(0.72f, 0.42f, 0.22f),
                new Color(0.12f, 0.08f, 0.08f)
            );

            AddDialogue(
                laia,
                "Laia",
                new string[]
                {
                    "Los mosaicos ayudan a ver patrones.",
                    "Mover la pieza Z sin pensar suele salir caro."
                }
            );

            ModuleZNPCStaticPose laiaPose =
                laia.AddComponent<ModuleZNPCStaticPose>();

            laiaPose.Configure(ModuleZNPCStaticPose.PoseType.Sitting);
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
                case ModuleZRivalId.Barcelona:
                    return "Has resuelto el puzzle con estilo. Buen duelo.";

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