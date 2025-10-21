using System;
using UnityEngine;

namespace Virus
{
    public interface ICollectableSource
    {
        event Action<int> OnCookiesChanged;

        int TotalCookies { get; }

        void AddCookie();
        void ResetCookies();
    }
}
