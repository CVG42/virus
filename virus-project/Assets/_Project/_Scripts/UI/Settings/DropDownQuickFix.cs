using UnityEngine;
using UnityEngine.UI;

namespace Virus
{
    public class DropDownQuickFix : MonoBehaviour
    {
        void Start()
        {
            Dropdown dropdown = GetComponent<Dropdown>();
            if (dropdown != null && dropdown.template != null)
            {
                Canvas dropdownCanvas = dropdown.template.GetComponent<Canvas>();
                if (dropdownCanvas != null)
                {
                    dropdownCanvas.overrideSorting = true;
                    dropdownCanvas.sortingOrder = 9999;
                }

                ScrollRect scrollRect = dropdown.template.GetComponentInChildren<ScrollRect>();
                if (scrollRect != null)
                {
                    Canvas scrollCanvas = scrollRect.GetComponent<Canvas>();
                    if (scrollCanvas != null)
                    {
                        scrollCanvas.overrideSorting = true;
                        scrollCanvas.sortingOrder = 10000;
                    }
                }
            }
        }
    }
}
