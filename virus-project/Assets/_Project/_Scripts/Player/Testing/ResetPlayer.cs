using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Virus
{
    public class ResetPlayer : MonoBehaviour
    {
        [System.Serializable]
        public class TransitionSettings
        {
            public string name;
            public Material materialTemplate;
            public float duration = 0.8f;
            public Color color = Color.black;
        }

        [Header("UI Setup")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _transitionImage;

        [Header("Transition Settings")]
        [SerializeField] private TransitionSettings _pixelateSettings;
        [SerializeField] private TransitionSettings _dissolveSettings;
        [SerializeField] private bool _usePixelate = true;

        [Header("Player")]
        [SerializeField] private GameObject _player;
        [SerializeField] private ParticleSystem _respawnParticles;

        private Material _currentMaterial;
        private bool _isTransitioning = false;
        private Vector3 _startPosition;
        private Transform _currentCheckpoint;

        private void Start()
        {
            _startPosition = transform.position;
            SetupTransitionSystem();
        }

        private void SetupTransitionSystem()
        {
            if (_canvasGroup == null || _transitionImage == null) return;

            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.gameObject.SetActive(false);

            TransitionSettings settings = _usePixelate ? _pixelateSettings : _dissolveSettings;

            if (settings.materialTemplate != null)
            {
                _currentMaterial = new Material(settings.materialTemplate);
                _transitionImage.material = _currentMaterial;

                _currentMaterial.SetColor("_Color", settings.color);
                _currentMaterial.SetFloat("_Progress", 0);

                if (_usePixelate)
                {
                    _currentMaterial.SetFloat("_PixelSize", 20);
                }
                else
                {
                    _currentMaterial.SetFloat("_NoiseScale", 10);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Pit") && !_isTransitioning)
            {
                PerformRespawn().Forget();
            }

            if (other.CompareTag("Checkpoint"))
            {
                _currentCheckpoint = other.transform;
            }
        }

        private async UniTaskVoid PerformRespawn()
        {
            _isTransitioning = true;

            SetPlayerControl(false);

            _canvasGroup.gameObject.SetActive(true);
            await AnimateTransition(0, 1, _pixelateSettings.duration);

            Vector3 respawnPos = _currentCheckpoint != null ?
                _currentCheckpoint.position : _startPosition;

            transform.position = respawnPos;
            _respawnParticles.Play();

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            await UniTask.Delay(300);

            await AnimateTransition(1, 0, _pixelateSettings.duration);

            _canvasGroup.gameObject.SetActive(false);
            SetPlayerControl(true);

            _isTransitioning = false;
        }

        private async UniTask AnimateTransition(float startProgress, float endProgress, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float progress = Mathf.Lerp(startProgress, endProgress, t);

                _canvasGroup.alpha = progress;

                _currentMaterial.SetFloat("_Progress", progress);

                Color matColor = _currentMaterial.GetColor("_Color");
                matColor.a = progress;
                _currentMaterial.SetColor("_Color", matColor);

                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }

            _canvasGroup.alpha = endProgress;
            _currentMaterial.SetFloat("_Progress", endProgress);

            Color finalColor = _currentMaterial.GetColor("_Color");
            finalColor.a = endProgress;
            _currentMaterial.SetColor("_Color", finalColor);
        }

        private void SetPlayerControl(bool enabled)
        {
            if (_player == null) return;

            var components = _player.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component != this && component.GetType() != typeof(Transform))
                {
                    component.enabled = enabled;
                }
            }
        }

        private void OnDestroy()
        {
            if (_currentMaterial != null)
            {
                Destroy(_currentMaterial);
            }
        }
    }
}
