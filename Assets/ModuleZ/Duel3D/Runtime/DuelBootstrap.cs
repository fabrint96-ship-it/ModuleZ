using System;
using ModuleZ.Core.Managers;
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

            if (!HasValidLegacyStartupState(out failureReason))
                return false;

            try
            {
                runtimeObject = new GameObject(RuntimeObjectName);
                runtimeObject.transform.SetParent(runtimeOwner, false);
                runtimeBuilder =
                    runtimeObject.AddComponent<Duel3DRuntimeBuilder>();

                Debug.Log(
                    "[ModuleZ] Production Duel3D runtime created."
                );
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

        private static bool HasValidLegacyStartupState(
            out string failureReason)
        {
            // Transitional F1.2 input boundary. Normal production entry uses
            // ModuleZDuelSessionState; the legacy pending-rival fallback keeps
            // direct scene/debug entry compatible until DuelContext migration.
            ModuleZRivalId rivalId = ModuleZDuelSessionState.HasActiveDuel
                ? ModuleZDuelSessionState.RivalId
                : ModuleZGameState.PendingDuelRival;

            if (!Enum.IsDefined(typeof(ModuleZRivalId), rivalId))
            {
                failureReason = "Legacy Duel rival state is invalid.";
                return false;
            }

            failureReason = null;
            return true;
        }
    }
}
