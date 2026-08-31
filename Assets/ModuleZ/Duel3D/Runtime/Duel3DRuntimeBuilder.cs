using System.Collections;
using System.Collections.Generic;
using ModuleZ.Core.Managers;
using ModuleZ.Duel3D.AI;
using ModuleZ.Duel3D.Board;
using ModuleZ.Duel3D.Core;
using ModuleZ.Duel3D.Feedback;
using ModuleZ.Duel3D.Pieces;
using ModuleZ.Duel3D.Rules;
using ModuleZ.Duel3D.UI;
using ModuleZ.Duel3D.Visuals;
using ModuleZ.OpenWorld.Encounters;
using ModuleZ.Duel3D.Audio;
using UnityEngine;

namespace ModuleZ.Duel3D.Runtime
{
    public class Duel3DRuntimeBuilder : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private Duel3DMatchConfig matchConfig;

        [Header("Board Fallback")]
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 6;
        [SerializeField] private int depth = 8;
        [SerializeField] private float cellSize = 0.45f;

        [Header("Piece Preview")]
        [SerializeField] private Vector3Int currentOrigin = new Vector3Int(2, 0, 2);
        [SerializeField] private ZPiece3DRotationState currentRotation = new ZPiece3DRotationState();

        [Header("AI")]
        [SerializeField] private Duel3DAISettings aiSettings;

        private Duel3DBoardGrid board;
        private Duel3DMatchResolver matchResolver;
        private Duel3DAIController aiController;
        private Duel3DHUDController hudController;
        private Duel3DAIDebugInfo aiDebugInfo;
        private Duel3DResultVisualController resultVisualController;
        private Duel3DGameFeedbackManager feedbackManager;

        private GameObject boardRoot;
        private GameObject cubesRoot;
        private GameObject previewRoot;
        private GameObject forbiddenRoot;

        private Material boardMaterial;
        private Material boundsMaterial;
        private Material playerMaterial;
        private Material opponentMaterial;
        private Material previewValidMaterial;
        private Material previewInvalidMaterial;
        private Material forbiddenMaterial;

        private readonly Dictionary<Vector3Int, GameObject> visualCubes =
            new Dictionary<Vector3Int, GameObject>();

        private bool playerTurn = true;
        private bool resolvingRemoval;

        private Vector3Int[] lastPlayerPieceCells;
        private Vector3Int[] lastOpponentPieceCells;

        private float aiProgress01;
        private Duel3DAISettings runtimeAISettings;

        private Duel3DRivalProfile rivalProfile;

        private float nextRivalAmbientCommentTime;
        private const float RivalAmbientCommentMinInterval = 12f;
        private const float RivalAmbientCommentMaxInterval = 22f;

        private bool nearVictoryReactionShown;

        private int previousPlayerCubeCount;
        private int previousOpponentCubeCount;

        private void Start()
        {
            BuildDuel3D();
        }

        private void Update()
        {
            if (board == null || matchResolver == null)
                return;

            if (ModuleZGameState.IsPaused)
            {
                UpdateHUD();
                return;
            }

            if (!matchResolver.MatchFinished && !resolvingRemoval)
            {
                HandlePlayerInput();
                RefreshPreview();
            }

            UpdateHUD();
            UpdateRivalAmbientComments();
            UpdateAIDebug();
        }

