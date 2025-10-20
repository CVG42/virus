using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Virus
{
    public class TypingManager : Singleton<ITypingSource>, ITypingSource
    {
        [Header("Antivirus UI Components")]
        [SerializeField] private GameObject _antivirusCanvas;
        [SerializeField] private TextMeshProUGUI _originalText;
        [SerializeField] private TextMeshProUGUI _typedText;

        [Header("Regular Typing UI Components")]
        [SerializeField] private GameObject _typingEventCanvas;
        [SerializeField] private TextMeshProUGUI _eventWordText;

        [Header("Typing Settings")]
        [SerializeField] private float _sentenceCompletionDelay = 0.5f;
        
        public event Action OnTypingCompleted;

        private string _targetText;
        private int _currentIndex = 0;
        private TextData _textData;

        private void Start()
        {
            InputManager.Source.OnTypingKeyPressed += HandleChars;
            InputManager.Source.OnBackspacePressed += HandleBackspace;
        }

        private void OnDestroy()
        {
            InputManager.Source.OnTypingKeyPressed -= HandleChars;
            InputManager.Source.OnBackspacePressed -= HandleBackspace;
        }

        private void UpdateUI()
        {
            string correctText = $"<color=green>{_targetText.Substring(0, _currentIndex)}</color>";
            string remainingText = $"<color=grey>{_targetText.Substring(_currentIndex)}</color>";

            if (GameManager.Source.CurrentGameState == GameState.OnAntivirusEvent)
            {
                 _originalText.text = correctText + remainingText;
                _typedText.text = _targetText.Substring(0, _currentIndex);
            }
            else if (GameManager.Source.CurrentGameState == GameState.OnTyping)
            {
                _eventWordText.text = correctText + remainingText;
            }
        }

        private async void HandleChars(char input)
        {
            if (_currentIndex >= _targetText.Length) return;

            char expectedChar = _targetText[_currentIndex];

            if (char.ToUpper(input) == expectedChar)
            {
                _currentIndex++;
                UpdateUI();

                if (_currentIndex >= _targetText.Length)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_sentenceCompletionDelay));
                    OnTypingCompleted?.Invoke();
                }
            }
        }

        private void HandleBackspace()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                UpdateUI();
            }
        }

        public void StartTypingEvent(TextData textData)
        {          
            _textData = textData;
            _targetText = _textData.textToType.Replace("\r\n", "\n").Replace("\r", "\n").ToUpper();
            _currentIndex = 0;

            ActivateUI();
            UpdateUI();
        }

        public void ActivateUI()
        {
            if (GameManager.Source.CurrentGameState == GameState.OnAntivirusEvent)
            {
                _antivirusCanvas.SetActive(true);
            }
            else if (GameManager.Source.CurrentGameState == GameState.OnTyping)
            {
                _typingEventCanvas.SetActive(true);
            }
        }

        public void DeactivateUI()
        {
            if (GameManager.Source.CurrentGameState == GameState.OnAntivirusEvent)
            {
                _antivirusCanvas.SetActive(false);
            }
            else if (GameManager.Source.CurrentGameState == GameState.OnTyping)
            {
                _typingEventCanvas.SetActive(false);
            }
        }
    }
}
