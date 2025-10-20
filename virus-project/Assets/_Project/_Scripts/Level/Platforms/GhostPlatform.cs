using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class GhostPlatform : MonoBehaviour
    {
        [SerializeField] private Renderer _platformRenderer;
        [SerializeField] private Collider _platformCollider;

        [Header("Fade timing")]
        [SerializeField] private float _disappearDelay = 3f;   
        [SerializeField] private float _blinkDuration = 0.5f;  
        [SerializeField] private int _blinkCount = 4;          
        [SerializeField] private float _respawnDelay = 3f;

        [Header("Alpha values")]
        [Range(0f, 1f)][SerializeField] private float invisibleAlpha = 0f;
        [Range(0f, 1f)][SerializeField] private float visibleAlpha = 1f;

        private Material _materialInstance;
        private bool _isActive = true;
        private bool _isProcessing = false;

        private void Awake()
        {
            _materialInstance = _platformRenderer.material;
            SetAlpha(visibleAlpha);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!_isActive || _isProcessing) return;

            if (collision.gameObject.CompareTag("Player"))
            {
                HandleDisappear().Forget();
            }
        }

        private async UniTaskVoid HandleDisappear()
        {
            _isProcessing = true;

            await UniTask.Delay(System.TimeSpan.FromSeconds(_disappearDelay));

            for (int i = 0; i < _blinkCount; i++)
            {
                await _materialInstance
                    .DOFade(invisibleAlpha, _blinkDuration / 2f)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();

                await _materialInstance
                    .DOFade(visibleAlpha, _blinkDuration / 2f)
                    .SetEase(Ease.Linear)
                    .AsyncWaitForCompletion();
            }

            await _materialInstance
                .DOFade(invisibleAlpha, 0.2f)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion();

            _platformCollider.enabled = false;
            _isActive = false;

            await UniTask.Delay(System.TimeSpan.FromSeconds(_respawnDelay));

            await _materialInstance
                .DOFade(visibleAlpha, 0.5f)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion();

            _platformCollider.enabled = true;
            _isActive = true;
            _isProcessing = false;
        }

        private void SetAlpha(float alpha)
        {
            Color color = _materialInstance.color;
            color.a = alpha;
            _materialInstance.color = color;
        }

        private void OnDestroy()
        {
            _materialInstance?.DOKill();
        }
    }
}
