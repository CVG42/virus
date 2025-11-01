using System;

namespace Virus
{
    public interface IGameSource
    {
        event Action<GameState> OnGameStateChanged;
        event Action OnGamePaused;
        event Action OnGameUnpaused;
        GameState CurrentGameState { get; }
        void ChangeState(GameState state);
        void ResumePreviousState();
    }
}
