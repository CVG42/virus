using System;

namespace Virus
{
    public interface IStateSource
    {
        event Action<GameState> OnGameStateChanged;
        GameState CurrentGameState { get; }
        void ChangeState(GameState state);
    }
}
