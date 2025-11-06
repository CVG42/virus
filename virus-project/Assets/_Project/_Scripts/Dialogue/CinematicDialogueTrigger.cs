using System;
using UnityEngine;

namespace Virus
{
    public class CinematicDialogueTrigger : MonoBehaviour
    {
        [SerializeField] private Dialogue _dialogue;

        public event Action OnDialogueCompleted;

        private void TriggerDialogue()
        {
            GameManager.Source.ChangeState(GameState.OnDialogue);
            DialogueManager.Source.StartCinematicDialogue(_dialogue, OnDialogueCompleted);
        }

        private void OnDialogueComplete()
        {
            OnDialogueCompleted?.Invoke();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                var _rbCollider = collider.GetComponent<Rigidbody>();
                _rbCollider.velocity = Vector3.zero;

                TriggerDialogue();

                GetComponent<Collider>().enabled = false;
            }
        }
    }
}
