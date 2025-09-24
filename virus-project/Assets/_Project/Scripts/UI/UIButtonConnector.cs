using UnityEngine;
using UnityEngine.UI;

namespace Virus
{
    public class UIButtonConnector : MonoBehaviour
    {
        [SerializeField] private string _buttonKey;
        [SerializeField] private UIActionData _action;

        [Header("Optional")]
        [SerializeField] private bool _disableAllButtonsOnClick = true;
        [SerializeField] private int _reenableDelay = 0;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            UIManager.Source.RegisterButton(_buttonKey, _button);

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(async () =>
            {
                if (_disableAllButtonsOnClick)
                {
                    UIManager.Source.LockAllButtons();

                    if (_reenableDelay > 0)
                    {
                        await UIManager.Source.UnlockAfterDelayAsync(_reenableDelay);
                    }
                }

                _action?.Execute();
            });
        }
    }
}
