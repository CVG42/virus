using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class DataRiver : MonoBehaviour
    {
        [Header("Launcher Settings")]
        [SerializeField] private Transform launchTarget;
        [SerializeField] private float launchDuration = 2f;
        [SerializeField] private Ease launchEase = Ease.InOutSine;

        [Header("Timing")]
        [SerializeField] private float extraWaitTime = 0.5f;

        private bool _isLaunching;

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

            GameManager.Source.ChangeState(GameState.OnTyping);

            player.transform.DOMove(launchTarget.position, launchDuration)
                .SetEase(launchEase);

            player.transform.DOLookAt(launchTarget.position, 0.5f);

            await UniTask.Delay(System.TimeSpan.FromSeconds(launchDuration + extraWaitTime));

            GameManager.Source.ChangeState(GameState.OnPlay);

            _isLaunching = false;
        }
    }
}
