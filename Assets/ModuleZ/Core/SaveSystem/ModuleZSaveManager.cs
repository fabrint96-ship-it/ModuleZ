using System;
using System.IO;
using System.Collections.Generic;
using ModuleZ.Core.Managers;
using ModuleZ.Core.Achievements;
using ModuleZ.Core.SceneLoading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ModuleZ.Core.SaveSystem
{
    public enum ModuleZLoadGameStatus
    {
        Success,
        NotFound,
        InvalidData,
        ReadError,
        ApplyError
    }

    public enum ModuleZSaveGameStatus
    {
        Success,
        CaptureError,
        SerializationError,
        TemporaryWriteError,
        ExistingSaveInvalid,
        PublicationError,
        UnsupportedPlatform,
        LifecycleRejected
    }

    public static class ModuleZSaveManager
    {
        private const string SaveFileName = "modulez_save.json";
        private const string PendingSuffix = ".pending";
        private const string BackupSuffix = ".bak";
        // A save gate, not another owner of game/session state. The bridge
        // alone owns the token and must release exactly its own publication.
        private static object provisionalTransitionOwner;
        private static bool loading;
        private static bool uncertainTransitionState;
        private static bool uncertainLoadState;
        private static ulong appliedResultRevision;
        private static bool hasAppliedResultRevision;
        private static bool resultApplicationPending;
        private static int appliedResultRival;
        private static bool appliedResultRematch;
        private static Vector3 appliedResultReturnPosition;

        internal static bool BeginDuelResultApplication(
            int rival, bool rematch, Vector3 returnPosition,
            ulong sessionRevision)
        {
            if (resultApplicationPending || provisionalTransitionOwner != null ||
                uncertainTransitionState || uncertainLoadState ||
                !SessionMatches(rival, rematch, returnPosition, sessionRevision))
                return false;
            resultApplicationPending = true;
            hasAppliedResultRevision = false;
            appliedResultRival = rival;
            appliedResultRematch = rematch;
            appliedResultReturnPosition = returnPosition;
            appliedResultRevision = sessionRevision;
            return true;
        }

        internal static bool ConfirmAppliedDuelResult(
            int rival, bool rematch, Vector3 returnPosition,
            int resultRival, ulong sessionRevision)
        {
            if (!resultApplicationPending ||
                appliedResultRevision != sessionRevision ||
                appliedResultRival != rival ||
                appliedResultRematch != rematch ||
                appliedResultReturnPosition != returnPosition ||
                resultRival != rival ||
                !SessionMatches(rival, rematch, returnPosition, sessionRevision))
                return false;
            hasAppliedResultRevision = true;
            return true;
        }

        // A direct-scene trial has no session authority. Keep its in-memory
        // Legacy projection from being published after leaving the Duel scene.
        internal static void MarkDirectDuelAsNonPersistable()
        {
            uncertainTransitionState = true;
            hasAppliedResultRevision = false;
        }

        private static bool SessionMatches(int rival, bool rematch,
            Vector3 returnPosition, ulong revision)
        {
            return ModuleZDuelSessionState.HasActiveDuel &&
                ModuleZDuelSessionState.Revision == revision &&
                (int)ModuleZDuelSessionState.RivalId == rival &&
                ModuleZDuelSessionState.IsRematch == rematch &&
                ModuleZDuelSessionState.ReturnPosition == returnPosition;
        }

        internal static bool BeginProvisionalTransition(object owner)
        {
            if (owner == null || provisionalTransitionOwner != null ||
                uncertainTransitionState)
                return false;
            provisionalTransitionOwner = owner;
            return true;
        }

        internal static void EndProvisionalTransition(object owner, bool stable)
        {
            if (!ReferenceEquals(provisionalTransitionOwner, owner))
                return;
            provisionalTransitionOwner = null;
            if (!stable)
                uncertainTransitionState = true;
        }

        // Called only after the menu has deleted the save, reset GameState and
        // cleared the transient session. Deletion alone is not a state reset.
        internal static bool ConfirmFreshStart()
        {
            if (loading || provisionalTransitionOwner != null ||
                ModuleZDuelSessionState.HasActiveDuel)
                return false;
            uncertainTransitionState = false;
            uncertainLoadState = false;
            hasAppliedResultRevision = false;
            resultApplicationPending = false;
            return true;
        }

        private static string SavePath
        {
            get
            {
                return Path.Combine(
                    ModuleZPersistencePaths.RootPath,
                    SaveFileName
                );
            }
        }

        private static string PendingPath => SavePath + PendingSuffix;
        private static string BackupPath => SavePath + BackupSuffix;

        public static void SaveGame()
        {
            ModuleZSaveGameStatus status = TrySaveGame();
            if (status != ModuleZSaveGameStatus.Success)
                Debug.LogWarning("[ModuleZ] No se pudo guardar la partida: " + status);
        }

        public static ModuleZSaveGameStatus TrySaveGame()
        {
            if (loading || provisionalTransitionOwner != null ||
                uncertainTransitionState || uncertainLoadState ||
                ((resultApplicationPending ||
                  ModuleZDuelSessionState.HasActiveDuel ||
                  (Application.isPlaying && SceneManager.GetActiveScene().name ==
                   ModuleZSceneController.DuelScene)) &&
                 (!hasAppliedResultRevision ||
                  !SessionMatches(appliedResultRival, appliedResultRematch,
                      appliedResultReturnPosition, appliedResultRevision))))
                return ModuleZSaveGameStatus.LifecycleRejected;

            ModuleZSaveData data;
            try
            {
                data = CaptureSaveData();
            }
            catch (Exception)
            {
                return ModuleZSaveGameStatus.CaptureError;
            }

            string json;
            try
            {
                json = JsonUtility.ToJson(data, true);
                if (!SaveJsonShape.IsValid(json) || !IsValid(data))
                    return ModuleZSaveGameStatus.SerializationError;
            }
            catch (Exception)
            {
                return ModuleZSaveGameStatus.SerializationError;
            }

            string pending = PendingPath;
            bool ownsPending = false;
            try
            {
                Directory.CreateDirectory(ModuleZPersistencePaths.RootPath);
                // CreateNew never truncates a pending file left by another
                // attempt or process. An unconfirmed pending file is not a save.
                using (FileStream stream = new FileStream(
                    pending, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    ownsPending = true;
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(json);
                        writer.Flush();
                        stream.Flush(true);
                    }
                }

                if (TryReadSaveDataAt(pending, out ModuleZSaveData verified) !=
                        ModuleZLoadGameStatus.Success || verified == null)
                {
                    DeleteOwnedPending(pending);
                    return ModuleZSaveGameStatus.TemporaryWriteError;
                }
            }
            catch (Exception)
            {
                if (ownsPending)
                    DeleteOwnedPending(pending);
                return ModuleZSaveGameStatus.TemporaryWriteError;
            }

            try
            {
                ModuleZLoadGameStatus existing =
                    TryReadSaveDataAt(SavePath, out _);
                if (existing == ModuleZLoadGameStatus.NotFound)
                {
                    // An orphaned backup may be the only recoverable copy.
                    // Never publish over that ambiguous state automatically.
                    if (File.Exists(BackupPath) || Directory.Exists(BackupPath))
                        return ModuleZSaveGameStatus.ExistingSaveInvalid;
                    // First creation has no previous save to replace. Move
                    // fails if another writer created the destination.
                    File.Move(pending, SavePath);
                }
                else if (existing == ModuleZLoadGameStatus.Success)
                {
                    // Both files share a directory/volume. Do not substitute
                    // copy/delete or overwrite-move when Replace is unsupported.
                    File.Replace(pending, SavePath, BackupPath);
                }
                else
                {
                    // A corrupt or unreadable main must not overwrite a
                    // potentially sole valid recovery copy.
                    return ModuleZSaveGameStatus.ExistingSaveInvalid;
                }
            }
            catch (PlatformNotSupportedException)
            {
                return ModuleZSaveGameStatus.UnsupportedPlatform;
            }
            catch (Exception)
            {
                // A verified pending file is retained for diagnosis. It is
                // never automatically promoted on a later load.
                return ModuleZSaveGameStatus.PublicationError;
            }

            Debug.Log("[ModuleZ] Partida guardada: " + SavePath);
            if (resultApplicationPending)
            {
                resultApplicationPending = false;
                hasAppliedResultRevision = false;
            }
            return ModuleZSaveGameStatus.Success;
        }

        private static void DeleteOwnedPending(string path)
        {
            try { File.Delete(path); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            catch (System.Security.SecurityException) { }
        }

        private static ModuleZSaveData CaptureSaveData()
        {
            ModuleZSaveData data = new ModuleZSaveData();

            data.returnX = ModuleZGameState.OpenWorldReturnPosition.x;
            data.returnY = ModuleZGameState.OpenWorldReturnPosition.y;
            data.returnZ = ModuleZGameState.OpenWorldReturnPosition.z;

            data.rivalMadridDefeated = ModuleZGameState.RivalMadridDefeated;
            data.rivalBarcelonaDefeated = ModuleZGameState.RivalBarcelonaDefeated;
            data.rivalValenciaDefeated = ModuleZGameState.RivalValenciaDefeated;
            data.rivalAndaluciaDefeated = ModuleZGameState.RivalAndaluciaDefeated;

            data.andaluciaUnlocked = ModuleZGameState.AndaluciaUnlocked;

            data.duelsWon = ModuleZGameState.DuelsWon;
            data.duelsLost = ModuleZGameState.DuelsLost;
            data.duelsAbandoned = ModuleZGameState.DuelsAbandoned;

            data.currentOpenWorldTheme = ModuleZGameState.CurrentOpenWorldTheme;

            data.mainProgressionCompleted = ModuleZGameState.MainProgressionCompleted;

            data.rematchesWon = ModuleZGameState.RematchesWon;
            data.rematchesLost = ModuleZGameState.RematchesLost;
            data.rematchesAbandoned = ModuleZGameState.RematchesAbandoned;

            data.rivalMadridPersonalityCompleted = ModuleZGameState.RivalMadridPersonalityCompleted;

            data.rivalBarcelonaPersonalityCompleted = ModuleZGameState.RivalBarcelonaPersonalityCompleted;

            data.rivalValenciaPersonalityCompleted = ModuleZGameState.RivalValenciaPersonalityCompleted;

            data.rivalAndaluciaPersonalityCompleted = ModuleZGameState.RivalAndaluciaPersonalityCompleted;

            ModuleZAchievementManager.EvaluateAll();

            data.achievements = ModuleZAchievementManager.GetAll();
            return data;
        }

        public static void LoadGame()
        {
            TryLoadGame();
        }

        public static ModuleZLoadGameStatus TryLoadGame()
        {
            if (loading || provisionalTransitionOwner != null)
                return ModuleZLoadGameStatus.ApplyError;
            loading = true;
            try
            {
                ModuleZLoadGameStatus status = TryLoadGameCore();
                if (status == ModuleZLoadGameStatus.Success)
                    uncertainLoadState = false;
                else if (status != ModuleZLoadGameStatus.NotFound)
                    uncertainLoadState = true;
                return status;
            }
            catch (Exception exception)
            {
                uncertainLoadState = true;
                Debug.LogWarning("[ModuleZ] Carga interrumpida; guardado bloqueado: " +
                    exception.Message);
                return ModuleZLoadGameStatus.ApplyError;
            }
            finally
            {
                loading = false;
            }
        }

        private static ModuleZLoadGameStatus TryLoadGameCore()
        {
            ModuleZLoadGameStatus status = TryReadSaveData(out ModuleZSaveData data);
            if (status != ModuleZLoadGameStatus.Success)
                return status;

            LoadSnapshot before;
            try
            {
                before = new LoadSnapshot();
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[ModuleZ] No se pudo preparar la carga: " + exception.Message);
                return ModuleZLoadGameStatus.ApplyError;
            }

            try
            {
                ApplyLoadedData(data);
                uncertainTransitionState = false;
                hasAppliedResultRevision = false;
                resultApplicationPending = false;
                Debug.Log("[ModuleZ] Partida cargada.");
                return ModuleZLoadGameStatus.Success;
            }
            catch (Exception exception)
            {
                // In particular, an achievement notification may throw after
                // GameState and some achievements have already changed.
                before.Restore();
                Debug.LogWarning("[ModuleZ] No se pudo aplicar la partida: " + exception.Message);
                return ModuleZLoadGameStatus.ApplyError;
            }
        }

        private static void ApplyLoadedData(ModuleZSaveData data)
        {
            ModuleZGameState.OpenWorldReturnPosition =
                new Vector3(data.returnX, data.returnY, data.returnZ);

            ModuleZGameState.RivalMadridDefeated = data.rivalMadridDefeated;
            ModuleZGameState.RivalBarcelonaDefeated = data.rivalBarcelonaDefeated;
            ModuleZGameState.RivalValenciaDefeated = data.rivalValenciaDefeated;
            ModuleZGameState.RivalAndaluciaDefeated = data.rivalAndaluciaDefeated;

            ModuleZGameState.AndaluciaUnlocked = data.andaluciaUnlocked;

            ModuleZGameState.DuelsWon = data.duelsWon;
            ModuleZGameState.DuelsLost = data.duelsLost;
            ModuleZGameState.DuelsAbandoned = data.duelsAbandoned;

            ModuleZGameState.CurrentOpenWorldTheme = data.currentOpenWorldTheme;

            ModuleZGameState.MainProgressionCompleted = data.mainProgressionCompleted;

            ModuleZGameState.RematchesWon = data.rematchesWon;
            ModuleZGameState.RematchesLost = data.rematchesLost;
            ModuleZGameState.RematchesAbandoned = data.rematchesAbandoned;

            ModuleZGameState.RivalMadridPersonalityCompleted = data.rivalMadridPersonalityCompleted;

            ModuleZGameState.RivalBarcelonaPersonalityCompleted = data.rivalBarcelonaPersonalityCompleted;

            ModuleZGameState.RivalValenciaPersonalityCompleted = data.rivalValenciaPersonalityCompleted;

            ModuleZGameState.RivalAndaluciaPersonalityCompleted = data.rivalAndaluciaPersonalityCompleted;

            ModuleZ.OpenWorld.Encounters.ModuleZRivalProgression.UpdateUnlocks();

            // Opcional
            ModuleZGameState.CurrentDuelIsRematch = false;
            ModuleZGameState.DuelCompleted = false;
            ModuleZGameState.DuelWasCancelled = false;
            ModuleZGameState.DuelWasLost = false;
            ModuleZGameState.DuelWasAbandoned = false;
            ModuleZGameState.IsPaused = false;

            ModuleZAchievementManager.LoadStates(data.achievements);
            ModuleZAchievementManager.EvaluateAll();
        }

        private sealed class LoadSnapshot
        {
            private readonly ModuleZSaveData state;
            private readonly bool rematch;
            private readonly bool completed;
            private readonly bool cancelled;
            private readonly bool lost;
            private readonly bool abandoned;
            private readonly bool paused;
            private readonly List<ModuleZAchievementState> achievements;
            private readonly List<bool> unlocked = new List<bool>();
            private readonly List<string> dates = new List<string>();

            internal LoadSnapshot()
            {
                state = new ModuleZSaveData {
                    returnX = ModuleZGameState.OpenWorldReturnPosition.x,
                    returnY = ModuleZGameState.OpenWorldReturnPosition.y,
                    returnZ = ModuleZGameState.OpenWorldReturnPosition.z,
                    rivalMadridDefeated = ModuleZGameState.RivalMadridDefeated,
                    rivalBarcelonaDefeated = ModuleZGameState.RivalBarcelonaDefeated,
                    rivalValenciaDefeated = ModuleZGameState.RivalValenciaDefeated,
                    rivalAndaluciaDefeated = ModuleZGameState.RivalAndaluciaDefeated,
                    andaluciaUnlocked = ModuleZGameState.AndaluciaUnlocked,
                    duelsWon = ModuleZGameState.DuelsWon,
                    duelsLost = ModuleZGameState.DuelsLost,
                    duelsAbandoned = ModuleZGameState.DuelsAbandoned,
                    currentOpenWorldTheme = ModuleZGameState.CurrentOpenWorldTheme,
                    mainProgressionCompleted = ModuleZGameState.MainProgressionCompleted,
                    rematchesWon = ModuleZGameState.RematchesWon,
                    rematchesLost = ModuleZGameState.RematchesLost,
                    rematchesAbandoned = ModuleZGameState.RematchesAbandoned,
                    rivalMadridPersonalityCompleted = ModuleZGameState.RivalMadridPersonalityCompleted,
                    rivalBarcelonaPersonalityCompleted = ModuleZGameState.RivalBarcelonaPersonalityCompleted,
                    rivalValenciaPersonalityCompleted = ModuleZGameState.RivalValenciaPersonalityCompleted,
                    rivalAndaluciaPersonalityCompleted = ModuleZGameState.RivalAndaluciaPersonalityCompleted
                };
                rematch = ModuleZGameState.CurrentDuelIsRematch;
                completed = ModuleZGameState.DuelCompleted;
                cancelled = ModuleZGameState.DuelWasCancelled;
                lost = ModuleZGameState.DuelWasLost;
                abandoned = ModuleZGameState.DuelWasAbandoned;
                paused = ModuleZGameState.IsPaused;
                achievements = ModuleZAchievementManager.GetAll();
                foreach (ModuleZAchievementState achievement in achievements)
                {
                    unlocked.Add(achievement.unlocked);
                    dates.Add(achievement.unlockedDate);
                }
            }

            internal void Restore()
            {
                // Do not call progression or EvaluateAll during rollback.
                // Those are the application operations that may have failed.
                ModuleZGameState.OpenWorldReturnPosition = new Vector3(
                    state.returnX, state.returnY, state.returnZ);
                ModuleZGameState.RivalMadridDefeated = state.rivalMadridDefeated;
                ModuleZGameState.RivalBarcelonaDefeated = state.rivalBarcelonaDefeated;
                ModuleZGameState.RivalValenciaDefeated = state.rivalValenciaDefeated;
                ModuleZGameState.RivalAndaluciaDefeated = state.rivalAndaluciaDefeated;
                ModuleZGameState.AndaluciaUnlocked = state.andaluciaUnlocked;
                ModuleZGameState.DuelsWon = state.duelsWon;
                ModuleZGameState.DuelsLost = state.duelsLost;
                ModuleZGameState.DuelsAbandoned = state.duelsAbandoned;
                ModuleZGameState.CurrentOpenWorldTheme = state.currentOpenWorldTheme;
                ModuleZGameState.MainProgressionCompleted = state.mainProgressionCompleted;
                ModuleZGameState.RematchesWon = state.rematchesWon;
                ModuleZGameState.RematchesLost = state.rematchesLost;
                ModuleZGameState.RematchesAbandoned = state.rematchesAbandoned;
                ModuleZGameState.RivalMadridPersonalityCompleted = state.rivalMadridPersonalityCompleted;
                ModuleZGameState.RivalBarcelonaPersonalityCompleted = state.rivalBarcelonaPersonalityCompleted;
                ModuleZGameState.RivalValenciaPersonalityCompleted = state.rivalValenciaPersonalityCompleted;
                ModuleZGameState.RivalAndaluciaPersonalityCompleted = state.rivalAndaluciaPersonalityCompleted;
                ModuleZGameState.CurrentDuelIsRematch = rematch;
                ModuleZGameState.DuelCompleted = completed;
                ModuleZGameState.DuelWasCancelled = cancelled;
                ModuleZGameState.DuelWasLost = lost;
                ModuleZGameState.DuelWasAbandoned = abandoned;
                ModuleZGameState.IsPaused = paused;
                ModuleZAchievementManager.LoadStates(achievements);
                for (int i = 0; i < achievements.Count; i++)
                {
                    achievements[i].unlocked = unlocked[i];
                    achievements[i].unlockedDate = dates[i];
                }
            }
        }

        private static ModuleZLoadGameStatus TryReadSaveData(out ModuleZSaveData data)
        {
            ModuleZLoadGameStatus main = TryReadSaveDataAt(SavePath, out data);
            if (main != ModuleZLoadGameStatus.InvalidData)
            {
                ReportReadFailure(main);
                return main;
            }

            // A corrupt main may have a valid, older copy from the previous
            // successful replacement. Read it without rewriting either file.
            ModuleZLoadGameStatus backup = TryReadSaveDataAt(BackupPath, out ModuleZSaveData recovered);
            if (backup == ModuleZLoadGameStatus.Success)
            {
                data = recovered;
                Debug.LogWarning("[ModuleZ] Guardado principal inválido; se leyó la copia de recuperación.");
                return ModuleZLoadGameStatus.Success;
            }

            data = null;
            ReportReadFailure(main);
            return main;
        }

        private static void ReportReadFailure(ModuleZLoadGameStatus status)
        {
            if (status == ModuleZLoadGameStatus.NotFound)
                Debug.Log("[ModuleZ] No existe partida guardada.");
            else if (status == ModuleZLoadGameStatus.ReadError)
                Debug.LogWarning("[ModuleZ] No se pudo leer la partida.");
            else if (status == ModuleZLoadGameStatus.InvalidData)
                Debug.LogWarning("[ModuleZ] SaveData inválido.");
        }

        private static ModuleZLoadGameStatus TryReadSaveDataAt(string path, out ModuleZSaveData data)
        {
            data = null;
            string json;
            try
            {
                json = File.ReadAllText(path);
            }
            catch (FileNotFoundException)
            {
                return ModuleZLoadGameStatus.NotFound;
            }
            catch (DirectoryNotFoundException)
            {
                return ModuleZLoadGameStatus.NotFound;
            }
            catch (Exception exception) when (exception is IOException ||
                exception is UnauthorizedAccessException ||
                exception is ArgumentException ||
                exception is System.Security.SecurityException ||
                exception is NotSupportedException)
            {
                return ModuleZLoadGameStatus.ReadError;
            }

            try
            {
                // JsonUtility tolerates missing and mistyped members. Validate
                // the complete JSON grammar and root field types first, while
                // retaining the optional fields of older current-name saves.
                if (!SaveJsonShape.IsValid(json))
                    return ModuleZLoadGameStatus.InvalidData;

                data = JsonUtility.FromJson<ModuleZSaveData>(json);
            }
            catch (Exception exception) when (exception is ArgumentException ||
                exception is FormatException)
            {
                return ModuleZLoadGameStatus.InvalidData;
            }

            if (!IsValid(data))
            {
                return ModuleZLoadGameStatus.InvalidData;
            }

            return ModuleZLoadGameStatus.Success;
        }

        private static bool IsValid(ModuleZSaveData data)
        {
            return data != null &&
                !float.IsNaN(data.returnX) && !float.IsInfinity(data.returnX) &&
                !float.IsNaN(data.returnY) && !float.IsInfinity(data.returnY) &&
                !float.IsNaN(data.returnZ) && !float.IsInfinity(data.returnZ) &&
                data.duelsWon >= 0 && data.duelsLost >= 0 &&
                data.duelsAbandoned >= 0 && data.rematchesWon >= 0 &&
                data.rematchesLost >= 0 && data.rematchesAbandoned >= 0 &&
                Enum.IsDefined(data.currentOpenWorldTheme.GetType(),
                    data.currentOpenWorldTheme) &&
                ValidAchievements(data.achievements);
        }

        private static bool ValidAchievements(List<ModuleZAchievementState> states)
        {
            if (states == null)
                return true;

            var ids = new HashSet<ModuleZAchievementId>();
            foreach (ModuleZAchievementState state in states)
            {
                if (state == null ||
                    !Enum.IsDefined(typeof(ModuleZAchievementId), state.achievementId) ||
                    !ids.Add(state.achievementId))
                    return false;
            }
            return true;
        }

        // Root-aware JSON grammar check, not a second deserializer. Unknown
        // fields remain compatible; known fields must have their saved type.
        private sealed class SaveJsonShape
        {
            private enum Kind { Object, Array, String, Integer, Number, Boolean, Null }
            private readonly string source;
            private readonly HashSet<string> rootNames = new HashSet<string>();
            private int position;

            private SaveJsonShape(string source) { this.source = source; }

            internal static bool IsValid(string source)
            {
                if (string.IsNullOrWhiteSpace(source))
                    return false;
                var reader = new SaveJsonShape(source);
                reader.WhiteSpace();
                if (!reader.Object(true, 0))
                    return false;
                reader.WhiteSpace();
                return reader.position == source.Length &&
                    reader.rootNames.Contains("duelsWon") &&
                    reader.rootNames.Contains("duelsLost") &&
                    reader.rootNames.Contains("duelsAbandoned");
            }

            private bool Object(bool root, int depth, bool achievement = false)
            {
                if (depth > 32 || !Take('{')) return false;
                var memberNames = achievement ? new HashSet<string>() : null;
                WhiteSpace();
                if (Take('}')) return !achievement;
                do
                {
                    WhiteSpace();
                    if (!String(out string name)) return false;
                    WhiteSpace();
                    if (!Take(':')) return false;
                    WhiteSpace();
                    int valueStart = position;
                    Kind kind;
                    if (root && name == "achievements")
                    {
                        kind = Kind.Array;
                        if (!AchievementArray(depth + 1)) return false;
                    }
                    else if (!Value(depth + 1, out kind)) return false;
                    if (root && (!rootNames.Add(name) || !ValidRootType(name, kind) ||
                        (IsIntegerField(name) && !int.TryParse(
                            source.Substring(valueStart, position - valueStart),
                            System.Globalization.NumberStyles.AllowLeadingSign,
                            System.Globalization.CultureInfo.InvariantCulture, out _))))
                        return false;
                    if (achievement && (!memberNames.Add(name) ||
                        !ValidAchievementType(name, kind) ||
                        (name == "achievementId" && !int.TryParse(
                            source.Substring(valueStart, position - valueStart),
                            System.Globalization.NumberStyles.AllowLeadingSign,
                            System.Globalization.CultureInfo.InvariantCulture, out _))))
                        return false;
                    WhiteSpace();
                    if (Take('}')) return !achievement ||
                        memberNames.Contains("achievementId");
                } while (Take(','));
                return false;
            }

            private bool AchievementArray(int depth)
            {
                if (depth > 32 || !Take('[')) return false;
                WhiteSpace();
                if (Take(']')) return true;
                do
                {
                    WhiteSpace();
                    if (!Object(false, depth + 1, true)) return false;
                    WhiteSpace();
                    if (Take(']')) return true;
                } while (Take(','));
                return false;
            }

            private bool Array(int depth)
            {
                if (depth > 32 || !Take('[')) return false;
                WhiteSpace();
                if (Take(']')) return true;
                do
                {
                    WhiteSpace();
                    if (!Value(depth + 1, out _)) return false;
                    WhiteSpace();
                    if (Take(']')) return true;
                } while (Take(','));
                return false;
            }

            private bool Value(int depth, out Kind kind)
            {
                kind = Kind.Null;
                if (position >= source.Length || depth > 32) return false;
                char c = source[position];
                if (c == '{') { kind = Kind.Object; return Object(false, depth); }
                if (c == '[') { kind = Kind.Array; return Array(depth); }
                if (c == '"') { kind = Kind.String; return String(out _); }
                if (c == 't' || c == 'f')
                {
                    kind = Kind.Boolean;
                    return Literal(c == 't' ? "true" : "false");
                }
                if (c == 'n') return Literal("null");
                return Number(out kind);
            }

            private bool Number(out Kind kind)
            {
                kind = Kind.Integer;
                Take('-');
                if (Take('0'))
                {
                    if (Digit()) return false;
                }
                else
                {
                    if (position >= source.Length || source[position] < '1' ||
                        source[position] > '9') return false;
                    do { position++; } while (Digit());
                }
                if (Take('.'))
                {
                    kind = Kind.Number;
                    if (!Digit()) return false;
                    do { position++; } while (Digit());
                }
                if (Take('e') || Take('E'))
                {
                    kind = Kind.Number;
                    if (!Take('+')) Take('-');
                    if (!Digit()) return false;
                    do { position++; } while (Digit());
                }
                return true;
            }

            private bool String(out string value)
            {
                value = null;
                if (!Take('"')) return false;
                var decoded = new System.Text.StringBuilder();
                while (position < source.Length)
                {
                    char c = source[position++];
                    if (c == '"') { value = decoded.ToString(); return true; }
                    if (c < 0x20) return false;
                    if (c != '\\') { decoded.Append(c); continue; }
                    if (position >= source.Length) return false;
                    c = source[position++];
                    if (c == 'u')
                    {
                        if (position + 4 > source.Length) return false;
                        string hex = source.Substring(position, 4);
                        if (!ushort.TryParse(hex, System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture, out ushort code))
                            return false;
                        decoded.Append((char)code);
                        position += 4;
                    }
                    else if ("\"\\/".IndexOf(c) >= 0) decoded.Append(c);
                    else if (c == 'b') decoded.Append('\b');
                    else if (c == 'f') decoded.Append('\f');
                    else if (c == 'n') decoded.Append('\n');
                    else if (c == 'r') decoded.Append('\r');
                    else if (c == 't') decoded.Append('\t');
                    else return false;
                }
                return false;
            }

            private bool Literal(string text)
            {
                if (position + text.Length > source.Length ||
                    string.CompareOrdinal(source, position, text, 0, text.Length) != 0)
                    return false;
                position += text.Length;
                return true;
            }

            private bool Take(char c)
            {
                if (position >= source.Length || source[position] != c) return false;
                position++;
                return true;
            }

            private bool Digit() => position < source.Length &&
                source[position] >= '0' && source[position] <= '9';

            private void WhiteSpace()
            {
                while (position < source.Length &&
                    (source[position] == ' ' || source[position] == '\t' ||
                     source[position] == '\r' || source[position] == '\n'))
                    position++;
            }

            private static bool ValidRootType(string name, Kind kind)
            {
                switch (name)
                {
                    case "duelsWon": case "duelsLost": case "duelsAbandoned":
                    case "rematchesWon": case "rematchesLost":
                    case "rematchesAbandoned": case "currentOpenWorldTheme":
                        return kind == Kind.Integer;
                    case "returnX": case "returnY": case "returnZ":
                        return kind == Kind.Integer || kind == Kind.Number;
                    case "achievements": return kind == Kind.Array;
                    case "rivalMadridDefeated": case "rivalBarcelonaDefeated":
                    case "rivalValenciaDefeated": case "rivalAndaluciaDefeated":
                    case "andaluciaUnlocked": case "mainProgressionCompleted":
                    case "rivalMadridPersonalityCompleted":
                    case "rivalBarcelonaPersonalityCompleted":
                    case "rivalValenciaPersonalityCompleted":
                    case "rivalAndaluciaPersonalityCompleted":
                        return kind == Kind.Boolean;
                    default: return true;
                }
            }

            private static bool IsIntegerField(string name)
            {
                switch (name)
                {
                    case "duelsWon": case "duelsLost": case "duelsAbandoned":
                    case "rematchesWon": case "rematchesLost":
                    case "rematchesAbandoned": case "currentOpenWorldTheme":
                        return true;
                    default: return false;
                }
            }

            private static bool ValidAchievementType(string name, Kind kind)
            {
                switch (name)
                {
                    case "achievementId": return kind == Kind.Integer;
                    case "unlocked": return kind == Kind.Boolean;
                    case "unlockedDate": return kind == Kind.String || kind == Kind.Null;
                    default: return true;
                }
            }
        }

        public static void DeleteSave()
        {
            TryDeleteSave();
        }

        public static bool TryDeleteSave()
        {
            if (loading || provisionalTransitionOwner != null)
                return false;
            try
            {
                // File.Delete is a no-op for a missing file, but unlike a
                // File.Exists guard it reports access and I/O failures. If
                // deletion is interrupted, leave the backup until last so a
                // recovery copy is not discarded before the main is removed.
                File.Delete(SavePath);
                File.Delete(PendingPath);
                File.Delete(BackupPath);
                ModuleZAchievementManager.ResetAll();
                Debug.Log("[ModuleZ] Partida eliminada.");
                return true;
            }
            catch (Exception exception) when (exception is IOException ||
                exception is UnauthorizedAccessException ||
                exception is ArgumentException ||
                exception is System.Security.SecurityException ||
                exception is NotSupportedException)
            {
                Debug.LogWarning("[ModuleZ] No se pudo borrar la partida: " + exception.Message);
                return false;
            }
        }

        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public static ModuleZSaveData GetSaveData()
        {
            return TryReadSaveData(out ModuleZSaveData data) ==
                ModuleZLoadGameStatus.Success ? data : null;
        }
    }
}
