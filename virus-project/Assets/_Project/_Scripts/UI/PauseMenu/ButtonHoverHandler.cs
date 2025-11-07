using UnityEngine;
using UnityEngine.EventSystems;

namespace Virus
{
    public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private PauseMenuTextController _textController;
        [SerializeField] private ButtonType _buttonType;

        [Header("HDR Color Intensity Settings")]
        [SerializeField] private float _defaultIntensity = 2.4f;
        [SerializeField] private float _hoverIntensity = 5.223011f;
        [SerializeField] private Material _buttonMaterial;

        private Color _originalHDRColor;

        public enum ButtonType { Resume, Settings, MainMenu }

        void Start()
        {
            if (_buttonMaterial != null)
            {
                _originalHDRColor = _buttonMaterial.GetColor("_GlowColor");
                SetHDRIntensity(_defaultIntensity);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetHDRIntensity(_hoverIntensity);

            if (_textController != null)
            {
                switch (_buttonType)
                {
                    case ButtonType.Resume:
                        _textController.OnResumeHover();
                        break;
                    case ButtonType.Settings:
                        _textController.OnSettingsHover();
                        break;
                    case ButtonType.MainMenu:
                        _textController.OnMainMenuHover();
                        break;
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _buttonMaterial.SetColor("_GlowColor", _originalHDRColor);
            _textController?.OnHoverExit();
        }

        private void SetHDRIntensity(float intensity)
        {
            if (_buttonMaterial != null)
            {
                Color baseColor = new Color(_originalHDRColor.r, _originalHDRColor.g, _originalHDRColor.b, _originalHDRColor.a);
                Color intensifiedColor = baseColor * intensity;
                intensifiedColor.a = _originalHDRColor.a;
                _buttonMaterial.SetColor("_GlowColor", intensifiedColor);
            }
        }

        void OnDisable()
        {
            _buttonMaterial.SetColor("_GlowColor", _originalHDRColor);
        }
    }
}
