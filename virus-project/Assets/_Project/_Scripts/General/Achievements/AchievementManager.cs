using System.Collections.Generic;
using System;
using UnityEngine;

namespace Virus
{
    public class AchievementManager : Singleton<IAchievementSouce>, IAchievementSouce
    {
        [SerializeField] private AchievementDatabase _achievementDatabase;

        public event Action<AchievementAttributes> OnAchievementUnlocked;
        
        private Dictionary<string, bool> _achievementStates = new();

        protected override void Awake()
        {
            base.Awake();

            _achievementDatabase.Initialize();
            LoadAchievements();
        }

        private void LoadAchievements()
        {
            _achievementStates.Clear();

            foreach (var a in _achievementDatabase.achievements)
            {
                bool unlocked = PlayerPrefs.GetInt("ACH_" + a.id, a.unlockByDefault ? 1 : 0) == 1;
                _achievementStates[a.id] = unlocked;
            }
        }

        private void SaveAchievement(AchievementAttributes achievement)
        {
            PlayerPrefs.SetInt("ACH_" + achievement.id, 1);
        }

        public bool IsUnlocked(string id)
        {
            return _achievementStates.ContainsKey(id) && _achievementStates[id];
        }

        public bool Unlock(string id)
        {
            if (!_achievementStates.ContainsKey(id))
                return false;

            if (_achievementStates[id])
                return false;

            AchievementAttributes achievement = _achievementDatabase.GetById(id);

            _achievementStates[id] = true;
            SaveAchievement(achievement);

            Debug.Log($"Achievement unlocked: {achievement.title}");

            OnAchievementUnlocked?.Invoke(achievement);

            return true;
        }

        public AchievementAttributes GetDefinition(string id)
        {
            return _achievementDatabase.GetById(id);
        }
    }
}
