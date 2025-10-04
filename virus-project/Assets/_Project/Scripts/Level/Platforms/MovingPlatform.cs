using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private enum MoveDirection { Horizontal, Vertical, Forward };
        [SerializeField] private MoveDirection _moveDirection;
        [SerializeField] private float _distance = 5f;
        [SerializeField] private float _duration = 2f;
        [SerializeField] private Ease _easeType = Ease.InOutSine;

        private Vector3 _startPosition;
        private Tween _moveTween;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _startPosition = transform.position;
        }

        private void Start()
        {
            Vector3 targetOffset = Vector3.zero;

            switch (_moveDirection)
            {
                case MoveDirection.Vertical:
                    targetOffset = Vector3.up * _distance;
                    break;

                case MoveDirection.Horizontal:
                    targetOffset = Vector3.right * _distance;
                    break;

                case MoveDirection.Forward:
                    targetOffset = Vector3.forward * _distance;
                    break;
            }

            Vector3 targetPosition = _startPosition + targetOffset;

            _moveTween = _rigidbody.DOMove(targetPosition, _duration)
                .SetEase(_easeType)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.SetParent(transform, true);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.SetParent(null, true);
            }
        }

        private void OnDestroy()
        {
            _moveTween?.Kill();
        }
    }
}
