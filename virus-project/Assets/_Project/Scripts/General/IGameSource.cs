using System;

namespace Virus
{
    public interface IGameSource
    {
        event Action<GameState> OnGameStateChanged;
        GameState CurrentGameState { get; }
        void ChangeState(GameState state);
    }
}
