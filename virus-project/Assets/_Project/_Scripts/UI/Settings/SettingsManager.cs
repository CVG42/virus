using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Virus
{
    public class SettingsManager : Singleton<ISettingsSource>, ISettingsSource
    {
        private const ColorblindMode DEFAULT_COLORBLIND_MODE = ColorblindMode.None;
        private const string COLORBLIND_MODE_KEY = "ColorblindMode";

        [SerializeField] private Material _colorblindMaterial;

        public ColorblindMode CurrentColorblindMode { get; private set; }

        public event Action<ColorblindMode> OnModeChanged;

        private void Start()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            var savedMode = PlayerPrefs.GetInt(COLORBLIND_MODE_KEY, (int)DEFAULT_COLORBLIND_MODE);
            CurrentColorblindMode = (ColorblindMode)savedMode;

            ApplyColorblindMode(CurrentColorblindMode);
        }

        public void SetColorblindMode(ColorblindMode mode)
        {
            CurrentColorblindMode = mode;

            ApplyColorblindMode(mode);

            PlayerPrefs.SetInt(COLORBLIND_MODE_KEY, (int)mode);
            PlayerPrefs.Save();

            OnModeChanged?.Invoke(mode);
        }

        private void ApplyColorblindMode(ColorblindMode mode)
        {
            var shader = _colorblindMaterial.shader;
            var none = new LocalKeyword(shader, "_COLORBLIND_MODE_NONE");
            var tritanopia = new LocalKeyword(shader, "_COLORBLIND_MODE_TRITANOPIA");
            var protonopia = new LocalKeyword(shader, "_COLORBLIND_MODE_PROTONOPIA");
            var deuteranopia = new LocalKeyword(shader, "_COLORBLIND_MODE_DEUTERANOPIA");

            _colorblindMaterial.SetKeyword(none, false);
            _colorblindMaterial.SetKeyword(tritanopia, false);
            _colorblindMaterial.SetKeyword(protonopia, false);
            _colorblindMaterial.SetKeyword(deuteranopia, false);

            switch (mode)
            {
                case ColorblindMode.None:
                    _colorblindMaterial.SetKeyword(none, true);
                    break;

                case ColorblindMode.Tritanopia:
                    _colorblindMaterial.SetKeyword(tritanopia, true);
                    break;

                case ColorblindMode.Protonopia:
                    _colorblindMaterial.SetKeyword(protonopia, true);
                    break;

                case ColorblindMode.Deuteranopia:
                    _colorblindMaterial.SetKeyword(deuteranopia, true);
                    break;
            }
        }
    }

    public interface ISettingsSource
    {
        event Action<ColorblindMode> OnModeChanged;

        ColorblindMode CurrentColorblindMode { get; }
        void SetColorblindMode(ColorblindMode mode);
    }

    public enum ColorblindMode
    {
        None = 0,
        Tritanopia = 1,
        Protonopia = 2,
        Deuteranopia = 3
    }
}
