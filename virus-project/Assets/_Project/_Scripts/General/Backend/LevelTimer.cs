using UnityEngine;
using Utilities;

namespace Virus
{
    public class LevelTimer : MonoBehaviour
    {
        private StopwatchTimer _timer;

        public float ElapsedTime => _timer.GetTime();

        private void Awake()
        {
            _timer = new StopwatchTimer();
        }

        private void Start()
        {
            _timer.Start();
        }

        private void Update()
        {
            _timer.Tick(Time.deltaTime);
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        public void ResetTimer()
        {
            _timer.Reset();
        }
    }
}
