using ModuleZ.Core.Managers;
using ModuleZ.Core.SaveSystem;
using ModuleZ.Core.SceneLoading;
using ModuleZ.Game.DuelTransition;
using ModuleZ.OpenWorld.Encounters;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ModuleZ.Duel3D.Rules
{
    public class Duel3DResultManager : MonoBehaviour
    {
        public static Duel3DResultManager Instance { get; private set; }

        private bool resultResolved;
        private DuelContext acceptedContext;
        private bool hasAcceptedContext;
        private ulong acceptedSessionRevision;
        private bool acceptedFromSession;
        private bool pendingPersistence;
        private float returnDelay;
        private Text retryMessage;
        private GameObject retryCanvas;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public bool Initialize(DuelContext context)
        {
            return Initialize(context, ModuleZDuelSessionState.Revision,
                ModuleZDuelSessionState.HasActiveDuel);
        }

        internal bool Initialize(DuelContext context, ulong sessionRevision,
            bool fromSession)
        {
            if (hasAcceptedContext)
                return false;

            acceptedContext = context;
            acceptedSessionRevision = sessionRevision;
            acceptedFromSession = fromSession;
            hasAcceptedContext = true;
            return true;
        }

        public void Complete(DuelResult result)
        {
            if (resultResolved)
                return;

            if (!hasAcceptedContext)
            {
                Debug.LogError(
                    "[ModuleZ] Duel result has no accepted DuelContext."
                );
                return;
            }

            if (!acceptedFromSession)
            {
                if (DuelResultBridge.Complete(acceptedContext, result,
                        (context, acceptedResult) => { }, out string trialFailure))
                    ShowTrialMode();
                else
                    Debug.LogError("[ModuleZ] Duel result rejected: " + trialFailure);
                return;
            }

            if (!AcceptedSessionStillCurrent())
            {
                Debug.LogError("[ModuleZ] Duel result belongs to an obsolete session.");
                return;
            }

            if (!DuelResultBridge.Complete(
                    acceptedContext,
                    result,
                    ApplyCompatibilityResult,
                    out string failureReason))
            {
                Debug.LogError(
                    "[ModuleZ] Duel result rejected: " + failureReason
                );
            }
        }

        public void AbandonDuel()
        {
            if (!hasAcceptedContext)
            {
                Debug.LogError(
                    "[ModuleZ] Duel abandon has no accepted DuelContext."
                );
                return;
            }

            Complete(new DuelResult(
                DuelOutcome.Abandoned,
                acceptedContext.RivalId
            ));
        }

        private void ApplyCompatibilityResult(
            DuelContext context,
            DuelResult result)
        {
            if (resultResolved)
                return;

            if (!ModuleZSaveManager.BeginDuelResultApplication(
                    (int)context.RivalId, context.IsRematch,
                    context.ReturnPosition, acceptedSessionRevision))
            {
                Debug.LogError("[ModuleZ] Duel result application lost session ownership.");
                return;
            }

            resultResolved = true;

            switch (result.Outcome)
            {
                case DuelOutcome.Victory:
                    ApplyVictory(context, result);
                    break;

                case DuelOutcome.Defeat:
                    ApplyDefeat(context, result);
                    break;

                case DuelOutcome.Abandoned:
                    ApplyAbandon(context, result);
                    break;
            }

            if (!ModuleZSaveManager.ConfirmAppliedDuelResult(
                    (int)context.RivalId, context.IsRematch,
                    context.ReturnPosition, (int)result.RivalId,
                    acceptedSessionRevision))
            {
                Debug.LogError("[ModuleZ] Applied Duel result cannot be authorized for saving.");
                return;
            }
            returnDelay = result.Outcome == DuelOutcome.Abandoned ? 0.8f : 1.5f;
            PersistAppliedResult();
        }

        private bool AcceptedSessionStillCurrent()
        {
            return ModuleZDuelSessionState.HasActiveDuel &&
                ModuleZDuelSessionState.Revision == acceptedSessionRevision &&
                ModuleZDuelSessionState.RivalId == acceptedContext.RivalId &&
                ModuleZDuelSessionState.IsRematch == acceptedContext.IsRematch &&
                ModuleZDuelSessionState.ReturnPosition == acceptedContext.ReturnPosition;
        }

        private void ShowTrialMode()
        {
            if (resultResolved)
                return;
            resultResolved = true;
            ModuleZSaveManager.MarkDirectDuelAsNonPersistable();
            ShowSaveRetry(ModuleZSaveGameStatus.LifecycleRejected);
            retryMessage.text = "Duelo abierto en modo de prueba: el resultado no se aplica ni se guarda.";
            Button button = retryCanvas.GetComponentInChildren<Button>();
            if (button != null)
            {
                button.gameObject.name = "ExitTrialModeButton";
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ExitTrialMode);
                Text label = button.GetComponentInChildren<Text>();
                if (label != null)
                    label.text = "Salir sin guardar";
            }
        }

        private void ExitTrialMode()
        {
            if (acceptedFromSession || !resultResolved)
                return;
            if (ModuleZSceneController.Instance != null)
                ModuleZSceneController.Instance.ReturnToMainMenu();
            else
                // Standalone Duel has no application controller by design.
                // Boot is the safe no-save exit for this transitional mode.
                SceneManager.LoadScene(ModuleZSceneController.BootScene);
        }

        // This action only retries storage. The accepted result and all of its
        // gameplay effects have already been applied exactly once.
        public bool RetryPendingSave()
        {
            if (!resultResolved || !pendingPersistence)
                return false;
            return PersistAppliedResult();
        }

        private bool PersistAppliedResult()
        {
            ModuleZSaveGameStatus status = ModuleZSaveManager.TrySaveGame();
            if (status != ModuleZSaveGameStatus.Success)
            {
                pendingPersistence = true;
                ShowSaveRetry(status);
                Debug.LogWarning("[ModuleZ] Resultado aplicado, no guardado: " + status);
                return false;
            }

            pendingPersistence = false;
            if (retryCanvas != null)
                Destroy(retryCanvas);
            retryCanvas = null;
            retryMessage = null;
            ModuleZDuelSessionState.Clear();
            Invoke(nameof(ReturnToOpenWorld), returnDelay);
            return true;
        }

        private void ShowSaveRetry(ModuleZSaveGameStatus status)
        {
            if (retryCanvas == null)
            {
                retryCanvas = new GameObject("DuelResult_SaveRetry");
                retryCanvas.transform.SetParent(transform, false);
                Canvas canvas = retryCanvas.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 3000;
                retryCanvas.AddComponent<CanvasScaler>();
                retryCanvas.AddComponent<GraphicRaycaster>();
                if (EventSystem.current == null)
                {
                    GameObject events = new GameObject("DuelResult_RetryEventSystem");
                    events.transform.SetParent(retryCanvas.transform, false);
                    events.AddComponent<EventSystem>();
                    events.AddComponent<StandaloneInputModule>();
                }

                GameObject panel = new GameObject("SaveRetryPanel");
                panel.transform.SetParent(retryCanvas.transform, false);
                Image image = panel.AddComponent<Image>();
                image.color = new Color(0.05f, 0.08f, 0.12f, 0.96f);
                RectTransform panelRect = panel.GetComponent<RectTransform>();
                panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.sizeDelta = new Vector2(650f, 230f);

                GameObject label = new GameObject("SaveStatus");
                label.transform.SetParent(panel.transform, false);
                retryMessage = label.AddComponent<Text>();
                retryMessage.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                retryMessage.fontSize = 23;
                retryMessage.alignment = TextAnchor.MiddleCenter;
                retryMessage.color = Color.white;
                RectTransform labelRect = label.GetComponent<RectTransform>();
                labelRect.anchorMin = new Vector2(0.05f, 0.52f);
                labelRect.anchorMax = new Vector2(0.95f, 0.95f);
                labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;

                GameObject buttonObject = new GameObject("RetrySaveButton");
                buttonObject.transform.SetParent(panel.transform, false);
                buttonObject.AddComponent<Image>().color = new Color(0.16f, 0.32f, 0.55f);
                Button button = buttonObject.AddComponent<Button>();
                button.onClick.AddListener(() => RetryPendingSave());
                RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0.22f, 0.1f);
                buttonRect.anchorMax = new Vector2(0.78f, 0.38f);
                buttonRect.offsetMin = buttonRect.offsetMax = Vector2.zero;
                GameObject buttonLabel = new GameObject("Text");
                buttonLabel.transform.SetParent(buttonObject.transform, false);
                Text buttonText = buttonLabel.AddComponent<Text>();
                buttonText.text = "Reintentar guardado";
                buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                buttonText.fontSize = 23;
                buttonText.alignment = TextAnchor.MiddleCenter;
                buttonText.color = Color.white;
                RectTransform buttonLabelRect = buttonLabel.GetComponent<RectTransform>();
                buttonLabelRect.anchorMin = Vector2.zero;
                buttonLabelRect.anchorMax = Vector2.one;
                buttonLabelRect.offsetMin = buttonLabelRect.offsetMax = Vector2.zero;
            }
            retryMessage.text = "Resultado aplicado en memoria; NO guardado (" + status +
                "). Permanece en el duelo y reintenta.";
        }

        private void ApplyVictory(
            DuelContext context,
            DuelResult result)
        {
            ModuleZRivalId defeatedRival = result.RivalId;

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.DuelsWon++;

            string victoryMessage;

            if (context.IsRematch)
            {
                ModuleZGameState.RematchesWon++;

                victoryMessage =
                    "Victoria en rematch contra " +
                    GetRivalName(defeatedRival);
            }
            else
            {
                ModuleZRivalProgression.MarkRivalDefeated(defeatedRival);

                victoryMessage =
                    "Victoria contra " +
                    GetRivalName(defeatedRival);

                string unlockMessage =
                    ModuleZRivalProgression.GetUnlockMessage(defeatedRival);

                if (!string.IsNullOrEmpty(unlockMessage))
                    victoryMessage += "\n" + unlockMessage;
            }

            ModuleZGameState.LastDuelResultMessage = victoryMessage;
            ModuleZGameState.PendingOpenWorldMessage = victoryMessage;

            ModuleZGameState.CurrentDuelIsRematch = false;

            Debug.Log("[ModuleZ] Duel3D resultado: Victoria.");

            Debug.Log(
                "[ModuleZ DEBUG] CurrentDuelRival = " +
                defeatedRival
            );
        }

        private void ApplyDefeat(
            DuelContext context,
            DuelResult result)
        {
            ModuleZRivalId defeatedRival = result.RivalId;

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = true;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.DuelsLost++;

            if (context.IsRematch)
                ModuleZGameState.RematchesLost++;

            string defeatMessage = context.IsRematch
                ? "Derrota en rematch contra " + GetRivalName(defeatedRival)
                : "Derrota contra " + GetRivalName(defeatedRival);

            ModuleZGameState.LastDuelResultMessage = defeatMessage;
            ModuleZGameState.PendingOpenWorldMessage = defeatMessage;

            ModuleZGameState.CurrentDuelIsRematch = false;

            Debug.Log("[ModuleZ] Duel3D resultado: Derrota.");
        }

        private void ApplyAbandon(
            DuelContext context,
            DuelResult result)
        {
            ModuleZRivalId defeatedRival = result.RivalId;

            ModuleZGameState.DuelCompleted = true;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = true;
            ModuleZGameState.DuelsAbandoned++;

            if (context.IsRematch)
                ModuleZGameState.RematchesAbandoned++;

            string abandonMessage = context.IsRematch
                ? "Rematch abandonado contra " + GetRivalName(defeatedRival)
                : "Duelo abandonado contra " + GetRivalName(defeatedRival);

            ModuleZGameState.LastDuelResultMessage = abandonMessage;
            ModuleZGameState.PendingOpenWorldMessage = abandonMessage;

            ModuleZGameState.CurrentDuelIsRematch = false;

            Debug.Log("[ModuleZ] Duel3D resultado: Abandono.");
        }

        private void ReturnToOpenWorld()
        {
            ModuleZGameState.ReturningFromDuel = true;
            ModuleZGameState.IsPaused = false;

            if (ModuleZSceneController.Instance != null)
                ModuleZSceneController.Instance.LoadOpenWorld();
            else
                Debug.LogError("[ModuleZ] No existe ModuleZSceneController.");
        }

        private string GetRivalName(ModuleZRivalId rivalId)
        {
            switch (rivalId)
            {
                case ModuleZRivalId.Madrid:
                    return "Rival Madrid";

                case ModuleZRivalId.Barcelona:
                    return "Rival Barcelona";

                case ModuleZRivalId.Valencia:
                    return "Rival Valencia";

                case ModuleZRivalId.Andalucia:
                    return "Rival Andalucía";

                default:
                    return "Rival";
            }
        }
    }
}
