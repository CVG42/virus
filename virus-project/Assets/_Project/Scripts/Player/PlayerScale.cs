using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class PlayerScale : MonoBehaviour
    {
        [SerializeField] private PlayerInputVariables _playerVariables;

        private Vector3 _originalScale;
        private float _originalMoveSpeed;

        private Tween _currentTween;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _originalMoveSpeed = _playerVariables.MoveSpeed;
        }

        public void Shrink(Vector3 shrunkScale, float duration, float temporalSpeed)
        {
            _currentTween?.Kill(false);

            _playerVariables.MoveSpeed = temporalSpeed;

            Sequence seq = DOTween.Sequence();

            seq.Append(transform.DOScale(shrunkScale, 0.1f).SetEase(Ease.OutQuad));
            seq.AppendInterval(duration);
            seq.Append(transform.DOScale(_originalScale * 1.2f, 0.15f).SetEase(Ease.OutCubic));
            seq.Append(transform.DOScale(_originalScale * 0.9f, 0.1f).SetEase(Ease.InOutCubic)); 
            seq.Append(transform.DOScale(_originalScale, 0.1f).SetEase(Ease.OutBounce));

            seq.OnComplete(() => _playerVariables.MoveSpeed = _originalMoveSpeed);

            _currentTween = seq;
        }
    }
}
