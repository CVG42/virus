using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Virus
{
    public class DistortionResponsiveUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        private Button button;
        private Image image;
        private RectTransform rectTransform;

        [Header("Distortion Compensation")]
        [Range(1.0f, 2.0f)] public float clickAreaMultiplier = 1.3f;
        [Range(1.0f, 2.0f)] public float hoverAreaMultiplier = 1.2f;

        void Start()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();

            if (image != null)
            {
                // Expand the clickable area
                float widthPadding = rectTransform.rect.width * (clickAreaMultiplier - 1f) * 0.5f;
                float heightPadding = rectTransform.rect.height * (clickAreaMultiplier - 1f) * 0.5f;
                image.raycastPadding = new Vector4(-widthPadding, -heightPadding, -widthPadding, -heightPadding);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (button.interactable)
            {
                button.onClick.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerEnterHandler);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ExecuteEvents.Execute(gameObject, eventData, ExecuteEvents.pointerExitHandler);
        }
    }
}
