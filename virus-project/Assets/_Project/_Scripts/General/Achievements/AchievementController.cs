using System;
using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class AchievementController : MonoBehaviour
    {
        private readonly List<Action> _evaluators = new();

        private void Start()
        {
            CollectablesManager.Source.OnCookieCollected += EvaluateCookiesAchievements;
        }

        private void OnDestroy()
        {
            CollectablesManager.Source.OnCookieCollected -= EvaluateCookiesAchievements;
        }

        private void EvaluateCookiesAchievements()
        {
            if (CollectablesManager.Source.TotalCookies == 1)
            {
                AchievementManager.Source.Unlock("collect_1_cookie");
            }
        }

        private void TryUnlock(string id, bool condition)
        {
            if (!condition)
                return;

            bool unlocked = AchievementManager.Source.Unlock(id);
            if (!unlocked)
                return; 

            var achievement = AchievementManager.Source.GetDefinition(id);
            // popupUI.Show(achievement);
        }
    }
}
