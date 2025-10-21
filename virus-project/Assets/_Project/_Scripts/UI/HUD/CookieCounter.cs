using TMPro;
using UnityEngine;

namespace Virus
{
    public class CookieCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _cookieCounterText;
        [SerializeField] private string _cookieCounterFormat = "{0}";

        private void Start()
        {
            CollectablesManager.Source.OnCookiesChanged += OnCookiesChanged;
        }

        private void OnCookiesChanged(int totalCookies)
        {
            UpdateCoinCounter(totalCookies);
        }

        private void UpdateCoinCounter(int totalCoins)
        {
            _cookieCounterText.text = string.Format(_cookieCounterFormat, totalCoins);
        }

        private void OnDestroy()
        {
            CollectablesManager.Source.OnCookiesChanged -= OnCookiesChanged;
        }
    }
}
