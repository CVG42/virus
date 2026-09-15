using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Virus
{
    public class ColorblindDropdown : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;

        private void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
        }

        private void Start()
        {
            LoadCurrentValue();
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDestroy()
        {
            _dropdown.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void LoadCurrentValue()
        {
            _dropdown.SetValueWithoutNotify((int)SettingsManager.Source.CurrentColorblindMode);
        }

        private void OnValueChanged(int index)
        {
            var mode = (ColorblindMode)index;

            SettingsManager.Source.SetColorblindMode(mode);
        }
    }
}
