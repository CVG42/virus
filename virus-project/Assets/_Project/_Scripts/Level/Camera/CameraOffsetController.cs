using Cinemachine;
using UnityEngine;

namespace Virus
{
    public class CameraOffsetController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        [Header("Offset Settings")]
        [SerializeField] private float _normalZOffset = -6f;
        [SerializeField] private float _backwardZOffset = -3f;
        [SerializeField] private float _transitionDuration = 0.5f;

        private CinemachineTransposer _transposer;
        private float _currentZOffset;
        private float _targetZOffset;
        private float _offsetChangeVelocity;
        private bool _isMovingBackward = false;

        private void Start()
        {
            _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();

            if (_transposer != null)
            {
                _currentZOffset = _transposer.m_FollowOffset.z;
                _targetZOffset = _normalZOffset;
            }
        }

        private void Update()
        {
            CheckBackwardMovement();
            UpdateCameraOffset();
        }

        private void CheckBackwardMovement()
        {
            bool isBackwardPressed = Input.GetAxisRaw("Vertical") < -0.1f;

            if (isBackwardPressed && !_isMovingBackward)
            {
                _targetZOffset = _backwardZOffset;
                _isMovingBackward = true;
            }
            else if (!isBackwardPressed && _isMovingBackward)
            {
                _targetZOffset = _normalZOffset;
                _isMovingBackward = false;
            }
        }

        private void UpdateCameraOffset()
        {
            if (_transposer != null)
            {
                _currentZOffset = Mathf.SmoothDamp(_currentZOffset, _targetZOffset, ref _offsetChangeVelocity, _transitionDuration);

                Vector3 offset = _transposer.m_FollowOffset;
                offset.z = _currentZOffset;
                _transposer.m_FollowOffset = offset;
            }
        }
    }
}
