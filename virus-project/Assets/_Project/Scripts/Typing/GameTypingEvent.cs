using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class GameTypingEvent : MonoBehaviour
    {
        [SerializeField] private List<TextData> _wordsToType;

        private int _currentWordIndex = 0;
        private bool _isActivated = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_isActivated) return;
            if (!other.CompareTag("Player")) return;

            GameManager.Source.ChangeState(GameState.OnTyping);

            if (_wordsToType.Count == 0) return;

            _isActivated = true;
            _currentWordIndex = 0;
            StartNextWord();
        }

        private void StartNextWord()
        {
            if (_currentWordIndex >= _wordsToType.Count)
            {
                TypingManager.Source.DeactivateUI();
                GameManager.Source.ChangeState(GameState.OnPlay);
                return;
            }

            TextData currentWord = _wordsToType[_currentWordIndex];
            TypingManager.Source.OnTypingCompleted += HandleWordCompleted;
            TypingManager.Source.StartTypingEvent(currentWord);
        }

        private void HandleWordCompleted()
        {
            TypingManager.Source.OnTypingCompleted -= HandleWordCompleted;
            _currentWordIndex++;
            StartNextWord();
        }

        private void OnDisable()
        {
            TypingManager.Source.OnTypingCompleted -= HandleWordCompleted;
        }
    }
}
