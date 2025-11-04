using System;

namespace Virus
{
    public interface IInputSource
    {
        event Action OnJumpButtonPressed;
        event Action OnBackspacePressed;
        event Action OnTerminalActivationPressed;
        event Action OnConfirmButtonPressed;
        event Action<char> OnShootLetterPressed;
        event Action<char> OnTypingKeyPressed;
        event Action<char> OnTypingKeyPressedInPlay;

        float MoveHorizontal { get; }
        float MoveForward { get; }
    }
}
