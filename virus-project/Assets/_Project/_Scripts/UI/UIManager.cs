using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Virus
{
    public class UIManager : Singleton<IUISource>, IUISource
    {
        [SerializeField] private GameObject _pauseCanvas;
        [SerializeField] private GameObject _settingsCanvas;

        private Dictionary<string, Button> _buttonRegistry = new();
        private bool _isButtonLocked = false;

        private void Start()
        {
            if (GameManager.Source != null)
            {
                GameManager.Source.OnGamePaused += OpenPauseScreen;
                GameManager.Source.OnGameUnpaused += HandleGameUnpaused;
            } 
        }

        private void OnDestroy()
        {
            if (GameManager.Source != null)
            {
                GameManager.Source.OnGamePaused -= OpenPauseScreen;
                GameManager.Source.OnGameUnpaused -= HandleGameUnpaused;
            }
        }

        private void HandleGameUnpaused()
        {
            ClosePauseScreen();
            CloseSettingsScreen();
        }

        public void RegisterButton(string key, Button button)
        {
            if (_buttonRegistry.ContainsKey(key))
            {
                _buttonRegistry[key] = button;
            }
            else
            {
                _buttonRegistry.Add(key, button);
            }
        }

        public void AssignButtonAction(string key, UIActionData action)
        {
            if (_buttonRegistry.TryGetValue(key, out Button button))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => action?.Execute());
            }
        }

        public void LockAllButtons()
        {
            if (_isButtonLocked) return;

            _isButtonLocked = true;
            foreach (var keyValue in _buttonRegistry)
            {
                if (keyValue.Value != null)
                {
                    keyValue.Value.interactable = false;
                }
            }
        }

        public void UnlockAllButtons()
        {
            _isButtonLocked = false;
            foreach (var keyValue in _buttonRegistry)
            {
                if (keyValue.Value != null)
                {
                    keyValue.Value.interactable = true;
                }
            }
        }

        public async UniTask UnlockAfterDelayAsync(int delay)
        {
            await UniTask.Delay(delay);
            UnlockAllButtons();
        }

        public void OpenPauseScreen()
        {
            foreach (var button in _pauseCanvas.GetComponentsInChildren<Button>(true))
            {
                button.interactable = true;
            }

            _pauseCanvas.SetActive(true);
        }

        public void ClosePauseScreen()
        {
            _pauseCanvas.SetActive(false);
        }

        public void OpenSettingsScreen()
        {
            LockAllButtons();

            foreach (var button in _settingsCanvas.GetComponentsInChildren<Button>(true))
            {
                button.interactable = true;
            }

            _settingsCanvas.SetActive(true);
        }

        public void CloseSettingsScreen()
        {
            _settingsCanvas.SetActive(false);
            UnlockAllButtons();
        }
    }
}