        private void BuildDuel3D()
        {
            if (ModuleZDuelSessionState.HasActiveDuel)
            {
                ModuleZGameState.CurrentDuelRival =
                    ModuleZDuelSessionState.RivalId;

                ModuleZGameState.CurrentDuelIsRematch =
                    ModuleZDuelSessionState.IsRematch;

                ModuleZGameState.OpenWorldReturnPosition =
                    ModuleZDuelSessionState.ReturnPosition;
            }

            ModuleZGameState.CurrentDuelRival = ModuleZGameState.PendingDuelRival;

            if (matchConfig == null)
                matchConfig = Duel3DMatchConfigProvider.CreateConfigForCurrentDuel();

            ApplyConfigValues();

            rivalProfile =
                Duel3DRivalProfileLibrary.Get(
                    ModuleZGameState.CurrentDuelRival
                );

            Duel3DCameraBuilder cameraBuilder = gameObject.AddComponent<Duel3DCameraBuilder>();
            Camera duelCamera = cameraBuilder.BuildCamera();

            if (matchConfig.useOrbitCamera)
            {
                Duel3DOrbitCameraController orbitCamera =
                    gameObject.AddComponent<Duel3DOrbitCameraController>();

                orbitCamera.Initialize(duelCamera, Vector3.zero);
            }

            CreateCityArena();

            Duel3DMusicController musicController =
                gameObject.AddComponent<Duel3DMusicController>();

            musicController.PlayCurrentDuelMusic();

            CreateMaterials();

            board = new Duel3DBoardGrid(width, height, depth);

            boardRoot = new GameObject("Duel3D_BoardRoot");
            cubesRoot = new GameObject("Duel3D_CubesRoot");
            previewRoot = new GameObject("Duel3D_PreviewRoot");
            forbiddenRoot = new GameObject("Duel3D_ForbiddenRoot");

            BuildBoardFrame();

            runtimeAISettings = GetRuntimeAISettings();
            aiController = new Duel3DAIController(runtimeAISettings, rivalProfile);

            matchResolver = gameObject.AddComponent<Duel3DMatchResolver>();
            matchResolver.Initialize(board);

            previousPlayerCubeCount = matchResolver.GetPlayerCubeCount();
            previousOpponentCubeCount = matchResolver.GetOpponentCubeCount();

            matchResolver.OnMatchFinished += HandleMatchFinished;

            if (FindObjectOfType<Duel3DResultManager>() == null)
            {
                gameObject.AddComponent<Duel3DResultManager>();
            }

            hudController = gameObject.AddComponent<Duel3DHUDController>();
            hudController.BuildHUD();

            ShowRivalIntroduction();
            MarkCurrentRivalPersonalityCompleted();

            gameObject.AddComponent<ModuleZ.UI.PauseMenu.DuelPauseMenuController>();

            resultVisualController =
                gameObject.AddComponent<Duel3DResultVisualController>();

            feedbackManager =
                gameObject.AddComponent<Duel3DGameFeedbackManager>();

            feedbackManager.Initialize(
                hudController,
                resultVisualController
            );

            // Oculta HUD IA
            if (false)
            {
                aiDebugInfo = gameObject.AddComponent<Duel3DAIDebugInfo>();
                aiDebugInfo.Build();
            }

            RefreshPreview();
            UpdateHUD();

            ScheduleNextRivalAmbientComment();

            Debug.Log("[ModuleZ] Duel3D Runtime iniciado con GameFeedbackManager.");
        }

        private void CheckMomentumShift()
        {
            if (hudController == null || matchResolver == null)
                return;

            int playerCubes = matchResolver.GetPlayerCubeCount();
            int opponentCubes = matchResolver.GetOpponentCubeCount();

            int playerGain =
                playerCubes - previousPlayerCubeCount;

            int opponentGain =
                opponentCubes - previousOpponentCubeCount;

            if (playerGain >= 5)
            {
                hudController.ShowActionMessage(
                    Duel3DRivalContextCommentLibrary.GetPlayerRecoveredComment(
                        ModuleZGameState.CurrentDuelRival
                    ),
                    4f
                );
            }
            else if (opponentGain >= 5)
            {
                hudController.ShowActionMessage(
                    Duel3DRivalContextCommentLibrary.GetOpponentRecoveredComment(
                        ModuleZGameState.CurrentDuelRival
                    ),
                    4f
                );
            }

            previousPlayerCubeCount = playerCubes;
            previousOpponentCubeCount = opponentCubes;
        }

