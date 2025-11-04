using System;

namespace Virus
{
    public interface IDialogueSource
    {
        void StartCinematicDialogue(Dialogue dialogue, Action onDialogueEnd = null);
        void StartGameplayDialogue(Dialogue dialogue, Action onDialogueEnd = null);
    }
}
