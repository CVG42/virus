using UnityEngine;

namespace Virus
{
    public class UserProgress : Singleton<IProgressSource>, IProgressSource
    {
        public ProgressData CurrentProgress { get; private set; }

        private int _userId;
        private int _levelId = 1;

        private async void Start()
        {
            _userId = PlayerPrefs.GetInt("UserID", 0);

            var response = await UserAPI.GetProgress(_userId, _levelId);

            if (response.success && response.progress != null)
            {
                CurrentProgress = response.progress;
                Debug.Log("Progress loaded");
            }
            else
            {
                Debug.Log("No progress found, starting fresh.");
            }
        }

        public async void UpdateCookiesProgress(int currentCookies)
        {
            await UserAPI.UpdateCookies(_userId, _levelId, currentCookies);
        }

        public async void CompleteLevel(int finalCookies, int timeSeconds)
        {
            await UserAPI.CompleteLevel(_userId, _levelId, finalCookies, timeSeconds);
        }
    }
}
