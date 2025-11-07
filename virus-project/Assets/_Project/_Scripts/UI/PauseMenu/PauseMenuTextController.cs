using TMPro;
using UnityEngine;

namespace Virus
{
    public class PauseMenuTextController : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_Text statusText;

        [Header("Hover Messages")]
        public string resumeHoverText = "Resume";
        public string settingsHoverText = "Settings";
        public string mainMenuHoverText = "Main Menu";
        public string defaultText = "Paused";

        void Start()
        {
            if (statusText != null)
            {
                statusText.text = defaultText.Localize();
            }
        }

        public void OnResumeHover()
        {
            statusText.text = resumeHoverText.Localize();
        }

        public void OnSettingsHover()
        {
            statusText.text = settingsHoverText.Localize();
        }

        public void OnMainMenuHover()
        {
            statusText.text = mainMenuHoverText.Localize();
        }

        public void OnHoverExit()
        {
            statusText.text = defaultText.Localize();
        }
    }
}
