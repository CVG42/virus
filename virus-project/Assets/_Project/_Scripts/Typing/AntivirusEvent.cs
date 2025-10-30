using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Virus
{
    public class AntivirusEvent : MonoBehaviour
    {
        [SerializeField] private TextData _textData;
        [SerializeField] private GameObject _barCanvas;
        [SerializeField] private Image _timerBar;
        [SerializeField] private float _timeLimit = 10f;

        [Header("Fullscreen effects")]
        [SerializeField] private TerminalTimerEffect _timerEffect;
        [SerializeField] private FullScreenPassRendererFeature _screenPassRendererFeature;
        [SerializeField] private TerminalShake _terminalShake;

        public event Action OnHackCompleted;

        private bool _isActivated = false;
        private CountdownTimer _timer;
        private CancellationTokenSource _cts;

        private void OnTriggerEnter(Collider other)
        {
            if (_isActivated) return;
            if (!other.CompareTag("Player")) return;

            GameManager.Source.ChangeState(GameState.OnAntivirusEvent);
            _isActivated = true;

            TypingManager.Source.StartTypingEvent(_textData);
            TypingManager.Source.OnTypingCompleted += HandleEventCompleted;

            _screenPassRendererFeature.SetActive(true);
            _barCanvas.SetActive(true);
            _timerBar.fillAmount = 1f;

            _timer = new CountdownTimer(_timeLimit);
            _timer.OnTimerStop += HandleTimerStopped;
            _timer.Start();

            _cts = new CancellationTokenSource();
            RunTimerAsync(_cts.Token).Forget();
            RunEffectPulseAsync(_cts.Token).Forget();
        }

        private void HandleEventCompleted()
        {
            Debug.Log("Hack success");
            EndEvent();
        }

        private void HandleTimerStopped()
        {
            if (!_isActivated) return;

            EnemyManager.Source.Attack(10);

            EndEvent();
        }

        private void EndEvent()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            if (_timer != null)
            {
                _timer.OnTimerStop -= HandleTimerStopped;
                _timer.Stop();
            }

            _screenPassRendererFeature.SetActive(false);
            _barCanvas.SetActive(false);

            TypingManager.Source.OnTypingCompleted -= HandleEventCompleted;
            TypingManager.Source.DeactivateUI();

            OnHackCompleted?.Invoke();
            GameManager.Source.ChangeState(GameState.OnPlay);
        }

        private async UniTaskVoid RunTimerAsync(CancellationToken token)
        {
            float elapsed = 0f;
            float nextPulse = 1f;

            try
            {
                while (_timer.IsRunning && !token.IsCancellationRequested)
                {
                    _timer.Tick(Time.deltaTime);

                    if (_timerBar != null)
                        _timerBar.fillAmount = _timer.Progress;

                    elapsed += Time.deltaTime;

                    if (elapsed >= nextPulse)
                    {
                        _terminalShake?.Shake();
                        nextPulse += 1f;
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                if (_timer.IsFinished && !token.IsCancellationRequested)
                    HandleTimerStopped();
            }
            catch (OperationCanceledException)
            {
                // Expected if event ends early
            }
        }

        private async UniTaskVoid RunEffectPulseAsync(CancellationToken token)
        {
            try
            {
                while (_timer.IsRunning && !token.IsCancellationRequested)
                {
                    _timerEffect.TriggerScreenDamage(UnityEngine.Random.Range(0.1f, 0.6f));

                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
                // expected if event ends early
            }
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();

            if (_timer != null)
                _timer.OnTimerStop -= HandleTimerStopped;

            TypingManager.Source.OnTypingCompleted -= HandleEventCompleted;
        }
    }
}
