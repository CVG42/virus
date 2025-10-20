using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class TerminalShake : MonoBehaviour
    {
        [SerializeField] private List<RectTransform> _targets = new List<RectTransform>();
        [SerializeField] private float _duration = 0.2f;
        [SerializeField] private float _strength = 30f;
        [SerializeField] private int _vibrato = 10;
        [SerializeField] private float _randomness = 90f;

        private readonly List<Tween> _activeTweens = new();

        public void Shake(float intensity = 1f)
        {
            foreach (var tween in _activeTweens)
                tween?.Kill();
            _activeTweens.Clear();

            foreach (var target in _targets)
            {
                if (target == null) continue;

                Vector2 originalPos = target.anchoredPosition;

                var tween = target.DOShakeAnchorPos(
                    duration: _duration,
                    strength: _strength * intensity,
                    vibrato: _vibrato,
                    randomness: _randomness,
                    snapping: false,
                    fadeOut: true
                )
                .OnComplete(() => target.anchoredPosition = originalPos);

                _activeTweens.Add(tween);
            }
        }
    }
}
