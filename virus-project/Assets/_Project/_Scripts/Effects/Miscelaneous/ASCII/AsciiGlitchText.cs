using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Virus
{
    public class AsciiGlitchText : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text _targetText;

        [Header("Source")]
        [SerializeField] private bool _useCurrentTextAsSource = true;

        [TextArea(8, 30)]
        [SerializeField] private string _asciiSource;

        [Header("Glitch Settings")]
        [SerializeField] private string _glitchCharacters = "#$%&@!?<>01/\\|[]{}*+=-";

        [SerializeField] private int _minCharactersPerGlitch = 1;
        [SerializeField] private int _maxCharactersPerGlitch = 6;

        [SerializeField] private float _minTimeBetweenGlitches = 0.08f;
        [SerializeField] private float _maxTimeBetweenGlitches = 0.35f;

        [SerializeField] private float _minGlitchDuration = 0.04f;
        [SerializeField] private float _maxGlitchDuration = 0.12f;

        [Header("Filters")]
        [SerializeField] private bool _ignoreSpaces = true;
        [SerializeField] private bool _ignoreLineBreaks = true;
        [SerializeField] private bool _ignoreTabs = true;

        private string _originalText;
        private char[] _originalCharacters;
        private char[] _runtimeCharacters;

        private readonly List<int> _validIndexes = new();
        private readonly List<int> _glitchedIndexes = new();

        private CancellationTokenSource _cts;

        private void Awake()
        {
            if (_targetText == null)
            {
                _targetText = GetComponent<TMP_Text>();
            }

            InitializeText();
        }

        private void OnEnable()
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

            GlitchLoopAsync(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            RestoreOriginalText();
        }

        private void InitializeText()
        {
            if (_targetText == null)
            {
                return;
            }

            _originalText = _useCurrentTextAsSource ? _targetText.text : _asciiSource;

            _originalCharacters = _originalText.ToCharArray();
            _runtimeCharacters = _originalText.ToCharArray();

            _validIndexes.Clear();

            for (int i = 0; i < _originalCharacters.Length; i++)
            {
                char character = _originalCharacters[i];

                if (_ignoreSpaces && character == ' ')
                {
                    continue;
                }

                if (_ignoreTabs && character == '\t')
                {
                    continue;
                }

                if (_ignoreLineBreaks && (character == '\n' || character == '\r'))
                {
                    continue;
                }

                _validIndexes.Add(i);
            }

            _targetText.text = _originalText;
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

                    ApplyRandomGlitch();

                    float glitchDuration = UnityEngine.Random.Range(
                        _minGlitchDuration,
                        _maxGlitchDuration
                    );

                    await UniTask.Delay(
                        TimeSpan.FromSeconds(glitchDuration),
                        cancellationToken: token
                    );

                    RestoreGlitchedCharacters();
                }
            }
            catch (OperationCanceledException)
            {
                RestoreOriginalText();
            }
        }

        private void ApplyRandomGlitch()
        {
            if (_targetText == null || _validIndexes.Count == 0)
            {
                return;
            }

            _glitchedIndexes.Clear();

            int charactersToGlitch = UnityEngine.Random.Range(
                _minCharactersPerGlitch,
                _maxCharactersPerGlitch + 1
            );

            charactersToGlitch = Mathf.Min(charactersToGlitch, _validIndexes.Count);

            int attempts = 0;
            int maxAttempts = charactersToGlitch * 10;

            while (_glitchedIndexes.Count < charactersToGlitch && attempts < maxAttempts)
            {
                attempts++;

                int randomValidIndex = UnityEngine.Random.Range(0, _validIndexes.Count);
                int characterIndex = _validIndexes[randomValidIndex];

                if (_glitchedIndexes.Contains(characterIndex))
                {
                    continue;
                }

                char originalCharacter = _originalCharacters[characterIndex];
                char glitchCharacter = GetRandomGlitchCharacter(originalCharacter);

                _runtimeCharacters[characterIndex] = glitchCharacter;
                _glitchedIndexes.Add(characterIndex);
            }

            _targetText.text = new string(_runtimeCharacters);
        }

        private void RestoreGlitchedCharacters()
        {
            if (_targetText == null)
            {
                return;
            }

            for (int i = 0; i < _glitchedIndexes.Count; i++)
            {
                int index = _glitchedIndexes[i];
                _runtimeCharacters[index] = _originalCharacters[index];
            }

            _glitchedIndexes.Clear();

            _targetText.text = new string(_runtimeCharacters);
        }

        private void RestoreOriginalText()
        {
            if (_targetText == null || _originalCharacters == null)
            {
                return;
            }

            Array.Copy(_originalCharacters, _runtimeCharacters, _originalCharacters.Length);
            _glitchedIndexes.Clear();

            _targetText.text = _originalText;
        }

        private char GetRandomGlitchCharacter(char originalCharacter)
        {
            if (string.IsNullOrEmpty(_glitchCharacters))
            {
                return originalCharacter;
            }

            char randomCharacter = originalCharacter;

            int attempts = 0;

            while (randomCharacter == originalCharacter && attempts < 10)
            {
                int index = UnityEngine.Random.Range(0, _glitchCharacters.Length);
                randomCharacter = _glitchCharacters[index];
                attempts++;
            }

            return randomCharacter;
        }
    }
}
