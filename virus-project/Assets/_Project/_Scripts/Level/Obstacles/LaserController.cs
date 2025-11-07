using TMPro;
using UnityEngine;

namespace Virus
{
    public class LaserController : MonoBehaviour
    {
        [SerializeField] private GameObject _laserWall;
        [SerializeField] private GameObject _laserDamageCollider;
        [SerializeField] private GameObject _canvas;

        [Header("Typing Settings")]
        [SerializeField] private TextData _textData;
        [SerializeField] private TextMeshProUGUI _typingUI;

        private string _currentWord;
        private int _currentIndex = 0;
        private bool _playerInRange = false;
        private bool _isDisabled = false;
        private bool _isCompleted = false;

        private void Start()
        {
            _currentWord = _textData.textToType.ToUpper();
        }

        private void UpdateUI()
        {
            if (_typingUI == null) return;

            string correctText = $"<color=green>{_currentWord.Substring(0, _currentIndex)}</color>";
            string remainingText = $"<color=black>{_currentWord.Substring(_currentIndex)}</color>";

            _typingUI.text = correctText + remainingText;
        }

        private void HandleTyping(char c)
        {
            if (_isDisabled || !_playerInRange || _isCompleted) return;

            if (char.ToUpper(c) == _currentWord[_currentIndex])
            {
                _currentIndex++;
                UpdateUI();

                if (_currentIndex >= _currentWord.Length)
                {
                    _isCompleted = true;
                    DisableLaser();
                }
            }
        }

        private void DisableLaser()
        {
            AudioManager.Source.PlayLaserOffSFX();
            _isDisabled = true;
            _laserWall.SetActive(false);
            _laserDamageCollider.SetActive(false);
            _typingUI.gameObject.SetActive(false);
            _canvas.SetActive(false);
            _currentIndex = 0;
            AudioManager.Source.StopAmbientAudio();
        }

        private void EnableLaser()
        {
            _isDisabled = false;
            _currentIndex = 0;
            _laserWall.SetActive(true);
            _laserDamageCollider.SetActive(true);
            _typingUI.gameObject.SetActive(true);
            _canvas.SetActive(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
                if (!_isCompleted)
                {
                    AudioManager.Source.PlayAmbientAudio("LaserWall");
                    EnableLaser();
                }

                if (!_isDisabled)
                {
                    InputManager.Source.OnTypingKeyPressedInPlay += HandleTyping;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                AudioManager.Source.StopAmbientAudio();
                InputManager.Source.OnTypingKeyPressedInPlay -= HandleTyping;
            }
        }
    }
}
