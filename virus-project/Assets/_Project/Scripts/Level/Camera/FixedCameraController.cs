using UnityEngine;

namespace Virus
{
    public class FixedCameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;

        [Header("Camera Settings")]
        [SerializeField] private Vector3 behindOffset = new Vector3(0, 3f, -5f);
        [SerializeField] private Vector3 aheadOffset = new Vector3(0, 3f, 4f);
        [SerializeField] private float followSpeed = 4f;
        [SerializeField] private float rotationSpeed = 3f;
        [SerializeField] private float switchDelay = 0.8f;

        private Vector3 _currentOffset;
        private float _directionChangeTimer;
        private bool _isBehindPlayer = true;

        private void Start()
        {
            _currentOffset = behindOffset;
            RepositionCamera();
        }

        private void LateUpdate()
        {
            HandleCameraSwitching();
            UpdateCamera();
        }

        private void HandleCameraSwitching()
        {
            // Get raw input to determine intent (don't depend on player rotation)
            float verticalInput = Input.GetAxisRaw("Vertical");
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            bool wantsToGoBackwards = verticalInput < -0.1f;
            bool wantsToGoForward = verticalInput > 0.1f;
            bool hasSignificantInput = Mathf.Abs(verticalInput) > 0.1f || Mathf.Abs(horizontalInput) > 0.1f;

            if (!hasSignificantInput)
            {
                _directionChangeTimer = 0f;
                return;
            }

            if (_isBehindPlayer && wantsToGoBackwards)
            {
                _directionChangeTimer += Time.deltaTime;
                if (_directionChangeTimer >= switchDelay)
                {
                    SwitchToAheadView();
                }
            }
            else if (!_isBehindPlayer && wantsToGoForward)
            {
                _directionChangeTimer += Time.deltaTime;
                if (_directionChangeTimer >= switchDelay * 0.6f) // Faster return
                {
                    SwitchToBehindView();
                }
            }
            else
            {
                // Reset timer if input direction doesn't match camera state
                _directionChangeTimer = Mathf.Max(0, _directionChangeTimer - Time.deltaTime);
            }
        }

        private void SwitchToAheadView()
        {
            _currentOffset = aheadOffset;
            _isBehindPlayer = false;
            _directionChangeTimer = 0f;
        }

        private void SwitchToBehindView()
        {
            _currentOffset = behindOffset;
            _isBehindPlayer = true;
            _directionChangeTimer = 0f;
        }

        private void UpdateCamera()
        {
            RepositionCamera();

            // Smooth look at player
            Vector3 lookTarget = player.position + Vector3.up * 1.5f;
            Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void RepositionCamera()
        {
            // Calculate desired position based on player's current world position and facing
            Vector3 desiredPosition = player.position +
                                    player.forward * _currentOffset.z +
                                    Vector3.up * _currentOffset.y;

            // Smooth movement
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }
    }
}
