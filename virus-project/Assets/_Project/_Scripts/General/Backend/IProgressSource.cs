using UnityEngine;

namespace Virus
{
    public interface IProgressSource
    {
        void UpdateCookiesProgress(int currentCookies);
        void CompleteLevel(int finalCookies, int timeSeconds);
    }
}
