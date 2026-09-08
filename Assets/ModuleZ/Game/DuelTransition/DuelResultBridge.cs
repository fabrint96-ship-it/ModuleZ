using System;
using ModuleZ.OpenWorld.Encounters;

namespace ModuleZ.Game.DuelTransition
{
    public static class DuelResultBridge
    {
        public static bool Complete(
            DuelContext acceptedContext,
            DuelResult result,
            Action<DuelContext, DuelResult> compatibilityProcessor,
            out string failureReason)
        {
            if (!Enum.IsDefined(typeof(DuelOutcome), result.Outcome))
            {
                failureReason = "Duel outcome is invalid.";
                return false;
            }

            if (!Enum.IsDefined(typeof(ModuleZRivalId), result.RivalId))
            {
                failureReason = "Duel result rival is invalid.";
                return false;
            }

            if (result.RivalId != acceptedContext.RivalId)
            {
                failureReason =
                    "Duel result rival does not match the accepted context.";
                return false;
            }

            if (compatibilityProcessor == null)
            {
                failureReason = "Duel result processor is unavailable.";
                return false;
            }

            compatibilityProcessor(acceptedContext, result);
            failureReason = null;
            return true;
        }
    }
}
