using System;

namespace Virus
{
    public interface IInputSource
    {
        event Action OnJumpButtonPressed;
        event Action<char> OnShootLetterPressed;

        float MoveHorizontal { get; }
        float MoveForward { get; }
    }
}
