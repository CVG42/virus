using UnityEngine;

namespace Virus
{
    public class EndLevelHandler : MonoBehaviour
    {
        [SerializeField] private LevelTimer _timerManager;

        public void OnLevelCompleted()
        {
            _timerManager.StopTimer();

            float completionSeconds = _timerManager.ElapsedTime;

            Debug.Log("Level completed in: " + completionSeconds + " seconds");

            UserProgress.Source.CompleteLevel(CollectablesManager.Source.TotalCookies, Mathf.FloorToInt(completionSeconds));
        }
    }
}
