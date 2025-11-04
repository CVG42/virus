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
        [SerializeField] private EnemyData _bulletData;
        [SerializeField] private Animator _animator;

        [Header("Patrol Settings")]
        [SerializeField] private Transform _pointA;
        [SerializeField] private Transform _pointB;
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _pointThreshold = 0.2f;

        [Header("Attack Settings")]
        [SerializeField] private float _detectionRange = 10f;
        [SerializeField] private float _fireRate = 1f;
        [SerializeField] private bool _rotateTowardPlayer = true;

        private float _fireTimer;
        private bool _playerDetected;
        private Vector3 _targetPoint;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _cts = new CancellationTokenSource();
            _fireTimer = 1f / _fireRate;
        }

        private void Start()
        {
            _targetPoint = _pointB.position;
        }

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
            PatrolAndFireLoopAsync(_cts.Token).Forget();
        }

        private void OnDisable()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async UniTaskVoid PatrolAndFireLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (GameManager.Source == null || GameManager.Source.CurrentGameState != GameState.OnPlay)
                {
                    _animator.speed = 0f;
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    continue;
                }
                else
                {
                    if (_animator.speed == 0f)
                        _animator.speed = 1f;
                }

                if (_targetPlayer == null)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    continue;
                }

                float distance = Vector3.Distance(transform.position, _targetPlayer.transform.position);
                _playerDetected = distance <= _detectionRange;

                if (_playerDetected)
                {
                    // Stop patrolling
                    if (_rotateTowardPlayer)
                    {
                        Vector3 dir = (_targetPlayer.transform.position - transform.position).normalized;
                        dir.y = 0f;
                        if (dir.sqrMagnitude > 0.01f)
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
                    }

                    _fireTimer -= Time.deltaTime;
                    if (_fireTimer <= 0f)
                    {
                        Fire();
                        _fireTimer = 1f / _fireRate;
                    }
                }
                else
                {
                    // Patrol logic
                    transform.position = Vector3.MoveTowards(transform.position, _targetPoint, _moveSpeed * Time.deltaTime);

                    Vector3 dir = (_targetPoint - transform.position).normalized;
                    dir.y = 0f;
                    if (dir.sqrMagnitude > 0.01f)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

                    if (Vector3.Distance(transform.position, _targetPoint) <= _pointThreshold)
                    {
                        _targetPoint = (_targetPoint == _pointA.position) ? _pointB.position : _pointA.position;
                    }
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }

        private void Fire()
        {
            if (_bulletPrefab == null || _firePoint == null || _targetPlayer == null) return;

            _animator?.SetTrigger("Shoot");
            AudioManager.Source.PlayEnemyShootSFX();

            GameObject bullet = ObjectPoolManager.Source.Borrow(_bulletPrefab);
            bullet.transform.position = _firePoint.position;
            bullet.transform.rotation = _firePoint.rotation;
            bullet.SetActive(true);

            if (bullet.TryGetComponent(out EnemyBullet enemyBullet))
                enemyBullet.Initialize(_targetPlayer, _bulletData);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);

            if (_pointA != null && _pointB != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_pointA.position, _pointB.position);
            }
        }
#endif
    }
}
