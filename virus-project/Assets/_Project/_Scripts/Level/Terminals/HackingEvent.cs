using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class HackingEvent : MonoBehaviour
    {
        [System.Serializable]
        public class DialogueEventPair
        {
            public CinematicDialogueTrigger dialogueTrigger;
            public AntivirusEvent antivirusEvent;
            public float delayBeforeAntivirus = 1f;
        }

        [SerializeField] private List<DialogueEventPair> _eventPairs = new List<DialogueEventPair>();

        private void Start()
        {
            foreach (var pair in _eventPairs)
            {
                if (pair.dialogueTrigger != null && pair.antivirusEvent != null)
                {
                    pair.dialogueTrigger.OnDialogueCompleted += () => StartAntivirusEvent(pair);
                }
            }
        }

        private async void StartAntivirusEvent(DialogueEventPair pair)
        {
            await TriggerAntivirusWithDelay(pair);
        }

        private async UniTask TriggerAntivirusWithDelay(DialogueEventPair pair)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(pair.delayBeforeAntivirus));

            pair.antivirusEvent.ForceStartEvent();
        }

        private void OnDestroy()
        {
            foreach (var pair in _eventPairs)
            {
                if (pair.dialogueTrigger != null)
                {
                    pair.dialogueTrigger.OnDialogueCompleted -= () => StartAntivirusEvent(pair);
                }
            }
        }
    }
}
