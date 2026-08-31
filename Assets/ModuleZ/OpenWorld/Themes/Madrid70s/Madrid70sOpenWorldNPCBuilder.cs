using ModuleZ.Core.Managers;
using ModuleZ.Game.Animation;
using ModuleZ.Game.Interaction;
using ModuleZ.Game.NPCs;
using ModuleZ.OpenWorld.Builders;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.OpenWorld.Themes.Madrid70s
{
    public class Madrid70sOpenWorldNPCBuilder : MonoBehaviour
    {
        public void Build()
        {
            CreateNPCs();

            Debug.Log("[Module Z] NPCs OpenWorld Madrid años 70 creados.");
        }

        private void CreateNPCs()
        {
            GameObject vecino = CreateHumanNPC(
                "NPC_Miguel_70s",
                new Vector3(-4f, 0f, 4f),
                new Color(0.45f, 0.32f, 0.18f),
                new Color(0.12f, 0.10f, 0.08f)
            );

            AddProgressDialogue(
                vecino,
                "Miguel",
                ModuleZRivalId.Madrid,
                "El rival de Madrid sigue esperando en la plaza.",
                "Ya has vencido al rival de Madrid. Buen trabajo."
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
                "Miguel",
                ModuleZNPCDialogueLibrary.MadridAmbient()
            );

            GameObject comerciante = CreateHumanNPC(
                "NPC_Ramon_70s",
                new Vector3(4f, 0f, 4f),
                new Color(0.70f, 0.62f, 0.42f),
                new Color(0.18f, 0.12f, 0.08f)
            );

            AddDialogue(
                comerciante,
                "Ramón",
                new string[]
                {
                    "Tengo piezas, herramientas y rumores.",
                    "Si buscas rivales, mira cerca de la plaza."
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
                "Ramón",
                 ModuleZNPCDialogueLibrary.MadridShopkeeper()
            );

            CreateGuideNPC();
            CreateSittingNPCs();

            CreateMadridRival();
            CreateBarcelonaRival();
            CreateValenciaRival();
            CreateAndaluciaRival();
        }

        private void CreateMadridRival()
        {
            Vector3 position = new Vector3(0f, 0f, 9f);

            if (!ModuleZGameState.RivalMadridDefeated)
            {
                GameObject rival = CreateHumanNPC(
                    "NPC_Rival_Madrid",
                    position,
                    new Color(0.55f, 0.10f, 0.08f),
                    new Color(0.05f, 0.05f, 0.08f)
                );

                ModuleZRivalWorldHUDBuilder.Build(rival.transform, ModuleZRivalId.Madrid);

                DuelStarterInteractable duelStarter =
                    rival.AddComponent<DuelStarterInteractable>();

                duelStarter.rivalId = ModuleZRivalId.Madrid;

                rival.AddComponent<ModuleZTalkAnimation>();
                rival.AddComponent<ModuleZNPCFacePlayer>();
                rival.AddComponent<ModuleZNPCInteractionCollider>();
            }
            else
            {
                GameObject defeatedRival = CreateHumanNPC(
                    "NPC_Rival_Madrid_Derrotado",
                    position,
                    new Color(0.25f, 0.25f, 0.25f),
                    new Color(0.05f, 0.05f, 0.08f)
                );

                ModuleZRivalWorldHUDBuilder.Build(defeatedRival.transform, ModuleZRivalId.Madrid);

                DuelStarterInteractable duelStarter =
                    defeatedRival.AddComponent<DuelStarterInteractable>();

                duelStarter.rivalId = ModuleZRivalId.Madrid;
                duelStarter.allowRematchWhenDefeated = true;

                defeatedRival.AddComponent<ModuleZTalkAnimation>();
                defeatedRival.AddComponent<ModuleZNPCFacePlayer>();
                defeatedRival.AddComponent<ModuleZNPCInteractionCollider>();
            }
        }

        private void CreateAndaluciaRival()
        {
            Vector3 position = new Vector3(0f, 0f, -11f);

            if (!ModuleZGameState.AndaluciaUnlocked)
            {
                GameObject lockedRival = CreateHumanNPC(
                    "NPC_Rival_Andalucia_Bloqueado",
                    position,
                    new Color(0.18f, 0.18f, 0.18f),
                    new Color(0.05f, 0.05f, 0.05f)
                );

                ModuleZRivalWorldHUDBuilder.Build(lockedRival.transform,ModuleZRivalId.Andalucia);

                MessageInteractable message =
                    lockedRival.AddComponent<MessageInteractable>();

                message.interactionText = "Pulsa E para hablar";
                message.message =
                    "Aún no puedes retarme. Derrota primero a Madrid, Barcelona y Valencia.";

                lockedRival.AddComponent<ModuleZTalkAnimation>();
                lockedRival.AddComponent<ModuleZNPCFacePlayer>();
                lockedRival.AddComponent<ModuleZNPCInteractionCollider>();
                return;
            }

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

                ModuleZRivalWorldHUDBuilder.Build(rival.transform, ModuleZRivalId.Andalucia);

                rival.AddComponent<ModuleZTalkAnimation>();
                rival.AddComponent<ModuleZNPCFacePlayer>();
                rival.AddComponent<ModuleZNPCInteractionCollider>();
            }
            else
            {
                GameObject defeatedRival = CreateHumanNPC(
                    "NPC_Rival_Andalucia_Derrotado",
                    position,
                    new Color(0.25f, 0.25f, 0.25f),
                    new Color(0.10f, 0.08f, 0.05f)
                );

                ModuleZRivalWorldHUDBuilder.Build(defeatedRival.transform, ModuleZRivalId.Andalucia);

                DuelStarterInteractable duelStarter =
                    defeatedRival.AddComponent<DuelStarterInteractable>();

                duelStarter.rivalId = ModuleZRivalId.Andalucia;
                duelStarter.allowRematchWhenDefeated = true;

                defeatedRival.AddComponent<ModuleZTalkAnimation>();
                defeatedRival.AddComponent<ModuleZNPCFacePlayer>();
                defeatedRival.AddComponent<ModuleZNPCInteractionCollider>();
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
            GameObject luis = CreateHumanNPC(
                "NPC_Luis_Sentado_70s",
                new Vector3(-8f, 0f, 8f),
                new Color(0.40f, 0.30f, 0.20f),
                new Color(0.08f, 0.08f, 0.08f)
            );

            AddDialogue(
                luis,
                "Luis",
                new string[]
                {
                    "Me gusta observar los duelos desde aquí.",
                    "A veces pensar sentado ayuda más que correr."
                }
            );

            ModuleZNPCStaticPose luisPose =
                luis.AddComponent<ModuleZNPCStaticPose>();

            luisPose.Configure(ModuleZNPCStaticPose.PoseType.Sitting);

            GameObject maria = CreateHumanNPC(
                "NPC_Maria_Sentada_70s",
                new Vector3(8f, 0f, 8f),
                new Color(0.55f, 0.40f, 0.30f),
                new Color(0.15f, 0.10f, 0.10f)
            );

            AddDialogue(
                maria,
                "María",
                new string[]
                {
                    "Aquí se está muy tranquila.",
                    "Los mejores jugadores observan antes de actuar."
                }
            );

            ModuleZNPCStaticPose mariaPose =
                maria.AddComponent<ModuleZNPCStaticPose>();

            mariaPose.Configure(ModuleZNPCStaticPose.PoseType.Sitting);
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
                case ModuleZRivalId.Madrid:
                    return "Buen duelo. En Madrid sabes moverte.";

                case ModuleZRivalId.Andalucia:
                    return "Buen duelo. Andalucía también reconoce tu victoria.";

                case ModuleZRivalId.Barcelona:
                    return "Has resuelto el puzzle con estilo. Buen duelo.";

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

        private void CreateBarcelonaRival()
        {
            CreateRivalGeneric(
                "NPC_Rival_Barcelona",
                ModuleZRivalId.Barcelona,
                ModuleZGameState.RivalBarcelonaDefeated,
                new Vector3(-8f, 0f, -6f),
                new Color(0.18f, 0.28f, 0.55f),
                new Color(0.05f, 0.05f, 0.08f)
            );
        }

        private void CreateValenciaRival()
        {
            CreateRivalGeneric(
                "NPC_Rival_Valencia",
                ModuleZRivalId.Valencia,
                ModuleZGameState.RivalValenciaDefeated,
                new Vector3(8f, 0f, -6f),
                new Color(0.80f, 0.55f, 0.18f),
                new Color(0.08f, 0.08f, 0.08f)
            );
        }

        private void CreateRivalGeneric(
            string rivalName,
            ModuleZRivalId rivalId,
            bool defeated,
            Vector3 position,
            Color shirtColor,
            Color pantsColor)
        {
            if (!defeated)
            {
                GameObject rival = CreateHumanNPC(
                    rivalName,
                    position,
                    shirtColor,
                    pantsColor
                );

                DuelStarterInteractable duelStarter =
                    rival.AddComponent<DuelStarterInteractable>();

                duelStarter.rivalId = rivalId;

                ModuleZRivalWorldHUDBuilder.Build(rival.transform, rivalId);

                rival.AddComponent<ModuleZTalkAnimation>();
                rival.AddComponent<ModuleZNPCFacePlayer>();

                rival.AddComponent<ModuleZNPCInteractionCollider>();
            }
            else
            {
                GameObject defeatedRival = CreateHumanNPC(
                    rivalName + "_Derrotado",
                    position,
                    new Color(0.25f, 0.25f, 0.25f),
                    pantsColor
                );

                ModuleZRivalWorldHUDBuilder.Build(defeatedRival.transform, rivalId);

                DuelStarterInteractable duelStarter =
                    defeatedRival.AddComponent<DuelStarterInteractable>();

                duelStarter.rivalId = rivalId;
                duelStarter.allowRematchWhenDefeated = true;

                defeatedRival.AddComponent<ModuleZTalkAnimation>();
                defeatedRival.AddComponent<ModuleZNPCFacePlayer>();

                defeatedRival.AddComponent<ModuleZNPCInteractionCollider>();
            }
        }
    }
}