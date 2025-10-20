using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Virus
{
    public class TerminalTimerEffect : MonoBehaviour
    {
        [SerializeField] private Material _screenDamageMat;
        [SerializeField] private CinemachineImpulseSource _impulseSource;

        private CancellationTokenSource _cts;

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public void TriggerScreenDamage(float intensity)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();

            ScreenDamageAsync(intensity, _cts.Token).Forget();
        }

        private async UniTaskVoid ScreenDamageAsync(float intensity, CancellationToken token)
        {
            var velocity = new Vector3(0, -0.5f, -1).normalized;
            _impulseSource.GenerateImpulse(velocity * intensity * 0.4f);

            float targetRadius = Remap(intensity, 0, 1, 0.71f, .67f);
            float curRadius = 1f;

            for (float t = 0; curRadius != targetRadius; t += Time.deltaTime)
            {
                if (token.IsCancellationRequested) return;
                curRadius = Mathf.Clamp(Mathf.Lerp(1, targetRadius, t), 1, targetRadius);
                _screenDamageMat.SetFloat("_Radius", curRadius);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            for (float t = 0; curRadius < 1; t += Time.deltaTime)
            {
                if (token.IsCancellationRequested) return;
                curRadius = Mathf.Lerp(targetRadius, 1, t);
                _screenDamageMat.SetFloat("_Radius", curRadius);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
        }
    }
}
