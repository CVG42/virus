using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Virus
{
    public class FinalDialogueTrigger : MonoBehaviour
    {
        [SerializeField] private Dialogue _finalDialogue;
        [SerializeField] private EndLevelHandler _endLevelHandler;

        private void TriggerFinalDialogue()
        {
            GameManager.Source.ChangeState(GameState.OnDialogue);
            DialogueManager.Source.OnCinematicDialogueEnd += HandleFinalDialogueComplete;
            DialogueManager.Source.StartCinematicDialogue(_finalDialogue, null);
        }

        private async void HandleFinalDialogueComplete()
        {
            DialogueManager.Source.OnCinematicDialogueEnd -= HandleFinalDialogueComplete;

            await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f));

            _endLevelHandler.OnLevelCompleted();
            UIManager.Source.OpenGameOverScreen();
            GameManager.Source.ChangeState(GameState.OnGameOver);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var _rbCollider = other.GetComponent<Rigidbody>();
                _rbCollider.velocity = Vector3.zero;

                TriggerFinalDialogue();
                gameObject.SetActive(false);
            }
        }
    }
}
