using System;

namespace Virus
{
    public interface ITypingSource
    {
        event Action OnTypingCompleted;

        void StartTypingEvent(TextData textData);
        void ActivateUI();
        void DeactivateUI();
    }
}
