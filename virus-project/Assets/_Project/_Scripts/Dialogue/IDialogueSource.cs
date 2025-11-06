using System;

namespace Virus
{
    public interface IDialogueSource
    {
        event Action OnCinematicDialogueEnd;
        void StartCinematicDialogue(Dialogue dialogue, Action onDialogueEnd = null);
        void StartGameplayDialogue(Dialogue dialogue, Action onDialogueEnd = null);
    }
}
