using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Virus
{
    public class GameManager : Singleton<IGameSource>, IGameSource
    {
        public GameState CurrentGameState { get; private set; }

        public event Action<GameState> OnGameStateChanged;

        public void ChangeState(GameState state)
        {
            if (CurrentGameState == state) return;

            CurrentGameState = state;
            OnGameStateChanged?.Invoke(CurrentGameState);
        }

        private void SetPauseState()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu") return;

            switch (CurrentGameState)
            {
                case GameState.OnPlay:
                    ChangeState(GameState.OnPause);
                    break;
                case GameState.OnPause:
                    ChangeState(GameState.OnPlay);
                    break;
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
