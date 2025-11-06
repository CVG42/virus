using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class Float : MonoBehaviour
    {
        [Header("Floating Settings")]
        [SerializeField] private float _floatAmount = 0.3f;
        [SerializeField] private float _floatDuration = 2f;
        [SerializeField] private Ease _floatEase = Ease.InOutSine;

        private Vector3 _startPosition;
        private Tween _floatTween;

        void Start()
        {
            _startPosition = transform.localPosition;
            InitializeFloating();
        }

        void InitializeFloating()
        {
            _floatTween = transform.DOLocalMoveY(_floatAmount, _floatDuration)
                .SetEase(_floatEase)
                .SetLoops(-1, LoopType.Yoyo)
                .SetRelative(true);
        }

        void OnDestroy()
        {
            _floatTween?.Kill();
        }
    }
}
