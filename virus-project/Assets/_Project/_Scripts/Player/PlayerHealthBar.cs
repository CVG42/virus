using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Virus
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private float _animationDuration = 0.5f;

        private Tween _currentTween;

        public void UpdateHealthBarInstant(float normalizedValue)
        {
            _fillImage.fillAmount = normalizedValue;
        }

        public async UniTask AnimateHealthChangeAsync(float targetNormalized)
        {
            _currentTween?.Kill();

            _currentTween = _fillImage.DOFillAmount(targetNormalized, _animationDuration)
                .SetEase(Ease.OutCubic);

            await _currentTween.AsyncWaitForCompletion();
        }
    }
}
