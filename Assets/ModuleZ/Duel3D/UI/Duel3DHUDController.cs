using ModuleZ.Core.Managers;
using ModuleZ.Core.Theme;
using ModuleZ.Duel3D.Core;
using ModuleZ.Duel3D.Pieces;
using ModuleZ.Duel3D.Rules;
using ModuleZ.OpenWorld.Encounters;
using ModuleZ.Duel3D.AI;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleZ.Duel3D.UI
{
    public class Duel3DHUDController : MonoBehaviour
    {
        private Canvas canvas;

        private Text titleText;
        private Text timerText;
        private Text turnText;
        private Text scoreText;
        private Text positionText;
        private Text rotationText;
        private Text controlsText;
        private Text resultText;
        private Text actionMessageText;

        private float actionMessageUntil;

        public void BuildHUD()
        {
            if (canvas != null)
                return;

            GameObject canvasObj = new GameObject("Duel3D_HUD");
            canvasObj.transform.SetParent(transform, false);

            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            CreateTopPanel();
            CreateControlsPanel();
            CreateResultText();
            CreateActionMessageText();
        }

        public void UpdateHUD(
            float remainingTime,
            int playerCubes,
            int opponentCubes,
            bool playerTurn,
            Vector3Int currentOrigin,
            ZPiece3DRotationState currentRotation)
        {
            if (canvas == null)
                BuildHUD();

            titleText.text = "DUELO 3D — " + GetRivalName();

            timerText.text =
                "TIEMPO: " +
                Mathf.CeilToInt(remainingTime).ToString();

            turnText.text = playerTurn
                ? "TURNO: PLAYER"
                : "TURNO: IA";

            turnText.color = playerTurn
                ? ModuleZ70sPalette.OliveGreen
                : ModuleZ70sPalette.CabinaRed;

            scoreText.text =
                "CUBOS  PLAYER: " +
                playerCubes +
                "  |  IA: " +
                opponentCubes;

            positionText.text =
                "POS: X " +
                currentOrigin.x +
                " / Y " +
                currentOrigin.y +
                " / Z " +
                currentOrigin.z;

            rotationText.text =
                "ROT: Yaw " +
                currentRotation.yaw +
                " | Pitch " +
                currentRotation.pitch +
                " | Roll " +
                currentRotation.roll;

            UpdateActionMessage();
        }

        public void ShowActionMessage(string message, float duration)
        {
            if (canvas == null)
                BuildHUD();

            actionMessageText.text = message;
            actionMessageText.gameObject.SetActive(true);

            actionMessageUntil = Time.time + duration;
        }

        public void ShowResult(Duel3DMatchResult result)
        {
            if (resultText == null)
                return;

            resultText.gameObject.SetActive(true);

            switch (result)
            {
                case Duel3DMatchResult.PlayerWin:
                    resultText.text = "VICTORIA";
                    resultText.color = ModuleZ70sPalette.OliveGreen;
                    break;

                case Duel3DMatchResult.OpponentWin:
                    resultText.text = "DERROTA";
                    resultText.color = ModuleZ70sPalette.CabinaRed;
                    break;

                case Duel3DMatchResult.Draw:
                    resultText.text = "EMPATE";
                    resultText.color = ModuleZ70sPalette.Orange;
                    break;

                default:
                    resultText.text = result.ToString();
                    resultText.color = ModuleZ70sPalette.UIText;
                    break;
            }
        }

        private void CreateTopPanel()
        {
            GameObject panel = CreatePanel(
                "Duel3D_TopPanel",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -20f),
                new Vector2(980f, 165f),
                ModuleZ70sPalette.UIBackground
            );

            CreateAccentLine(panel.transform);

            titleText = CreateText(
                "TitleText",
                panel.transform,
                new Vector2(0f, -12f),
                new Vector2(930f, 34f),
                28,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.UIText
            );

            timerText = CreateText(
                "TimerText",
                panel.transform,
                new Vector2(-330f, -55f),
                new Vector2(250f, 30f),
                22,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.Orange
            );

            turnText = CreateText(
                "TurnText",
                panel.transform,
                new Vector2(0f, -55f),
                new Vector2(300f, 30f),
                22,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.OliveGreen
            );

            scoreText = CreateText(
                "ScoreText",
                panel.transform,
                new Vector2(320f, -55f),
                new Vector2(360f, 30f),
                21,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.UIText
            );

            positionText = CreateText(
                "PositionText",
                panel.transform,
                new Vector2(-230f, -105f),
                new Vector2(360f, 28f),
                19,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.FadedBlue
            );

            rotationText = CreateText(
                "RotationText",
                panel.transform,
                new Vector2(250f, -105f),
                new Vector2(470f, 28f),
                19,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.FadedBlue
            );
        }

        private void CreateControlsPanel()
        {
            GameObject panel = CreatePanel(
                "Duel3D_ControlsPanel",
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(25f, 25f),
                new Vector2(455f, 160f),
                ModuleZ70sPalette.UIBackground
            );

            CreateAccentLine(panel.transform);

            controlsText = CreateText(
                "ControlsText",
                panel.transform,
                new Vector2(20f, -15f),
                new Vector2(410f, 130f),
                18,
                TextAnchor.UpperLeft,
                ModuleZ70sPalette.UIText
            );

            controlsText.text =
                "CONTROLES\n" +
                "Flechas: mover pieza\n" +
                "R / F: altura\n" +
                "Q / E: yaw   Z / X: pitch\n" +
                "C / V: roll   Espacio: colocar\n" +
                "ESC: pausa";
        }

        private void CreateResultText()
        {
            resultText = CreateText(
                "ResultText",
                canvas.transform,
                new Vector2(0f, 0f),
                new Vector2(900f, 110f),
                64,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.UIText
            );

            RectTransform rect = resultText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            resultText.fontStyle = FontStyle.Bold;
            resultText.gameObject.SetActive(false);
        }

        private void CreateActionMessageText()
        {
            actionMessageText = CreateText(
                "ActionMessageText",
                canvas.transform,
                new Vector2(0f, -190f),
                new Vector2(850f, 45f),
                26,
                TextAnchor.MiddleCenter,
                ModuleZ70sPalette.Orange
            );

            RectTransform rect = actionMessageText.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -190f);

            actionMessageText.fontStyle = FontStyle.Bold;
            actionMessageText.gameObject.SetActive(false);
        }

        private void UpdateActionMessage()
        {
            if (actionMessageText == null)
                return;

            if (!actionMessageText.gameObject.activeSelf)
                return;

            if (Time.time >= actionMessageUntil)
                actionMessageText.gameObject.SetActive(false);
        }

        private GameObject CreatePanel(
            string name,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 position,
            Vector2 size,
            Color color)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(canvas.transform, false);

            Image image = obj.AddComponent<Image>();
            image.color = color;

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            return obj;
        }

        private void CreateAccentLine(Transform parent)
        {
            GameObject accentObj = new GameObject("AccentLine");
            accentObj.transform.SetParent(parent, false);

            Image accent = accentObj.AddComponent<Image>();
            accent.color = ModuleZ70sPalette.UIAccent;

            RectTransform rect = accentObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(8f, 0f);
            rect.anchoredPosition = Vector2.zero;
        }

        private Text CreateText(
            string name,
            Transform parent,
            Vector2 position,
            Vector2 size,
            int fontSize,
            TextAnchor alignment,
            Color color)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            Text text = obj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;

            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            return text;
        }

        private string GetRivalName()
        {
            Duel3DRivalProfile profile =
                Duel3DRivalProfileLibrary.Get(
                    ModuleZGameState.CurrentDuelRival
                );

            if (profile != null &&
                !string.IsNullOrEmpty(profile.displayName))
            {
                return profile.displayName;
            }

            return "Rival";
        }
    }
}