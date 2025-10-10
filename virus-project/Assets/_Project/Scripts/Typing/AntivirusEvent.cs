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

            _barCanvas.SetActive(true);
            _timerBar.fillAmount = 1f;

            _timer = new CountdownTimer(_timeLimit);
            _timer.OnTimerStop += HandleTimerStopped;
            _timer.Start();

            _cts = new CancellationTokenSource();
            RunTimerAsync(_cts.Token).Forget();
        }

        private void HandleEventCompleted()
        {
            Debug.Log("Hack success");
            EndEvent();
        }

        private void HandleTimerStopped()
        {
            if (!_isActivated) return;

            Debug.Log("Player failed to complete typing.");

            EndEvent();
        }

        private void EndEvent()
        {
            _isActivated = false;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            if (_timer != null)
            {
                _timer.OnTimerStop -= HandleTimerStopped;
                _timer.Stop();
            }

            _barCanvas.SetActive(false);

            TypingManager.Source.OnTypingCompleted -= HandleEventCompleted;
            TypingManager.Source.DeactivateUI();

            OnHackCompleted?.Invoke();
            GameManager.Source.ChangeState(GameState.OnPlay);
        }

        private async UniTaskVoid RunTimerAsync(CancellationToken token)
        {
            try
            {
                while (_timer.IsRunning && !token.IsCancellationRequested)
                {
                    _timer.Tick(Time.deltaTime);

                    if (_timerBar != null)
                        _timerBar.fillAmount = _timer.Progress;

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
