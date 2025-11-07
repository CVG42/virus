using System;
using UnityEngine;

namespace Virus
{
    public class InputManager : Singleton<IInputSource>, IInputSource
    {
        public float MoveForward => _moveForward;
        public float MoveHorizontal => _moveHorizontal;

        public event Action OnJumpButtonPressed;
        public event Action OnBackspacePressed;
        public event Action OnTerminalActivationPressed;
        public event Action OnPauseButtonPressed;
        public event Action OnConfirmButtonPressed;
        public event Action<char> OnTypingKeyPressed;
        public event Action<char> OnShootLetterPressed;
        public event Action<char> OnTypingKeyPressedInPlay;

        private float _moveForward;
        private float _moveHorizontal;

        private void Update()
        {
            CheckMovementInput();
            CheckTerminalActivationInput();
            CheckShootingInput();
            CheckOnPlayTypingInput();
            CheckTypingInput();
            CheckPauseInput();
            CheckEnterInput();
        }

        private void CheckMovementInput()
        {
            if (GameManager.Source.CurrentGameState != GameState.OnPlay)
            {
                ResetInputValues();
                return;
            }

            _moveHorizontal = Input.GetAxisRaw("Horizontal");
            _moveForward = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Jump"))
            {
                OnJumpButtonPressed?.Invoke();
            }
        }

        private void CheckTerminalActivationInput()
        {
            if (GameManager.Source.CurrentGameState != GameState.OnPlay) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                OnTerminalActivationPressed?.Invoke();
            }
        }

        private void CheckShootingInput()
        {
            if (GameManager.Source.CurrentGameState != GameState.OnPlay) return;

            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (key < KeyCode.A || key > KeyCode.Z) continue;

                if (Input.GetKeyDown(key))
                {
                    char pressed = key.ToString()[0];
                    OnShootLetterPressed?.Invoke(pressed);
                    break;
                }
            }
        }

        private void CheckTypingInput()
        {
            if (GameManager.Source.CurrentGameState is not (GameState.OnAntivirusEvent or GameState.OnTyping)) return;

            if (!string.IsNullOrEmpty(Input.inputString))
            {
                foreach (char c in Input.inputString)
                {
                    if (c == '\b')
                    {
                        OnBackspacePressed?.Invoke();
                        continue;
                    }

                    if (c == '\n' || c == '\r')
                    {
                        continue; 
                    }

                    OnTypingKeyPressed?.Invoke(c);
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnTypingKeyPressed?.Invoke('\n');
            }

            if (Input.GetKeyDown(KeyCode.KeypadDivide)) OnTypingKeyPressed?.Invoke('/');
            if (Input.GetKeyDown(KeyCode.KeypadMultiply)) OnTypingKeyPressed?.Invoke('*');
            if (Input.GetKeyDown(KeyCode.KeypadPeriod)) OnTypingKeyPressed?.Invoke('.');
            if (Input.GetKeyDown(KeyCode.KeypadMinus)) OnTypingKeyPressed?.Invoke('-');
        }

        private void CheckOnPlayTypingInput()
        {
            if (GameManager.Source.CurrentGameState != GameState.OnPlay) return;

            if (!string.IsNullOrEmpty(Input.inputString))
            {
                foreach (char c in Input.inputString)
                {
                    if (c == '\b') continue;
                    if (c == '\n' || c == '\r') continue;

                    OnTypingKeyPressedInPlay?.Invoke(c);
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnTypingKeyPressedInPlay?.Invoke('\n');
        }

        private void CheckPauseInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Source.CurrentGameState != GameState.OnPause)
                {
                    OnPauseButtonPressed?.Invoke();
                    GameManager.Source.ChangeState(GameState.OnPause);
                }
                else
                {
                    GameManager.Source.ResumePreviousState();
                }
            }
        }

        private void CheckEnterInput()
        {
            if (GameManager.Source.CurrentGameState != GameState.OnDialogue) return;

            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnConfirmButtonPressed?.Invoke();
            }
        }

        private void ResetInputValues()
        {
            _moveForward = 0;
            _moveHorizontal = 0;
        }
    }
}