        private void ScheduleNextRivalAmbientComment()
        {
            nextRivalAmbientCommentTime =
                Time.time +
                Random.Range(
                    RivalAmbientCommentMinInterval,
                    RivalAmbientCommentMaxInterval
                );
        }

        private void UpdateRivalAmbientComments()
        {
            if (hudController == null || matchResolver == null)
                return;

            if (matchResolver.MatchFinished)
                return;

            if (ModuleZGameState.IsPaused)
                return;

            if (Time.time < nextRivalAmbientCommentTime)
                return;

            string comment =
                Duel3DRivalAmbientCommentLibrary.GetRandomComment(
                    ModuleZGameState.CurrentDuelRival
                );

            if (!string.IsNullOrEmpty(comment))
                hudController.ShowActionMessage(comment, 4f);

            ScheduleNextRivalAmbientComment();
        }

        private void ShowRivalIntroduction()
        {
            if (hudController == null)
                return;

            string intro =
                Duel3DRivalIntroLibrary.GetIntro(
                    ModuleZGameState.CurrentDuelRival
                );

            hudController.ShowActionMessage(
                intro,
                4f
            );
        }

        private void MarkCurrentRivalPersonalityCompleted()
        {
            switch (ModuleZGameState.CurrentDuelRival)
            {
                case ModuleZRivalId.Madrid:
                    ModuleZGameState.RivalMadridPersonalityCompleted = true;
                    break;

                case ModuleZRivalId.Barcelona:
                    ModuleZGameState.RivalBarcelonaPersonalityCompleted = true;
                    break;

                case ModuleZRivalId.Valencia:
                    ModuleZGameState.RivalValenciaPersonalityCompleted = true;
                    break;

                case ModuleZRivalId.Andalucia:
                    ModuleZGameState.RivalAndaluciaPersonalityCompleted = true;
                    break;
            }
        }

        private void CreateCityArena()
        {
            GameObject arenaObj = new GameObject("Duel3D_CityArena");
            arenaObj.AddComponent<Duel3DCityArenaBuilder>().Build();
        }

        private void ApplyConfigValues()
        {
            width = Mathf.Max(1, matchConfig.boardWidth);
            height = Mathf.Max(1, matchConfig.boardHeight);
            depth = Mathf.Max(1, matchConfig.boardDepth);
        }

        private Duel3DAISettings GetRuntimeAISettings()
        {
            if (aiSettings != null)
                return aiSettings;

            if (matchConfig.overrideAIScaling)
            {
                aiProgress01 = Mathf.Clamp01(matchConfig.aiProgress01);
                return Duel3DAIDifficultyScaler.CreateScaledSettings(aiProgress01);
            }

            float rivalProgress =
                Duel3DAIDifficultyScaler.GetProgressByRivalsDefeated(
                    ModuleZGameState.RivalMadridDefeated,
                    ModuleZGameState.RivalBarcelonaDefeated,
                    ModuleZGameState.RivalValenciaDefeated,
                    ModuleZGameState.RivalAndaluciaDefeated
                );

            float duelWinProgress =
                Duel3DAIDifficultyScaler.GetProgressByDuelWins(
                    ModuleZGameState.DuelsWon
                );

            float rematchProgress =
                Duel3DAIDifficultyScaler.GetProgressByRematchesWon(
                    ModuleZGameState.RematchesWon
                );

            aiProgress01 =
                Duel3DAIDifficultyScaler.CombineProgress(
                    rivalProgress,
                    duelWinProgress,
                    rematchProgress
                );

            return Duel3DAIDifficultyScaler.CreateScaledSettings(aiProgress01);
        }

