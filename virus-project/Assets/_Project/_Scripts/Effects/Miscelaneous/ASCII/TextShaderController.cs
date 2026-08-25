using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Virus
{
    public class TextShaderController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text _targetText;

        [Header("Idle Time")]
        [SerializeField] private float _minTimeBetweenGlitches = 0.4f;
        [SerializeField] private float _maxTimeBetweenGlitches = 1.8f;

        [Header("Glitch Pulse")]
        [SerializeField] private float _minGlitchDuration = 0.04f;
        [SerializeField] private float _maxGlitchDuration = 0.16f;

        [SerializeField] private float _minGlitchAmount = 4f;
        [SerializeField] private float _maxGlitchAmount = 22f;

        [SerializeField] private float _minChromaticAmount = 0.5f;
        [SerializeField] private float _maxChromaticAmount = 4f;

        [Header("Shader Settings")]
        [SerializeField, Range(0f, 1f)] private float _sliceChance = 0.25f;
        [SerializeField] private float _sliceFrequency = 0.05f;
        [SerializeField] private float _sliceSpeed = 25f;
        [SerializeField, Range(0f, 1f)] private float _jitter = 1f;

        private Material _runtimeMaterial;
        private CancellationTokenSource _cts;

        private static readonly int GlitchAmountId = Shader.PropertyToID("_GlitchAmount");
        private static readonly int ChromaticAmountId = Shader.PropertyToID("_ChromaticAmount");
        private static readonly int SliceAmountId = Shader.PropertyToID("_SliceAmount");
        private static readonly int SliceFrequencyId = Shader.PropertyToID("_SliceFrequency");
        private static readonly int SliceSpeedId = Shader.PropertyToID("_SliceSpeed");
        private static readonly int JitterId = Shader.PropertyToID("_Jitter");

        private void Awake()
        {
            if (_targetText == null)
            {
                _targetText = GetComponent<TMP_Text>();
            }

            CreateRuntimeMaterial();
        }

        private void OnEnable()
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(
                this.GetCancellationTokenOnDestroy()
            );

            GlitchLoopAsync(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            ResetGlitch();
        }

        private void OnDestroy()
        {
            if (_runtimeMaterial != null)
            {
                Destroy(_runtimeMaterial);
            }
        }

        private void CreateRuntimeMaterial()
        {
            if (_targetText == null)
            {
                return;
            }

            _runtimeMaterial = Instantiate(_targetText.fontSharedMaterial);
            _targetText.fontSharedMaterial = _runtimeMaterial;

            ApplyBaseSettings();
            ResetGlitch();
        }

        private void ApplyBaseSettings()
        {
            if (_runtimeMaterial == null)
            {
                return;
            }

            _runtimeMaterial.SetFloat(SliceAmountId, _sliceChance);
            _runtimeMaterial.SetFloat(SliceFrequencyId, _sliceFrequency);
            _runtimeMaterial.SetFloat(SliceSpeedId, _sliceSpeed);
            _runtimeMaterial.SetFloat(JitterId, _jitter);
        }

        private async UniTaskVoid GlitchLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    float waitTime = UnityEngine.Random.Range(
                        _minTimeBetweenGlitches,
                        _maxTimeBetweenGlitches
                    );

                    await UniTask.Delay(
                        TimeSpan.FromSeconds(waitTime),
                        cancellationToken: token
                    );

                    TriggerGlitch();

                    float duration = UnityEngine.Random.Range(
                        _minGlitchDuration,
                        _maxGlitchDuration
                    );

                    await UniTask.Delay(
                        TimeSpan.FromSeconds(duration),
                        cancellationToken: token
                    );

                    ResetGlitch();
                }
            }
            catch (OperationCanceledException)
            {
                ResetGlitch();
            }
        }

        private void TriggerGlitch()
        {
            if (_runtimeMaterial == null)
            {
                return;
            }

            float glitchAmount = UnityEngine.Random.Range(
                _minGlitchAmount,
                _maxGlitchAmount
            );

            float chromaticAmount = UnityEngine.Random.Range(
                _minChromaticAmount,
                _maxChromaticAmount
            );

            _runtimeMaterial.SetFloat(GlitchAmountId, glitchAmount);
            _runtimeMaterial.SetFloat(ChromaticAmountId, chromaticAmount);
        }

        private void ResetGlitch()
        {
            if (_runtimeMaterial == null)
            {
                return;
            }

            _runtimeMaterial.SetFloat(GlitchAmountId, 0f);
            _runtimeMaterial.SetFloat(ChromaticAmountId, 0f);
        }
    }
}
