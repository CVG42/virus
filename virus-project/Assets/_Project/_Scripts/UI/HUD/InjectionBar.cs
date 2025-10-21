using UnityEngine;
using UnityEngine.UI;

namespace Virus
{
    public class InjectionBar : MonoBehaviour
    {
        [SerializeField] private Image _injectionBarFill;

        private void Start()
        {
            InjectionManager.Source.OnProgressChanged += OnProgressChanged;
        }

        private void OnProgressChanged(int currentProgress, float progressPercentage)
        {
            UpdateProgressBar(currentProgress, progressPercentage);
        }

        private void UpdateProgressBar(int currentProgress, float progressPercentage)
        {
            if (_injectionBarFill != null)
            {
                _injectionBarFill.fillAmount = progressPercentage;
            }
        }

        private void OnDestroy()
        {
            InjectionManager.Source.OnProgressChanged -= OnProgressChanged;
        }
    }
}
