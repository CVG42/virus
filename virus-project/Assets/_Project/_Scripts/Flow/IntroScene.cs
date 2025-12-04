using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Virus.Flow;

namespace Virus
{
    public class IntroScene : MonoBehaviour
    {
        [SerializeField] private float _waitTime = 5f;

        private CancellationTokenSource _cancellationTokenSource;

        private async void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await UniTask.Delay((int)(_waitTime * 1000),
                    cancellationToken: _cancellationTokenSource.Token);

                FlowManager.Source.LoadScene("MainMenu");
            }
            catch (System.OperationCanceledException) { }
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