        private void HandlePlayerInput()
        {
            if (!playerTurn)
                return;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                currentOrigin.x--;

            if (Input.GetKeyDown(KeyCode.RightArrow))
                currentOrigin.x++;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                currentOrigin.z--;

            if (Input.GetKeyDown(KeyCode.UpArrow))
                currentOrigin.z++;

            if (Input.GetKeyDown(KeyCode.R))
                currentOrigin.y++;

            if (Input.GetKeyDown(KeyCode.F))
                currentOrigin.y--;

            if (Input.GetKeyDown(KeyCode.Q))
                currentRotation.RotateYawNegative();

            if (Input.GetKeyDown(KeyCode.E))
                currentRotation.RotateYawPositive();

            if (Input.GetKeyDown(KeyCode.Z))
                currentRotation.RotatePitchNegative();

            if (Input.GetKeyDown(KeyCode.X))
                currentRotation.RotatePitchPositive();

            if (Input.GetKeyDown(KeyCode.C))
                currentRotation.RotateRollNegative();

            if (Input.GetKeyDown(KeyCode.V))
                currentRotation.RotateRollPositive();

            ClampCurrentOrigin();

            if (Input.GetKeyDown(KeyCode.Space))
                TryPlacePlayerPiece();
        }

        private void TryPlacePlayerPiece()
        {
            Vector3Int[] playerPieceCells =
                ZPiece3DShape.GetCells(currentOrigin, currentRotation);

            if (matchConfig.useLastPieceRestriction &&
                Duel3DPiecePlacement.TouchesForbiddenCells(
                    board,
                    playerPieceCells,
                    lastPlayerPieceCells))
            {
                feedbackManager?.PlayInvalid("No puedes tocar tu última pieza Z");
                return;
            }

            bool placed = Duel3DPiecePlacement.PlacePiece(
                board,
                currentOrigin,
                currentRotation,
                Duel3DCellOwner.Player
            );

            if (!placed)
            {
                feedbackManager?.PlayInvalid("Posición inválida");
                return;
            }

            lastPlayerPieceCells = playerPieceCells;

            CreateVisualCubesForCells(playerPieceCells, Duel3DCellOwner.Player, true);
            feedbackManager?.PlayPlace();

            AfterPiecePlaced(Duel3DCellOwner.Player, false);

            if (matchResolver.MatchFinished)
                return;

            playerTurn = false;

            ClearChildren(previewRoot.transform);
            ClearChildren(forbiddenRoot.transform);

            UpdateHUD();

            feedbackManager?.PlayTurnChanged(false);

            Invoke(nameof(PlayOpponentTurn), rivalProfile.reactionDelay);
        }

        private void PlayOpponentTurn()
        {
            if (matchResolver.MatchFinished)
                return;

            bool placed = TryPlaceOpponentAIPiece();

            if (!placed)
            {
                Debug.Log("[ModuleZ] IA no encontró jugada válida.");
                matchResolver.FinishMatch(matchResolver.EvaluateTimeExpiredWinner());
                return;
            }

            feedbackManager?.PlayPlace();

            AfterPiecePlaced(Duel3DCellOwner.Opponent, true);

            if (matchResolver.MatchFinished)
                return;

            playerTurn = true;

            UpdateHUD();

            feedbackManager?.PlayTurnChanged(true);
        }

        private bool TryPlaceOpponentAIPiece()
        {
            if (aiController == null)
                aiController = new Duel3DAIController(runtimeAISettings, rivalProfile);

            Vector3Int bestOrigin;
            int bestRotation;

            bool foundMove = aiController.TryFindBestMove(
                board,
                lastOpponentPieceCells,
                out bestOrigin,
                out bestRotation
            );

            if (!foundMove)
                return false;

            if (rivalProfile != null && Random.value < rivalProfile.mistakeChance)
            {
                bestOrigin += new Vector3Int(
                    Random.Range(-1, 2),
                    0,
                    Random.Range(-1, 2)
                );

                ClampAIMoveOrigin(ref bestOrigin);
            }

            Vector3Int[] opponentPieceCells =
                Duel3DPiecePlacement.GetPreviewCells(bestOrigin, bestRotation);

            bool placed = Duel3DPiecePlacement.PlacePiece(
                board,
                bestOrigin,
                bestRotation,
                Duel3DCellOwner.Opponent
            );

            if (!placed)
                return false;

            lastOpponentPieceCells = opponentPieceCells;

            CreateVisualCubesForCells(
                opponentPieceCells,
                Duel3DCellOwner.Opponent,
                true
            );

            Debug.Log(
                $"[ModuleZ] IA coloca pieza en {bestOrigin} rot={bestRotation}"
            );

            return true;
        }

