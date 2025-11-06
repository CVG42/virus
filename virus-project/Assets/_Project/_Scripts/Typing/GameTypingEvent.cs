using System;
using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class GameTypingEvent : MonoBehaviour
    {
        [SerializeField] private List<TextData> _wordsToType;

        public event Action OnTypingCompleted;

        private int _currentWordIndex = 0;
        private bool _isActivated = false;
        private bool _forceStayInTypingState = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            StartTypingEvent(false);
        }

        public void StartTypingEvent(bool stayInTypingState)
        {
            if (_isActivated) return;
            if (_wordsToType.Count == 0) return;

            _isActivated = true;
            _forceStayInTypingState = stayInTypingState;
            _currentWordIndex = 0;

            GameManager.Source.ChangeState(GameState.OnTyping);
            StartNextWord();
        }

        private void StartNextWord()
        {
            if (_currentWordIndex >= _wordsToType.Count)
            {
                TypingManager.Source.DeactivateUI();
                OnTypingCompleted?.Invoke();

                if (!_forceStayInTypingState)
                {
                    GameManager.Source.ChangeState(GameState.OnPlay);
                }

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

        public void ForceStopTypingEvent()
        {
            TypingManager.Source.DeactivateUI();

            _isActivated = false;

            OnTypingCompleted?.Invoke();

            if (!_forceStayInTypingState)
            {
                GameManager.Source.ChangeState(GameState.OnPlay);
            }
        }

        private void OnDisable()
        {
            TypingManager.Source.OnTypingCompleted -= HandleWordCompleted;
        }
    }
}
