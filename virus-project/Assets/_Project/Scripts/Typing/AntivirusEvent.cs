using UnityEngine;

namespace Virus
{
    public class AntivirusEvent : MonoBehaviour
    {
        [SerializeField] private TextData _textData;

        private bool _isActivated = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_isActivated) return;
            if (!other.CompareTag("Player")) return;

            GameManager.Source.ChangeState(GameState.OnAntivirusEvent);

            _isActivated = true;
            TypingManager.Source.StartTypingEvent(_textData);
            TypingManager.Source.OnTypingCompleted += HandleEventCompleted;
        }

        private void HandleEventCompleted()
        {
            TypingManager.Source.OnTypingCompleted -= HandleEventCompleted;
            TypingManager.Source.DeactivateUI();
            GameManager.Source.ChangeState(GameState.OnPlay);
        }

        private void OnDisable()
        {
            TypingManager.Source.OnTypingCompleted -= HandleEventCompleted;
        }
    }
}
