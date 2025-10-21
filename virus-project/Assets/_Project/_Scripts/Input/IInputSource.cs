using System;

namespace Virus
{
    public interface IInputSource
    {
        event Action OnJumpButtonPressed;
        event Action OnBackspacePressed;
        event Action OnTerminalActivationPressed;
        event Action<char> OnShootLetterPressed;
        event Action<char> OnTypingKeyPressed;

        float MoveHorizontal { get; }
        float MoveForward { get; }
    }
}
