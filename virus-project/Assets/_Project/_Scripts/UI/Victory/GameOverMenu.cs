using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Virus
{
    public class GameOverMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _dataStolenText;
        [SerializeField] private TextMeshProUGUI _completionText;

        private void OnEnable()
        {
            UpdateProgressDisplay();
            PlayEntranceAnimation();
        }

        private void UpdateProgressDisplay()
        {
            float percentage = InjectionManager.Source.ProgressPercentage * 100f;

            if (InjectionManager.Source.IsFull)
            {
                _dataStolenText.text = "TOTAL SYSTEM BREACH";
                _completionText.text = "100% DATA EXFILTRATED";
            }
            else if (percentage >= 50f)
            {
                _dataStolenText.text = "MAJOR SECURITY BREACH";
                _completionText.text = $"{percentage:F0}% CRITICAL DATA STOLEN";
            }
            else
            {
                _dataStolenText.text = "PARTIAL SECURITY BREACH";
                _completionText.text = $"{percentage:F0}% DATA FRAGMENTS RECOVERED";
            }
        }

        private void PlayEntranceAnimation()
        {
            float targetPercentage = InjectionManager.Source.ProgressPercentage * 100f;
            float currentPercentage = 0f;

            DOTween.To(() => currentPercentage, x => currentPercentage = x, targetPercentage, 2f)
                .OnUpdate(() => {
                    _completionText.text = $"{currentPercentage:F1}% DATA RECOVERED";
                })
                .SetEase(Ease.OutCubic);
        }
    }
}
