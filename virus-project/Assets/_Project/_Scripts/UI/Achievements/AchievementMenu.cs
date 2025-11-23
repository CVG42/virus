using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class AchievementMenu : MonoBehaviour
    {
        [SerializeField] private AchievementDatabase _achievements;
        [SerializeField] private Transform _contentParent;
        [SerializeField] private AchievementItemUI _itemPrefab;

        private List<AchievementItemUI> _items = new();

        private void OnEnable()
        {
            Refresh();
            AchievementManager.Source.OnAchievementUnlocked += HandleAchievementUnlocked;
        }

        private void OnDisable()
        {
            AchievementManager.Source.OnAchievementUnlocked -= HandleAchievementUnlocked;
        }

        private void HandleAchievementUnlocked(AchievementAttributes achievement)
        {
            Refresh();
        }

        public void Refresh()
        {
            foreach (Transform t in _contentParent)
                Destroy(t.gameObject);

            _items.Clear();

            foreach (var achievement in _achievements.achievements)
            {
                AchievementItemUI item = Instantiate(_itemPrefab, _contentParent);
                bool unlocked = AchievementManager.Source.IsUnlocked(achievement.id);

                item.SetData(achievement, unlocked);
                _items.Add(item);
            }
        }
    }
}
