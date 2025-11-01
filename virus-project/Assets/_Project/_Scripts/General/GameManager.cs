using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Virus
{
    public class GameManager : Singleton<IGameSource>, IGameSource
    {
        public GameState CurrentGameState { get; private set; }

        public event Action OnGamePaused;
        public event Action OnGameUnpaused;
        public event Action<GameState> OnGameStateChanged;

        private GameState _previousState;
        
        public void ChangeState(GameState state)
        {
            if (CurrentGameState == state) return;

            if (state == GameState.OnPause)
            {
                _previousState = CurrentGameState;
            }

            CurrentGameState = state;
            OnGameStateChanged?.Invoke(CurrentGameState);

            CheckPauseState(state);
        }

        public void ResumePreviousState()
        {
            ChangeState(_previousState);
        }

        private void CheckPauseState(GameState state)
        {
            if (state == GameState.OnPause)
            {
                OnGamePaused?.Invoke();
            }
            else
            {
                OnGameUnpaused?.Invoke();
            }
        }
    }

    public enum GameState
    {
        OnPlay,
        OnPause,
        OnAntivirusEvent,
        OnTyping
    }
}
