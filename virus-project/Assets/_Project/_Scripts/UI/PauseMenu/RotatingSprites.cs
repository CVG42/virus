using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class RotatingSprites : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float _rotationDuration = 2f;
        [SerializeField] private RotateMode _rotateMode = RotateMode.FastBeyond360;
        [SerializeField] private Ease _easeType = Ease.Linear;

        private RectTransform _rectTransform;
        private Tween _rotationTween;

        void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            StartRotation();
        }

        void StartRotation()
        {
            if (_rectTransform != null)
            {
                _rotationTween = _rectTransform.DORotate(new Vector3(0, 0, 360f), _rotationDuration, _rotateMode)
                    .SetEase(_easeType)
                    .SetLoops(-1, LoopType.Restart)
                    .SetUpdate(UpdateType.Normal, true);
            }
        }

        public void StartRotating()
        {
            if (_rotationTween != null && !_rotationTween.IsPlaying())
            {
                _rotationTween.Play();
            }
            else if (_rectTransform != null && _rotationTween == null)
            {
                StartRotation();
            }
        }

        public void StopRotating()
        {
            _rotationTween?.Pause();
        }

        public void RestartRotation()
        {
            _rotationTween?.Restart();
        }

        void OnEnable()
        {
            StartRotating();
        }

        void OnDisable()
        {
            StopRotating();
        }

        void OnDestroy()
        {
            _rotationTween?.Kill();
        }
    }
}
