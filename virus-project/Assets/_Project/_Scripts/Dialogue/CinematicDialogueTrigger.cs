using UnityEngine;

namespace Virus
{
    public class CinematicDialogueTrigger : MonoBehaviour
    {
        [SerializeField] private Dialogue _dialogue;

        private void TriggerDialogue()
        {
            GameManager.Source.ChangeState(GameState.OnDialogue);
            DialogueManager.Source.StartCinematicDialogue(_dialogue, null);
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                var _rbCollider = collider.GetComponent<Rigidbody>();
                _rbCollider.velocity = Vector3.zero;

                TriggerDialogue();

                gameObject.SetActive(false);
            }
        }
    }
}
