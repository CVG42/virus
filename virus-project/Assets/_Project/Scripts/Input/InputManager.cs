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
        public event Action<char> OnTypingKeyPressed;
        public event Action<char> OnShootLetterPressed;

        private float _moveForward;
        private float _moveHorizontal;

        private void Update()
        {
            CheckMovementInput();
            CheckShootingInput();
            CheckTypingInput();
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

                    if (c == '\n' || c == '\r') continue;

                    OnTypingKeyPressed?.Invoke(c);
                }
            }
        }

        private void ResetInputValues()
        {
            _moveForward = 0;
            _moveHorizontal = 0;
        }
    }
}
