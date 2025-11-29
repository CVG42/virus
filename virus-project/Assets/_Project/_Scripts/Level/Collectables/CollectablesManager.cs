using System;
using UnityEngine;

namespace Virus
{
    public class CollectablesManager : Singleton<ICollectableSource>, ICollectableSource
    {
        public int TotalCookies { get; private set; }

        public event Action<int> OnCookiesChanged;
        public event Action OnCookieCollected;

        protected override void Awake()
        {
            base.Awake();

            ResetCookies();
        }

        public void AddCookie()
        {
            TotalCookies++;

            OnCookiesChanged.Invoke(TotalCookies);
            OnCookieCollected?.Invoke();

            UserProgress.Source.UpdateCookiesProgress(TotalCookies);
        }

        public void ResetCookies()
        {
            TotalCookies = 0;
            OnCookiesChanged?.Invoke(TotalCookies);
        }
    }
}
