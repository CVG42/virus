using System;
using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class AchievementController : MonoBehaviour
    {
        [SerializeField] AchievementPopupUI _popupUI;
        private readonly List<Action> _evaluators = new();

        private void Start()
        {
            CollectablesManager.Source.OnCookieCollected += () => { EvaluateCookieAchievements(); };

            _evaluators.Add(EvaluateCookieAchievements);
        }

        private void OnDestroy()
        {
            CollectablesManager.Source.OnCookieCollected -= EvaluateCookieAchievements;
        }

        private void EvaluateCookieAchievements()
        {
            TryUnlock("collect_1_cookie", CollectablesManager.Source.TotalCookies == 1);
            TryUnlock("collect_5_cookie", CollectablesManager.Source.TotalCookies == 5);
            TryUnlock("collect_10_cookie", CollectablesManager.Source.TotalCookies == 10);
            TryUnlock("collect_70_cookie", CollectablesManager.Source.TotalCookies == 70);
        }

        private void TryUnlock(string id, bool condition)
        {
            if (!condition) return;

            bool unlocked = AchievementManager.Source.Unlock(id);
            
            if (!unlocked) return; 

            var achievement = AchievementManager.Source.GetDefinition(id);
            _popupUI.Show(achievement);
        }
    }
}
