using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class GameplayDialogueTrigger : MonoBehaviour
    {
        [SerializeField] private Dialogue _dialogue;
        private bool _isTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !_isTriggered)
            {
                _isTriggered = true;
                DialogueManager.Source.StartGameplayDialogue(_dialogue, OnDialogueFinished);

                GetComponent<Collider>().enabled = false;
            }
        }

        private void OnDialogueFinished()
        {
            gameObject.SetActive(false);
        }
    }
}
