using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    [CreateAssetMenu(fileName = "AchievementDatabase", menuName = "Achievements/Achievement Database")]
    public class AchievementDatabase : ScriptableObject
    {
        public List<AchievementAttributes> achievements = new();

        private Dictionary<string, AchievementAttributes> _achievements;

        public void Initialize()
        {
            _achievements = new Dictionary<string, AchievementAttributes>();

            foreach (var achievement in achievements)
            {
                if (achievement == null || string.IsNullOrEmpty(achievement.id))
                {
                    continue;
                }

                if (_achievements.ContainsKey(achievement.id))
                {
                    continue;
                }

                _achievements.Add(achievement.id, achievement);
            }
        }

        public AchievementAttributes GetById(string id)
        {
            if (_achievements == null) Initialize();

            _achievements.TryGetValue(id, out var achievement);
            return achievement;
        }
    }
}
