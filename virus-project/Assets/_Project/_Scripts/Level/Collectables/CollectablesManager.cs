using System;
using UnityEngine;

namespace Virus
{
    public class CollectablesManager : Singleton<ICollectableSource>, ICollectableSource
    {
        public int TotalCookies { get; private set; }

        public event Action<int> OnCookiesChanged;

        protected override void Awake()
        {
            base.Awake();

            ResetCookies();
        }

        public void AddCookie()
        {
            TotalCookies++;

            OnCookiesChanged.Invoke(TotalCookies);
        }

        public void ResetCookies()
        {
            TotalCookies = 0;
            OnCookiesChanged?.Invoke(TotalCookies);
        }
    }
}
