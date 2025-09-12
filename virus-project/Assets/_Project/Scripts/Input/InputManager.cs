using System;
using UnityEngine;

namespace Virus
{
    public class InputManager : Singleton<IInputSource>, IInputSource
    {
        public float MoveForward => _moveForward;
        public float MoveHorizontal => _moveHorizontal;

        public event Action OnJumpButtonPressed;
        public event Action<char> OnShootLetterPressed;

        private float _moveForward;
        private float _moveHorizontal;

        private void Update()
        {
            CheckMovementInputs();
            CheckTypingInput();
        }

        private void CheckMovementInputs()
        {
            _moveHorizontal = Input.GetAxisRaw("Horizontal");
            _moveForward = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Jump"))
            {
                OnJumpButtonPressed?.Invoke();
            }
        }

        private void CheckTypingInput()
        {
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
    }
}
