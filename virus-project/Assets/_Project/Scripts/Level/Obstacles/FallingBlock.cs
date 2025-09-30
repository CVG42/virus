using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class FallingBlock : MonoBehaviour
    {
        [SerializeField] private float _prepareUpDistance = 0.3f;
        [SerializeField] private float _prepareUpDuration = 0.15f;
        [SerializeField] private float _fallDistance = 5f;
        [SerializeField] private float _fallDuration = 0.3f;
        [SerializeField] private float _riseDuration = 1f;
        [SerializeField] private float _delayBeforeRise = 1f;

        private Vector3 _startPosition;
        private bool _isFalling;

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void OnTriggerStay(Collider other)
        {
            if (_isFalling) return;

            if (other.CompareTag("Player"))
            {
                TriggerFall().Forget();
            }
        }

        private async UniTaskVoid TriggerFall()
        {
            _isFalling = true;

            Vector3 preparePosition = _startPosition + Vector3.up * _prepareUpDistance;
            await transform.DOMove(preparePosition, _prepareUpDuration)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion();

            Vector3 targetPosition = _startPosition + Vector3.down * _fallDistance;
            await transform.DOMove(targetPosition, _fallDuration)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion();

            await UniTask.Delay(System.TimeSpan.FromSeconds(_delayBeforeRise));

            await transform.DOMove(_startPosition, _riseDuration)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion();

            _isFalling = false;
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}
