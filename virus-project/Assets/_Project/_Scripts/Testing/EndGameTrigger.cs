using UnityEngine;

namespace Virus
{
    public class EndGameTrigger : MonoBehaviour
    {
        [SerializeField] GameObject _endPanel;

        private AntivirusEvent _antivirusEvent;

        private void Start()
        {
            _antivirusEvent = GetComponent<AntivirusEvent>();

            _antivirusEvent.OnHackCompleted += ActivateGameOver;

            _endPanel.SetActive(false);
        }

        private void ActivateGameOver()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _endPanel.SetActive(true);
        }

        private void OnDestroy()
        {
            _antivirusEvent.OnHackCompleted -= ActivateGameOver;
        }
    }
}
