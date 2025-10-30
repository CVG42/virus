using UnityEngine;

namespace Virus
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputVariables _playerVariables;
        [SerializeField] private Animator _animator;

        private bool _isGrounded = true;
        private float _groundCheckTimer = 0f;
        private float _groundCheckDelay = 0.3f;
        private float _playerHeight;
        private float _raycastDistance;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _rigidbody.freezeRotation = true;
            _playerHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
            _raycastDistance = (_playerHeight / 2) + 0.2f;

            InputManager.Source.OnJumpButtonPressed += Jump;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            UpdateAnimations();
        }

        private void FixedUpdate()
        {
            MovePlayer();
            GroundCheck();
            ApplyJumpPhysics();
            RotatePlayer();
        }

        private void OnDestroy()
        {
            InputManager.Source.OnJumpButtonPressed -= Jump;
        }

        private void GroundCheck()
        {
            if (!_isGrounded && _groundCheckTimer <= 0f)
            {
                Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
                _isGrounded = Physics.Raycast(rayOrigin, Vector3.down, _raycastDistance, _playerVariables.GroundLayer);
            }
            else
            {
                _groundCheckTimer -= Time.fixedDeltaTime;
            }
        }

        private void MovePlayer()
        {
            Vector3 movement = (Vector3.right * InputManager.Source.MoveHorizontal + Vector3.forward * InputManager.Source.MoveForward).normalized;
            Vector3 targetVelocity = movement * _playerVariables.MoveSpeed;

            Vector3 velocity = _rigidbody.velocity;
            velocity.x = targetVelocity.x;
            velocity.z = targetVelocity.z;
            _rigidbody.velocity = velocity;

            if (_isGrounded && InputManager.Source.MoveHorizontal == 0 && InputManager.Source.MoveForward == 0)
            {
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
            }
        }

        private void Jump()
        {
            if (_isGrounded)
            {
                _isGrounded = false;
                _groundCheckTimer = _groundCheckDelay;
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _playerVariables.JumpForce, _rigidbody.velocity.z);
            }
        }

        private void ApplyJumpPhysics()
        {
            if (_rigidbody.velocity.y < 0)
            {
                _rigidbody.velocity += Vector3.up * Physics.gravity.y * _playerVariables.FallMultiplier * Time.fixedDeltaTime;
            }
            else if (_rigidbody.velocity.y > 0)
            {
                _rigidbody.velocity += Vector3.up * Physics.gravity.y * _playerVariables.AscendMultiplier * Time.fixedDeltaTime;
            }
        }

        private void RotatePlayer()
        {
            float horizontal = InputManager.Source.MoveHorizontal;  
            float vertical = InputManager.Source.MoveForward;      

            Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

            if (inputDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(inputDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _playerVariables.RotationSpeed * Time.fixedDeltaTime);
            }
        }

        private void UpdateAnimations()
        {
            Vector3 flatVelocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            float speed = flatVelocity.magnitude;

            _animator.SetFloat("Speed", speed);
            _animator.SetBool("isGrounded", _isGrounded);

            if (!_isGrounded)
            {
                if (_rigidbody.velocity.y > 0.1f)
                {
                    _animator.SetBool("isJumping", true);
                    _animator.SetBool("isFalling", false);
                }
                else if (_rigidbody.velocity.y < -0.1f)
                {
                    _animator.SetBool("isJumping", false);
                    _animator.SetBool("isFalling", true);
                }
            }
            else
            {
                _animator.SetBool("isJumping", false);
                _animator.SetBool("isFalling", false);
            }
        }
    }
}