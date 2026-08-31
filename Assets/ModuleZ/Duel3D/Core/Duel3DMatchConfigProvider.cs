using ModuleZ.Core.Managers;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Duel3D.Core
{
    public static class Duel3DMatchConfigProvider
    {
        public static Duel3DMatchConfig CreateConfigForCurrentDuel()
        {
            return CreateConfigForRival(ModuleZGameState.CurrentDuelRival);
        }

        public static Duel3DMatchConfig CreateConfigForRival(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return CreateMadridConfig();

                case ModuleZRivalId.Barcelona:
                    return CreateBarcelonaConfig();

                case ModuleZRivalId.Valencia:
                    return CreateValenciaConfig();

                case ModuleZRivalId.Andalucia:
                    return CreateAndaluciaConfig();

                default:
                    return Duel3DMatchConfig.CreateDefault();
            }
        }

        private static Duel3DMatchConfig CreateMadridConfig()
        {
            Duel3DMatchConfig config = Duel3DMatchConfig.CreateDefault();

            config.boardWidth = 8;
            config.boardHeight = 5;
            config.boardDepth = 8;
            config.matchDurationSeconds = 210f;

            config.overrideAIScaling = true;
            config.aiProgress01 = 0.15f;

            config.showAIDebug = true;
            config.showForbiddenCells = true;
            config.useOrbitCamera = true;

            return config;
        }

        private static Duel3DMatchConfig CreateBarcelonaConfig()
        {
            Duel3DMatchConfig config = Duel3DMatchConfig.CreateDefault();

            config.boardWidth = 8;
            config.boardHeight = 6;
            config.boardDepth = 8;
            config.matchDurationSeconds = 190f;

            config.overrideAIScaling = true;
            config.aiProgress01 = 0.35f;

            config.showAIDebug = true;
            config.showForbiddenCells = true;
            config.useOrbitCamera = true;

            return config;
        }

        private static Duel3DMatchConfig CreateValenciaConfig()
        {
            Duel3DMatchConfig config = Duel3DMatchConfig.CreateDefault();

            config.boardWidth = 9;
            config.boardHeight = 6;
            config.boardDepth = 9;
            config.matchDurationSeconds = 170f;

            config.overrideAIScaling = true;
            config.aiProgress01 = 0.60f;

            config.showAIDebug = true;
            config.showForbiddenCells = true;
            config.useOrbitCamera = true;

            return config;
        }

        private static Duel3DMatchConfig CreateAndaluciaConfig()
        {
            Duel3DMatchConfig config = Duel3DMatchConfig.CreateDefault();

            config.boardWidth = 10;
            config.boardHeight = 7;
            config.boardDepth = 10;
            config.matchDurationSeconds = 150f;

            config.overrideAIScaling = true;
            config.aiProgress01 = 0.85f;

            config.showAIDebug = true;
            config.showForbiddenCells = true;
            config.useOrbitCamera = true;

            return config;
        }
    }
}