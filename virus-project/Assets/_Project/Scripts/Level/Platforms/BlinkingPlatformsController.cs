using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public class BlinkingPlatformsController : MonoBehaviour
    {
        [Header("Blocks")]
        [SerializeField] private List<GameObject> _firstBatch;
        [SerializeField] private List<GameObject> _secondBatch;

        [Header("Settings")]
        [SerializeField] private bool _isFirstBatchActiveAtStart;
        [SerializeField] private float _activeTime = 3f;
        [SerializeField] private float _blinkDuration = 0.5f;
        [SerializeField] private int _blinkCount = 3;

        private bool _running = false;
        private bool _playerInside = false;
        private bool _isFirstBatchActive;
        
        private Material _firstBatchMaterial;
        private Material _secondBatchMaterial;

        private void Awake()
        {
            InitialSetUp();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = true;

                if (!_running)
                {
                    BlinkCycle().Forget();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInside = false;
            }
        }

        private async UniTaskVoid BlinkCycle()
        {
            _running = true;

            while (_playerInside)
            {
                SetActive(_firstBatch, true);
                SetActive(_secondBatch, false);

                await UniTask.Delay(System.TimeSpan.FromSeconds(_activeTime - _blinkDuration));
                await BlinkBlocks(_firstBatchMaterial);

                if (!_playerInside) break;

                SetActive(_secondBatch, true);
                SetActive(_firstBatch, false);

                await UniTask.Delay(System.TimeSpan.FromSeconds(_activeTime - _blinkDuration));
                await BlinkBlocks(_secondBatchMaterial);
            }

            _running = false;
        }

        private void SetActive(List<GameObject> blocks, bool active)
        {
            foreach (var block in blocks)
            {
                block.SetActive(active);
            }
        }

        private async UniTask BlinkBlocks(Material material)
        {
            if (material == null) return;

            for (int i = 0; i < _blinkCount; i++)
            {
                material.DOFade(0f, _blinkDuration / 2f);
                await UniTask.Delay(System.TimeSpan.FromSeconds(_blinkDuration / 2f));

                material.DOFade(1f, _blinkDuration / 2f);
                await UniTask.Delay(System.TimeSpan.FromSeconds(_blinkDuration / 2f));
            }
        }

        private void InitialSetUp()
        {
            if (_firstBatch.Count > 0)
            {
                _firstBatchMaterial = _firstBatch[0].GetComponent<Renderer>().sharedMaterial;
            }

            if (_secondBatch.Count > 0)
            {
                _secondBatchMaterial = _secondBatch[0].GetComponent<Renderer>().sharedMaterial;
            }

            _isFirstBatchActive = _isFirstBatchActiveAtStart;
            SetActive(_firstBatch, _isFirstBatchActive);
            SetActive(_secondBatch, !_isFirstBatchActive);
        }
    }
}
