using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class Eye : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _player;
        [SerializeField] private float _followDistance = 2f;
        [SerializeField] private float _followHeight = 1.5f;

        [Header("Tilt Settings")]
        [SerializeField] private float _maxTiltAngle = 30f;
        [SerializeField] private float _tiltSmoothness = 0.2f;
        [SerializeField] private float _followTiltThreshold = 0.3f;

        private Vector3 _startPosition;
        private Tween _floatTween;
        private Vector3 _lastPosition;
        private Vector3 _velocity;

        void Start()
        {
            _startPosition = transform.localPosition;
            _lastPosition = transform.position;
        }

        void Update()
        {
            FollowPlayer();
            UpdateTilt();
        }

        void FollowPlayer()
        {
            if (_player == null) return;

            Vector3 targetPosition = _player.position - _player.forward * _followDistance;
            targetPosition.y = _player.position.y + _followHeight;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
        }

        void UpdateTilt()
        {
            if (_player == null) return;

            Vector3 currentPosition = transform.position;
            _velocity = (currentPosition - _lastPosition) / Time.deltaTime;
            _lastPosition = currentPosition;

            float followSpeed = _velocity.magnitude;
            bool isActivelyFollowing = followSpeed > _followTiltThreshold;

            if (isActivelyFollowing)
            {
                Vector3 horizontalVelocity = _velocity;
                horizontalVelocity.y = 0;

                if (horizontalVelocity.magnitude > 0.1f)
                {
                    Quaternion targetTilt = Quaternion.Euler(_maxTiltAngle, 0, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetTilt, _tiltSmoothness);
                }
                else
                {
                    Quaternion targetTilt = Quaternion.Euler(_maxTiltAngle, 0, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetTilt, _tiltSmoothness);
                }
            }
            else
            {
                Quaternion targetTilt = Quaternion.Euler(0, 0, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetTilt, _tiltSmoothness);
            }
        }

        public void SetTiltAngle(float newAngle)
        {
            _maxTiltAngle = newAngle;
        }

        public void SetFollowDistance(float newDistance)
        {
            _followDistance = newDistance;
        }

        void OnDestroy()
        {
            _floatTween?.Kill();
        }
    }
}
