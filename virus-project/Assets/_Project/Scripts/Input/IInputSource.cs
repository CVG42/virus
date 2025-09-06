using System;

namespace Virus
{
    public interface IInputSource
    {
        event Action OnJumpButtonPressed;

        float MoveHorizontal { get; }
        float MoveForward { get; }
    }
}
