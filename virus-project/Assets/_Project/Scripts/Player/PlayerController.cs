using UnityEngine;

namespace Virus
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInputVariables _playerVariables;

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
            GroundCheck();
        }

        private void FixedUpdate()
        {
            MovePlayer();
            ApplyJumpPhysics();
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
                _groundCheckTimer -= Time.deltaTime;
            }
        }

        private void MovePlayer()
        {
            Vector3 movement = (transform.right * InputManager.Source.MoveHorizontal + transform.forward * InputManager.Source.MoveForward).normalized;
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
    }
}