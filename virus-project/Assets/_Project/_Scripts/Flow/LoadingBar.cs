using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using System.Threading;
using Virus.Flow;

namespace Virus
{
    public class LoadingBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _loadingText;
        [SerializeField] private TextMeshProUGUI _percentageText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Animation Settings")]
        [SerializeField] private int _barWidth = 50;
        [SerializeField] private float _updateInterval = 0.1f;
        [SerializeField] private char _pacmanOpen = 'C';
        [SerializeField] private char _pacmanClosed = 'c';
        [SerializeField] private char _ghostChar = 'o';
        [SerializeField] private char _dotChar = '.';
        [SerializeField] private char _emptyChar = ' ';

        private bool _isAnimating = false;
        private float _currentProgress = 0f;
        private int _animationFrame = 0;
        private HashSet<int> _eatenGhosts = new HashSet<int>();
        private HashSet<int> _ghostPositions;

        private CancellationTokenSource _cts;

        void Awake()
        {
            _ghostPositions = new HashSet<int>
        {
            _barWidth / 6,
            _barWidth / 3,
            _barWidth / 2,
            (_barWidth * 2) / 3,
            (_barWidth * 5) / 6
        };

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.gameObject.SetActive(false);
            }
        }

        public async UniTask ShowLoadingScreen()
        {
            _currentProgress = 0f;
            _isAnimating = true;
            _animationFrame = 0;
            _eatenGhosts.Clear();

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            if (_canvasGroup != null)
            {
                _canvasGroup.gameObject.SetActive(true);
                await _canvasGroup.DOFade(1f, 0.3f).AsyncWaitForCompletion();
            }

            _ = AnimatePacmanProgress(_cts.Token);
        }

        public async UniTask HideLoadingScreen()
        {
            _isAnimating = false;
            _cts?.Cancel();

            if (_canvasGroup != null)
            {
                await _canvasGroup.DOFade(0f, 0.3f).AsyncWaitForCompletion();
                _canvasGroup.gameObject.SetActive(false);
                FlowManager.Source.LoadScene("Level");
            }
        }

        public void UpdateProgress(float progress)
        {
            _currentProgress = Mathf.Clamp01(progress);
            UpdatePercentageText();
        }

        private async UniTask AnimatePacmanProgress(CancellationToken token)
        {
            int pacmanPosition = 0;

            while (_isAnimating && pacmanPosition < _barWidth && !token.IsCancellationRequested)
            {
                _animationFrame++;

                if (_ghostPositions.Contains(pacmanPosition) && !_eatenGhosts.Contains(pacmanPosition))
                    _eatenGhosts.Add(pacmanPosition);

                UpdateLoadingBar(pacmanPosition);

                int targetPosition = Mathf.FloorToInt(_currentProgress * _barWidth);
                if (pacmanPosition < targetPosition || (_currentProgress >= 1f && pacmanPosition < _barWidth - 1))
                    pacmanPosition++;

                await UniTask.Delay((int)(_updateInterval * 1000), cancellationToken: token);
            }

            if (_currentProgress >= 1f)
                UpdateLoadingBar(_barWidth - 1);
        }

        private void UpdateLoadingBar(int pacmanPos)
        {
            if (_loadingText == null) return;

            var sb = new StringBuilder(_barWidth + 2);
            sb.Append('[');

            for (int i = 0; i < _barWidth; i++)
                sb.Append(GetPositionCharacter(i, pacmanPos));

            sb.Append(']');
            _loadingText.text = sb.ToString();
        }

        private char GetPositionCharacter(int position, int pacmanPos)
        {
            if (position == pacmanPos)
                return GetPacmanCharacter(pacmanPos);

            if (_ghostPositions.Contains(position) && !_eatenGhosts.Contains(position))
                return _ghostChar;

            return position < pacmanPos ? _dotChar : _emptyChar;
        }

        private char GetPacmanCharacter(int pacmanPos)
        {
            bool isEatingGhost = _eatenGhosts.Contains(pacmanPos);

            if (isEatingGhost)
                return _animationFrame % 2 == 0 ? _pacmanOpen : _pacmanClosed;

            return _animationFrame % 6 < 4 ? _pacmanClosed : _pacmanOpen;
        }

        private void UpdatePercentageText()
        {
            if (_percentageText != null)
                _percentageText.text = $" {(_currentProgress * 100):F0}%";
        }
    }
}
