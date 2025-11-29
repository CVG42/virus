using System.Collections.Generic;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Virus
{
    public class AchievementManager : Singleton<IAchievementSouce>, IAchievementSouce
    {
        [SerializeField] private AchievementDatabase _achievementDatabase;

        public event Action<AchievementAttributes> OnAchievementUnlocked;

        private Dictionary<string, bool> _achievementStates = new();
        private Dictionary<string, UserAPI.AchievementData> _serverAchievementData = new();
        private bool _useOnlineMode = true;
        private CancellationTokenSource _cancellationTokenSource;

        private int UserId => PlayerPrefs.GetInt("UserID", 0);

        protected override void Awake()
        {
            base.Awake();
            _cancellationTokenSource = new CancellationTokenSource();
            _achievementDatabase.Initialize();

            LoadAchievementsAsync().Forget();
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private async UniTaskVoid LoadAchievementsAsync()
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested)
                return;

            try
            {
                if (_useOnlineMode && UserId > 0)
                {
                    await LoadAchievementsFromServer();
                }
                else
                {
                    LoadAchievementsLocal();
                }
            }
            catch (Exception e)
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Debug.LogError($"Error cargando logros: {e.Message}");
                }
            }
        }

        private async UniTask LoadAchievementsFromServer()
        {
            try
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;

                var response = await UserAPI.GetAchievements(UserId);

                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;

                if (response.success && response.achievements != null)
                {
                    ProcessServerResponse(response);
                    Debug.Log($"Logros cargados del servidor: {response.achievements.Length}");
                }
                else
                {
                    Debug.LogWarning("Error cargando logros del servidor, usando datos locales");
                    LoadAchievementsLocal();
                }
            }
            catch (Exception e)
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Debug.LogError($"Error cargando logros: {e.Message}");
                    LoadAchievementsLocal();
                }
            }
        }

        private void ProcessServerResponse(UserAPI.GetAchievementsResponse response)
        {
            _achievementStates.Clear();
            _serverAchievementData.Clear();

            foreach (var serverAchievement in response.achievements)
            {
                string achievementId = serverAchievement.achievement_id;
                bool unlocked = serverAchievement.unlocked == 1;

                _achievementStates[achievementId] = unlocked;
                _serverAchievementData[achievementId] = serverAchievement;

                PlayerPrefs.SetInt("ACH_" + achievementId, unlocked ? 1 : 0);
            }

            PlayerPrefs.Save();
        }

        private void LoadAchievementsLocal()
        {
            _achievementStates.Clear();

            foreach (var a in _achievementDatabase.achievements)
            {
                bool unlocked = PlayerPrefs.GetInt("ACH_" + a.id, a.unlockByDefault ? 1 : 0) == 1;
                _achievementStates[a.id] = unlocked;
            }

            Debug.Log("Logros cargados localmente");
        }

        public bool IsUnlocked(string id)
        {
            return _achievementStates.ContainsKey(id) && _achievementStates[id];
        }

        public bool Unlock(string id)
        {
            if (!_achievementStates.ContainsKey(id))
            {
                Debug.LogWarning($"Logro no encontrado: {id}");
                return false;
            }

            if (_achievementStates[id])
            {
                Debug.Log($"Logro ya desbloqueado: {id}");
                return false;
            }

            AchievementAttributes achievement = _achievementDatabase.GetById(id);
            if (achievement == null)
            {
                Debug.LogError($"Definición de logro no encontrada: {id}");
                return false;
            }

            _achievementStates[id] = true;
            SaveAchievementLocal(achievement);
            TriggerAchievementUnlocked(achievement);

            if (_useOnlineMode && UserId > 0)
            {
                UnlockOnServerAsync(id).Forget();
            }

            return true;
        }

        private async UniTaskVoid UnlockOnServerAsync(string achievementId)
        {
            try
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;

                var response = await UserAPI.UnlockAchievement(UserId, achievementId);

                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;

                if (response.success)
                {
                    Debug.Log($"Logro {achievementId} sincronizado con servidor");
                }
                else
                {
                    Debug.LogError($"Error sincronizando logro {achievementId}: {response.message}");
                }
            }
            catch (Exception e)
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Debug.LogError($"Excepción sincronizando logro {achievementId}: {e.Message}");
                }
            }
        }

        private void SaveAchievementLocal(AchievementAttributes achievement)
        {
            PlayerPrefs.SetInt("ACH_" + achievement.id, 1);
            PlayerPrefs.Save();
        }

        private void TriggerAchievementUnlocked(AchievementAttributes achievement)
        {
            Debug.Log($"Achievement unlocked: {achievement.title}");
            OnAchievementUnlocked?.Invoke(achievement);
        }

        public AchievementAttributes GetDefinition(string id)
        {
            return _achievementDatabase.GetById(id);
        }

        public async UniTask SyncWithServer()
        {
            try
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;

                if (UserId > 0 && _useOnlineMode)
                {
                    await LoadAchievementsFromServer();
                }
            }
            catch (Exception e)
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Debug.LogError($"Error sincronizando: {e.Message}");
                }
            }
        }

        public void SetOnlineMode(bool online)
        {
            _useOnlineMode = online;
            if (online && UserId > 0)
            {
                LoadAchievementsAsync().Forget();
            }
        }
    }
}
