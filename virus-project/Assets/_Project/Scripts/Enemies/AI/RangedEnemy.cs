using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Virus
{
    public class RangedEnemy : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController _targetPlayer; 
        [SerializeField] private GameObject _bulletPrefab;       
        [SerializeField] private Transform _firePoint;           

        [Header("Settings")]
        [SerializeField] private float _detectionRange = 10f;
        [SerializeField] private float _fireRate = 1f;
        [SerializeField] private bool _rotateTowardPlayer = true;

        private float _fireTimer;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _cts = new CancellationTokenSource();
            _fireTimer = 1f / _fireRate;
        }

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
            FireLoopAsync(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async UniTaskVoid FireLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (GameManager.Source == null || GameManager.Source.CurrentGameState != GameState.OnPlay)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    continue;
                }

                if (_targetPlayer == null)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    continue;
                }

                float distance = Vector3.Distance(transform.position, _targetPlayer.transform.position);

                if (distance <= _detectionRange && _rotateTowardPlayer)
                {
                    Vector3 dir = (_targetPlayer.transform.position - transform.position).normalized;
                    dir.y = 0f; 
                    if (dir.sqrMagnitude > 0.01f)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
                }

                if (distance <= _detectionRange)
                {
                    _fireTimer -= Time.deltaTime;
                    if (_fireTimer <= 0f)
                    {
                        Fire();
                        _fireTimer = 1f / _fireRate;
                    }
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        private void Fire()
        {
            if (_bulletPrefab == null || _firePoint == null || _targetPlayer == null)
                return;

            GameObject bullet = ObjectPoolManager.Source.Borrow(_bulletPrefab);
            bullet.transform.position = _firePoint.position;
            bullet.transform.rotation = _firePoint.rotation;
            bullet.SetActive(true);

            if (bullet.TryGetComponent(out EnemyBullet enemyBullet))
                enemyBullet.Initialize(_targetPlayer);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
        
        }
#endif
    }
}
