using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Virus
{
    public class Launcher : MonoBehaviour
    {
        [Header("Launcher Settings")]
        [SerializeField] private Transform launchTarget;
        [SerializeField] private float launchDuration = 2f;
        [SerializeField] private float arcHeight = 3f;
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

            Vector3 start = player.transform.position;
            Vector3 end = launchTarget.position;
            Vector3 mid = Vector3.Lerp(start, end, 0.5f) + Vector3.up * arcHeight;

            Quaternion originalRotation = player.transform.rotation;

            Vector3[] path = new Vector3[] { start, mid, end };

            player.transform
                .DOPath(path, launchDuration, PathType.CatmullRom, PathMode.Full3D, 10)
                .SetEase(launchEase)
                .OnUpdate(() => player.transform.rotation = originalRotation);

            await UniTask.Delay(System.TimeSpan.FromSeconds(launchDuration + extraWaitTime));

            GameManager.Source.ChangeState(GameState.OnPlay);
            _isLaunching = false;
        }
    }
}
