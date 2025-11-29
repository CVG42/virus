using System;
using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class AchievementController : MonoBehaviour
    {
        [SerializeField] AchievementPopupUI _popupUI;

        private void Start()
        {
            CollectablesManager.Source.OnCookieCollected += EvaluateCookieAchievements;
            AchievementManager.Source.OnAchievementUnlocked += ShowAchievementPopup;
        }

        private void OnDestroy()
        {
            CollectablesManager.Source.OnCookieCollected -= EvaluateCookieAchievements;

            if (AchievementManager.Source != null)
            {
                AchievementManager.Source.OnAchievementUnlocked -= ShowAchievementPopup;
            }
        }

        private void EvaluateCookieAchievements()
        {
            int totalCookies = CollectablesManager.Source.TotalCookies;

            TryUnlock("1", totalCookies >= 1);
            TryUnlock("2", totalCookies >= 5);
            TryUnlock("3", totalCookies >= 10);
            TryUnlock("4", totalCookies >= 70);
        }

        private void TryUnlock(string id, bool condition)
        {
            if (!condition) return;

            bool unlocked = AchievementManager.Source.Unlock(id);

            if (unlocked)
            {
                Debug.Log($"Condición cumplida para logro: {id}");
            }
        }

        private void ShowAchievementPopup(AchievementAttributes achievement)
        {
            if (_popupUI != null)
            {
                _popupUI.Show(achievement);
            }
        }
    }
}
