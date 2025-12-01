using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class CookieTween : MonoBehaviour
    {
        [Header("Rotation")]
        public float _rotationSpeed = 180f;

        [Header("Float")]
        public float _floatHeight = 0.25f;
        public float _floatDuration = 1.5f;

        private Tween _rotationTween;
        private Tween _floatTween;

        private void Start()
        {
            GameManager.Source.OnGamePaused += PauseAnimation;
            GameManager.Source.OnGameUnpaused += ResumeAnimation;

            CookieMovement();
        }

        private void OnDisable()
        {
            GameManager.Source.OnGamePaused -= PauseAnimation;
            GameManager.Source.OnGameUnpaused -= ResumeAnimation;
            _rotationTween?.Kill();
            _floatTween?.Kill();
        }

        private void CookieMovement()
        {
            Vector3 baseRotation = transform.localEulerAngles;

            _rotationTween = DOTween.To(
                () => transform.localEulerAngles,
                x => transform.localEulerAngles = new Vector3(baseRotation.x, x.y, baseRotation.z),
                new Vector3(baseRotation.x, 360f, baseRotation.z),
                _rotationSpeed
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental)
            .SetAutoKill(false)
            .Pause();

            Vector3 startPos = transform.position;
            _floatTween = transform.DOMoveY(startPos.y + _floatHeight, _floatDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(false)
                .Pause();
        }

        private void PauseAnimation()
        {
            _rotationTween.Pause();
            _floatTween.Pause();
        }

        private void ResumeAnimation()
        {
            _rotationTween.Play();
            _floatTween.Play();
        }

        private void OnDestroy()
        {
            _rotationTween?.Kill();
            _floatTween?.Kill();
        }
    }
}
