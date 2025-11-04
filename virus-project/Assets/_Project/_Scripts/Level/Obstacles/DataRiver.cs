using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class DataRiver : MonoBehaviour
    {
        [SerializeField] private GameTypingEvent _typingEvent;

        [Header("Launcher Settings")]
        [SerializeField] private Transform launchTarget;
        [SerializeField] private float launchDuration = 2f;
        [SerializeField] private Ease launchEase = Ease.InOutSine;

        [Header("Timing")]
        [SerializeField] private float extraWaitTime = 0.5f;

        private bool _isLaunching;
        private bool _wordsCompleted = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_isLaunching) return;

            if (other.CompareTag("Player"))
            {

                LaunchPlayer(other.gameObject).Forget();

            }
        }

        private async UniTaskVoid LaunchPlayer(GameObject player)
        {
            _isLaunching = true;
            _wordsCompleted = false;

            _typingEvent.OnTypingCompleted += HandleWordsCompleted;

            _typingEvent.StartTypingEvent(true);

            player.transform.DOMove(launchTarget.position, launchDuration)
                .SetEase(launchEase);

            player.transform.DOLookAt(launchTarget.position, 0.5f);

            await UniTask.Delay(System.TimeSpan.FromSeconds(launchDuration + extraWaitTime));

            _typingEvent.OnTypingCompleted -= HandleWordsCompleted;

            if (!_wordsCompleted)
            {
                EnemyManager.Source.Attack(4);
            }

            GameManager.Source.ChangeState(GameState.OnPlay);

            _isLaunching = false;
        }

        private void HandleWordsCompleted()
        {
            _wordsCompleted = true;
        }
    }
}
