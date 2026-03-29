using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Virus
{
    public class DissolveEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private GameObject _mesh;
        [SerializeField] private GameObject _canvas;

        [Header("Dissolve Settings")]
        [SerializeField] private float _dissolveTime = 0.75f;
        [SerializeField] private bool _useDissolve = true;
        [SerializeField] private bool _useVertical = false;
        [SerializeField] private bool _destroyOnComplete = true;
        [SerializeField] private bool _useScaledTime = true;

        [Header("Dissolve Material")]
        [SerializeField] private Material _dissolveMaterialTemplate;

        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        private static readonly int VerticalDissolveAmount = Shader.PropertyToID("_VerticalDissolveAmount");

        private SkinnedMeshRenderer[] _skinnedMeshRenderers;
        private Material[] _originalMaterials;
        private Material[] _dissolveMaterials;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isDissolving = false;

        private void Awake()
        {
            _skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            if (_skinnedMeshRenderers != null && _skinnedMeshRenderers.Length > 0)
            {
                SaveOriginalMaterials();
            }
        }

        private void SaveOriginalMaterials()
        {
            List<Material> originalMats = new List<Material>();

            foreach (var renderer in _skinnedMeshRenderers)
            {
                if (renderer != null)
                {
                    originalMats.AddRange(renderer.sharedMaterials);
                }
            }

            _originalMaterials = originalMats.ToArray();
        }

        public void StartDissolve()
        {
            if (_isDissolving) return;

            _isDissolving = true;

            if (_dissolveMaterialTemplate == null)
            {
                Debug.LogError("No dissolve material template assigned!", this);
                _isDissolving = false;
                return;
            }

            ApplyDissolveMaterials();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            DissolveAsync(_cancellationTokenSource.Token).Forget();
        }

        private void ApplyDissolveMaterials()
        {
            if (_skinnedMeshRenderers == null || _skinnedMeshRenderers.Length == 0)
                return;

            List<Material> dissolveMatsList = new List<Material>();

            foreach (var renderer in _skinnedMeshRenderers)
            {
                if (renderer == null) continue;

                Material[] instanceMaterials = new Material[renderer.sharedMaterials.Length];

                for (int i = 0; i < instanceMaterials.Length; i++)
                {
                    instanceMaterials[i] = new Material(_dissolveMaterialTemplate);
                    dissolveMatsList.Add(instanceMaterials[i]);
                }

                renderer.materials = instanceMaterials;
            }

            _dissolveMaterials = dissolveMatsList.ToArray();

            InitializeDissolveValues();
        }

        private void InitializeDissolveValues()
        {
            if (_dissolveMaterials == null) return;

            foreach (var mat in _dissolveMaterials)
            {
                if (mat != null)
                {
                    if (_useDissolve)
                        mat.SetFloat(DissolveAmount, 0f);

                    if (_useVertical)
                        mat.SetFloat(VerticalDissolveAmount, 0f);
                }
            }
        }

        private async UniTask WaitForParticles()
        {
            if (_particles == null) return;

            float waitTime = 1.5f;
            float elapsed = 0f;

            while (elapsed < waitTime && (_particles.isPlaying || _particles.particleCount > 0))
            {
                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }
        }

        private async UniTaskVoid DissolveAsync(CancellationToken cancellationToken)
        {
            float elapsedTime = 0f;

            while (elapsedTime < _dissolveTime && !cancellationToken.IsCancellationRequested)
            {
                elapsedTime += _useScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;

                float progress = elapsedTime / _dissolveTime;
                float dissolveValue = Mathf.Lerp(0, 1f, progress);
                float verticalDissolveValue = Mathf.Lerp(0, 1.1f, progress);

                UpdateDissolveValues(dissolveValue, verticalDissolveValue);

                await UniTask.Yield(cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                RestoreOriginalMaterials();
                _isDissolving = false;
                return;
            }

            UpdateDissolveValues(1f, 1.1f);
            _mesh.SetActive(false);
            _canvas.SetActive(false);
            _particles.Play();

            await WaitForParticles();

            await UniTask.Yield();

            OnDissolveComplete();
        }

        private void UpdateDissolveValues(float dissolveValue, float verticalDissolveValue)
        {
            if (_dissolveMaterials == null) return;

            foreach (var mat in _dissolveMaterials)
            {
                if (mat == null) continue;

                if (_useDissolve)
                    mat.SetFloat(DissolveAmount, dissolveValue);

                if (_useVertical)
                    mat.SetFloat(VerticalDissolveAmount, verticalDissolveValue);
            }
        }

        private void OnDissolveComplete()
        {
            _isDissolving = false;

            if (_destroyOnComplete)
            {
                DisableComponents();
                gameObject.SetActive(false);
            }
            else
            {
                DisableComponents();
                gameObject.SetActive(false);

                RestoreOriginalMaterials();
            }
        }

        private void RestoreOriginalMaterials()
        {
            if (_skinnedMeshRenderers == null || _originalMaterials == null)
                return;

            int materialIndex = 0;

            foreach (var renderer in _skinnedMeshRenderers)
            {
                if (renderer == null) continue;

                Material[] originalMats = new Material[renderer.sharedMaterials.Length];

                for (int i = 0; i < originalMats.Length && materialIndex < _originalMaterials.Length; i++)
                {
                    originalMats[i] = _originalMaterials[materialIndex];
                    materialIndex++;
                }

                renderer.materials = originalMats;
            }

            _dissolveMaterials = null;
        }

        private void DisableComponents()
        {
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            var animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }
        }

        public void CancelDissolve()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            if (_isDissolving)
            {
                RestoreOriginalMaterials();
                _isDissolving = false;
            }
        }

        private void OnDestroy()
        {
            CancelDissolve();
        }

        private void OnDisable()
        {
            CancelDissolve();
        }
    }
}
