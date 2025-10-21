using System;

namespace Virus
{
    public interface IInjectionSource
    {
        event Action<int, float> OnProgressChanged;
        event Action OnInjectionFull;

        int CurrentProgress { get; }
        int MaxProgress { get; }
        float ProgressPercentage { get; }
        bool IsFull { get; }

        void AddProgress(int amount = 1);
        void ResetProgress();
        bool TryUseProgress();
    }
}