        private void ClampAIMoveOrigin(ref Vector3Int origin)
        {
            origin.x = Mathf.Clamp(origin.x, 0, width - 1);
            origin.y = Mathf.Clamp(origin.y, 0, height - 1);
            origin.z = Mathf.Clamp(origin.z, 0, depth - 1);
        }

        private void AfterPiecePlaced(Duel3DCellOwner owner, bool evaluateVictory)
        {
            matchResolver.RegisterPiecePlaced(owner);

            List<Vector3Int> removedCells =
                Duel3DGroupResolver.ResolveGroupsAndReturnCells(board);

            if (removedCells.Count > 0)
            {
                StartCoroutine(PlayRemoveEffectRoutine(removedCells, evaluateVictory));
                return;
            }

            RefreshPreview();
            UpdateHUD();
            CheckMomentumShift();
            CheckNearVictoryReaction();

            if (evaluateVictory)
                matchResolver.EvaluateImmediateVictory();
        }

        private IEnumerator PlayRemoveEffectRoutine(
            List<Vector3Int> removedCells,
            bool evaluateVictory)
        {
            resolvingRemoval = true;

            feedbackManager?.PlayRemove();

            ShowBigMoveReaction(removedCells.Count);

            for (int i = 0; i < removedCells.Count; i++)
            {
                Vector3Int cell = removedCells[i];

                if (!visualCubes.TryGetValue(cell, out GameObject cube))
                    continue;

                Duel3DRemoveEffect effect = cube.AddComponent<Duel3DRemoveEffect>();
                effect.PlayAndDestroy();
            }

            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < removedCells.Count; i++)
            {
                Vector3Int cell = removedCells[i];

                if (visualCubes.ContainsKey(cell))
                    visualCubes.Remove(cell);
            }

            RefreshPreview();
            UpdateHUD();

            resolvingRemoval = false;

