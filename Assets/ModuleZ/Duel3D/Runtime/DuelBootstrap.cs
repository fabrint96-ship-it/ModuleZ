using System;
using ModuleZ.Core.Managers;
using ModuleZ.Game.DuelTransition;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;

namespace ModuleZ.Duel3D.Runtime
{
    [DisallowMultipleComponent]
    public sealed class DuelBootstrap : MonoBehaviour
    {
        private const string RuntimeObjectName = "Duel3D_Runtime";

        public bool TryCreateRuntime(
            Transform runtimeOwner,
            out GameObject runtimeObject,
            out Duel3DRuntimeBuilder runtimeBuilder,
            out string failureReason)
        {
            runtimeObject = null;
            runtimeBuilder = null;
            failureReason = null;

            if (runtimeOwner == null)
            {
                failureReason = "A scene-scoped runtime owner is required.";
                return false;
            }

            if (!TryResolveAcceptedContext(
                    out DuelContext acceptedContext,
                    out failureReason))
                return false;

            try
            {
                runtimeObject = new GameObject(RuntimeObjectName);
                runtimeObject.transform.SetParent(runtimeOwner, false);
                runtimeBuilder =
                    runtimeObject.AddComponent<Duel3DRuntimeBuilder>();

                if (!runtimeBuilder.Initialize(acceptedContext))
                {
                    failureReason =
                        "Duel3D runtime rejected the accepted DuelContext.";
                    Destroy(runtimeObject);
                    runtimeObject = null;
                    runtimeBuilder = null;
                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                failureReason =
                    "Duel3D runtime creation failed: " + exception.Message;

                if (runtimeObject != null)
                    Destroy(runtimeObject);

                runtimeObject = null;
                runtimeBuilder = null;
                return false;
            }
        }

        private static bool TryResolveAcceptedContext(
            out DuelContext acceptedContext,
            out string failureReason)
        {
            if (ModuleZDuelSessionState.HasActiveDuel)
                return TryResolveActiveSession(
                    out acceptedContext,
                    out failureReason
                );

            return TryResolveDirectSceneFallback(
                out acceptedContext,
                out failureReason
            );
        }

        private static bool TryResolveActiveSession(
            out DuelContext acceptedContext,
            out string failureReason)
        {
            acceptedContext = new DuelContext(
                ModuleZDuelSessionState.RivalId,
                ModuleZDuelSessionState.IsRematch,
                ModuleZDuelSessionState.ReturnPosition
            );

            return TryValidateAcceptedContext(
                acceptedContext,
                "Active Duel session rival is invalid.",
                out failureReason
            );
        }

        private static bool TryResolveDirectSceneFallback(
            out DuelContext acceptedContext,
            out string failureReason)
        {
            Debug.LogWarning(
                "[ModuleZ] DuelBootstrap using transitional direct-scene fallback."
            );

            acceptedContext = new DuelContext(
                ModuleZGameState.PendingDuelRival,
                ModuleZGameState.CurrentDuelIsRematch,
                ModuleZGameState.OpenWorldReturnPosition
            );

            return TryValidateAcceptedContext(
                acceptedContext,
                "Direct-scene fallback rival is invalid.",
                out failureReason
            );
        }

        private static bool TryValidateAcceptedContext(
            DuelContext acceptedContext,
            string invalidRivalReason,
            out string failureReason)
        {

            if (!Enum.IsDefined(
                    typeof(ModuleZRivalId),
                    acceptedContext.RivalId))
            {
                failureReason = invalidRivalReason;
                return false;
            }

            failureReason = null;
            return true;
        }
    }
}
