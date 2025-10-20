using System.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Virus
{
    public class TextHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float _effectDuration = 0.5f;

        private TMP_Text _text;
        private string _originalText;
        private bool _isHovering;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_text == null) return;

            _originalText = _text.text;
            _isHovering = true;
            AnimateGlitch().Forget();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;

            if (_text != null)
            { 
                _text.text = _originalText; 
            }
        }

        private async UniTaskVoid AnimateGlitch()
        {
            StringBuilder sb = new StringBuilder();
            float timer = 0f;

            while (timer < _effectDuration && _isHovering)
            {
                sb.Clear();
                foreach (char c in _originalText)
                {
                    sb.Append(Random.Range(0, 2) == 0 ? (char)Random.Range(48, 58) : c);
                }

                _text.text = sb.ToString();

                await UniTask.Delay(50);
                timer += 0.05f;
            }

            _text.text = _originalText;
        }
    }
}