            if (evaluateVictory)
                matchResolver.EvaluateImmediateVictory();
        }

        private void CheckNearVictoryReaction()
        {
            if (nearVictoryReactionShown)
                return;

            if (hudController == null || matchResolver == null)
                return;

            int playerCubes = matchResolver.GetPlayerCubeCount();
            int opponentCubes = matchResolver.GetOpponentCubeCount();

            int difference = opponentCubes - playerCubes;

            if (difference < 8)
                return;

            nearVictoryReactionShown = true;

            hudController.ShowActionMessage(
                Duel3DRivalReactionLibrary.GetNearVictoryReaction(
                    ModuleZGameState.CurrentDuelRival
                ),
                4f
            );
        }

        private void ShowBigMoveReaction(int removedCount)
        {
            if (hudController == null)
                return;

            if (removedCount < 4)
                return;

            string message =
                playerTurn
                    ? Duel3DRivalReactionLibrary.GetPlayerBigMoveReaction(
                        ModuleZGameState.CurrentDuelRival
                    )
                    : Duel3DRivalReactionLibrary.GetOpponentBigMoveReaction(
                        ModuleZGameState.CurrentDuelRival
                    );

            hudController.ShowActionMessage(message, 4f);
        }

        private void CreateVisualCubesForCells(
            Vector3Int[] cells,
            Duel3DCellOwner owner,
            bool playPlaceEffect)
        {
            if (cells == null)
                return;

            for (int i = 0; i < cells.Length; i++)
            {
                Vector3Int cell = cells[i];

                if (visualCubes.ContainsKey(cell))
                    continue;

                Material material = owner == Duel3DCellOwner.Player
                    ? playerMaterial
                    : opponentMaterial;

                GameObject cube = Duel3DPieceVisualBuilder.CreatePlacedCube(
                    $"Cube_{owner}_{cell.x}_{cell.y}_{cell.z}",
                    cubesRoot.transform,
                    GridToWorld(cell),
                    cellSize * 0.9f,
                    material
                );

                visualCubes[cell] = cube;

                if (playPlaceEffect)
                {
                    Duel3DPlaceEffect effect = cube.AddComponent<Duel3DPlaceEffect>();
                    effect.Play();
                }
            }
        }

        private void HandleMatchFinished(Duel3DMatchResult result)
        {
            Debug.Log("[ModuleZ] Resultado Duel3D: " + result);

            if (hudController != null)
                hudController.ShowResult(result);
            
            feedbackManager?.PlayResult(result, cubesRoot.transform);

            ShowRivalResultMessage(result);

            if (Duel3DResultManager.Instance == null)
                return;

            switch (result)
            {
                case Duel3DMatchResult.PlayerWin:
                    Duel3DResultManager.Instance.WinDuel();
                    break;

                case Duel3DMatchResult.OpponentWin:
                    Duel3DResultManager.Instance.LoseDuel();
                    break;

                case Duel3DMatchResult.Draw:
                    Duel3DResultManager.Instance.LoseDuel();
                    break;
            }
        }

        private void ShowRivalResultMessage(
            Duel3DMatchResult result)
        {
            if (hudController == null)
                return;

            string message;

            switch (result)
            {
                case Duel3DMatchResult.PlayerWin:

                    message =
                        Duel3DRivalVictoryLibrary
                            .GetPlayerVictoryMessage(
                                ModuleZGameState.CurrentDuelRival
                            );
                    break;

                case Duel3DMatchResult.OpponentWin:

                    message =
                        Duel3DRivalVictoryLibrary
                            .GetPlayerDefeatMessage(
                                ModuleZGameState.CurrentDuelRival
                            );
                    break;

                default:

                    message =
                        Duel3DRivalVictoryLibrary
                            .GetDrawMessage(
                                ModuleZGameState.CurrentDuelRival
                            );
                    break;
            }

            hudController.ShowActionMessage(
                message,
                5f
            );
        }

        private void UpdateHUD()
        {
            if (hudController == null || matchResolver == null)
                return;

            hudController.UpdateHUD(
                matchResolver.RemainingTime,
                matchResolver.GetPlayerCubeCount(),
                matchResolver.GetOpponentCubeCount(),
                playerTurn,
                currentOrigin,
                currentRotation
            );
        }

        private void UpdateAIDebug()
        {
            if (aiDebugInfo == null || runtimeAISettings == null)
                return;

            aiDebugInfo.UpdateInfo(
                aiProgress01,
                runtimeAISettings
            );
        }

        private void BuildBoardFrame()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Duel3D_BoardBase";
            floor.transform.SetParent(boardRoot.transform);

            floor.transform.position = new Vector3(
                0f,
                -cellSize * 0.65f,
                0f
            );

            floor.transform.localScale = new Vector3(
                width * cellSize,
                0.08f,
                depth * cellSize
            );

            floor.GetComponent<Renderer>().material = boardMaterial;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    marker.name = $"CellMarker_{x}_{z}";
                    marker.transform.SetParent(boardRoot.transform);

                    marker.transform.position =
                        GridToWorld(new Vector3Int(x, -1, z)) + Vector3.up * 0.08f;

                    marker.transform.localScale = new Vector3(
                        cellSize * 0.12f,
                        cellSize * 0.04f,
                        cellSize * 0.12f
                    );

                    marker.GetComponent<Renderer>().material = boardMaterial;
                }
            }

            Duel3DBoardBoundsBuilder.BuildBounds(
                boardRoot.transform,
                width,
                height,
                depth,
                cellSize,
                boundsMaterial
            );
        }

        private void RefreshPreview()
        {
            ClearChildren(previewRoot.transform);
            ClearChildren(forbiddenRoot.transform);

            if (!playerTurn || resolvingRemoval || matchResolver.MatchFinished)
                return;

            Vector3Int[] cells = ZPiece3DShape.GetCells(
                currentOrigin,
                currentRotation
            );

            if (matchConfig.showForbiddenCells)
            {
                Duel3DForbiddenCellsPreview.Build(
                    forbiddenRoot.transform,
                    board,
                    lastPlayerPieceCells,
                    GridToWorld,
                    cellSize,
                    forbiddenMaterial
                );
            }

            bool valid =
                Duel3DPiecePlacement.CanPlacePiece(board, currentOrigin, currentRotation) &&
                (!matchConfig.useLastPieceRestriction ||
                 !Duel3DPiecePlacement.TouchesForbiddenCells(
                     board,
                     cells,
                     lastPlayerPieceCells
                 ));

            for (int i = 0; i < cells.Length; i++)
            {
                if (!board.IsInside(cells[i]))
                    continue;

                Duel3DPieceVisualBuilder.CreatePreviewCube(
                    "PreviewCube",
                    previewRoot.transform,
                    GridToWorld(cells[i]),
                    cellSize * 0.82f,
                    valid ? previewValidMaterial : previewInvalidMaterial,
                    valid
                );
            }
        }

        private Vector3 GridToWorld(Vector3Int cell)
        {
            float offsetX = -(width - 1) * cellSize * 0.5f;
            float offsetZ = -(depth - 1) * cellSize * 0.5f;

            return new Vector3(
                offsetX + cell.x * cellSize,
                cell.y * cellSize,
                offsetZ + cell.z * cellSize
            );
        }

        private void ClampCurrentOrigin()
        {
            Vector3Int[] shape = ZPiece3DShape.GetRotatedShape(currentRotation);

            int maxX = 0;
            int maxY = 0;
            int maxZ = 0;

            for (int i = 0; i < shape.Length; i++)
            {
                maxX = Mathf.Max(maxX, shape[i].x);
                maxY = Mathf.Max(maxY, shape[i].y);
                maxZ = Mathf.Max(maxZ, shape[i].z);
            }

            currentOrigin.x = Mathf.Clamp(currentOrigin.x, 0, width - 1 - maxX);
            currentOrigin.y = Mathf.Clamp(currentOrigin.y, 0, height - 1 - maxY);
            currentOrigin.z = Mathf.Clamp(currentOrigin.z, 0, depth - 1 - maxZ);
        }

        private void CreateMaterials()
        {
            Shader shader = Shader.Find("Standard");

            playerMaterial =
                CreateMaterial(shader, matchConfig.playerColor);

            opponentMaterial =
                CreateMaterial(shader, matchConfig.opponentColor);

            boardMaterial =
                CreateMaterial(shader, matchConfig.boardColor);

            boundsMaterial =
                CreateMaterial(shader, matchConfig.boundsColor);

            forbiddenMaterial =
                CreateMaterial(shader, matchConfig.forbiddenColor);

            previewValidMaterial =
                CreateMaterial(shader, new Color(
                    matchConfig.playerColor.r,
                    matchConfig.playerColor.g,
                    matchConfig.playerColor.b,
                    0.25f
                ));

            previewInvalidMaterial =
                CreateMaterial(shader, new Color(1f, 0.15f, 0.15f, 0.25f));
        }

        private Material CreateMaterial(Shader shader, Color color)
        {
            Material material = new Material(shader);
            material.color = color;

            if (color.a < 0.99f)
            {
                material.SetFloat("_Mode", 3);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);

                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

                material.renderQueue = 3000;
            }

            return material;
        }

        private void ClearChildren(Transform parent)
        {
            if (parent == null)
                return;

            for (int i = parent.childCount - 1; i >= 0; i--)
                Destroy(parent.GetChild(i).gameObject);
        }
    }
}