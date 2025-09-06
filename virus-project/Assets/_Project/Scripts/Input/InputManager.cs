using System;
using UnityEngine;

namespace Virus
{
    public class InputManager : Singleton<IInputSource>, IInputSource
    {
        public float MoveForward => _moveForward;
        public float MoveHorizontal => _moveHorizontal;

        public event Action OnJumpButtonPressed;

        private float _moveForward;
        private float _moveHorizontal;

        private void Update()
        {
            CheckMovementInputs();
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
    }
}
