using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Virus
{
    public class UIManager : Singleton<IUISource>, IUISource
    {
        private Dictionary<string, Button> _buttonRegistry = new();
        private bool _isButtonLocked = false;

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
    }
}
