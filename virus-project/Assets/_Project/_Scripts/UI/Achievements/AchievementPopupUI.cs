using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Virus
{
    public class AchievementPopupUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private TMP_Text _titleText;

        [SerializeField] private float _visibleAmount = -500f;

        [Header("Timing")]
        [SerializeField] private float _slideDuration = 0.4f;
        [SerializeField] private float _showDuration = 2.5f;

        private Vector2 _hiddenPos;
        private Vector2 _visiblePos;

        private void Awake()
        {
            _visiblePos = _panel.anchoredPosition;
            _hiddenPos = _visiblePos + new Vector2(_visibleAmount, 0f);

            _panel.anchoredPosition = _hiddenPos;
            _canvasGroup.alpha = 0;
        }

        public void Show(AchievementAttributes achievement)
        {
            _titleText.text = achievement.title;

            Sequence s = DOTween.Sequence();

            s.Append(_canvasGroup.DOFade(1, 0.2f));
            s.Join(_panel.DOAnchorPos(_visiblePos, _slideDuration).SetEase(Ease.OutCubic));

            s.AppendInterval(_showDuration);

            s.Append(_panel.DOAnchorPos(_hiddenPos, 0.4f).SetEase(Ease.InCubic));
            s.Join(_canvasGroup.DOFade(0, 0.2f));
        }
    }
}
