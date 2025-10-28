using System;
using UnityEngine;

namespace Virus
{
    public class InjectionManager : Singleton<IInjectionSource>, IInjectionSource
    {
        [SerializeField] private int _fullInjectionValue = 20;

        public event Action<int, float> OnProgressChanged;
        public event Action OnInjectionFull;

        public int CurrentProgress {  get; private set; }
        public int MaxProgress => _fullInjectionValue;
        public float ProgressPercentage => (float)CurrentProgress / _fullInjectionValue;
        public bool IsFull => CurrentProgress >= _fullInjectionValue;

        protected override void Awake()
        {
            base.Awake();

            ResetProgress();
        }

        public void AddProgress(int  amount = 1)
        {
            int previousProgress = CurrentProgress;
            CurrentProgress = Mathf.Min(CurrentProgress + amount, _fullInjectionValue);

            OnProgressChanged?.Invoke(CurrentProgress, ProgressPercentage);

            if (!IsFull && previousProgress < _fullInjectionValue && CurrentProgress >= _fullInjectionValue)
            {
                OnInjectionFull?.Invoke();
            }
        }

        public void ResetProgress()
        {
            CurrentProgress = 0;
            OnProgressChanged?.Invoke(CurrentProgress, ProgressPercentage);
        }

        public bool TryUseProgress()
        {
            if (!IsFull) return false;

            ResetProgress();

            return true;
        }
    }
}
