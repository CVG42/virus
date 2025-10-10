using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace Virus
{
    public class Enemy : MonoBehaviour
    {
        public enum MoveAxis { X, Z }

        [Header("Patrol Settings")]
        [SerializeField] private MoveAxis patrolAxis = MoveAxis.X;
        [SerializeField] private float patrolDistance = 3f;
        [SerializeField] private float patrolSpeed = 2f;
        [SerializeField] private float pauseTime = 1f;

        [Header("Detection Settings")]
        [SerializeField] private Transform player;
        [SerializeField] private float chaseRange = 6f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float loseSightTime = 3f;

        [Header("Attack Settings")]
        [SerializeField] private float attackDuration = 0.6f;

        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;

        private NavMeshAgent _agent;
        private Vector3 _patrolCenter;
        private Vector3 _patrolPointA;
        private Vector3 _patrolPointB;
        private Vector3 _currentTarget;

        private bool _isPatrolling = true;
        private bool _isChasing = false;
        private bool _isAttacking = false;
        private bool _isPaused = false;

        private float _lastSeenPlayerTime;
        private Tween _attackTween;
        private CancellationTokenSource _destroyCts;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _patrolCenter = transform.position;
            CalculatePatrolPoints();
            _destroyCts = new CancellationTokenSource();
        }

        private void Start()
        {
            StartPatrol();
        }

        // TO-DO: REFACTOR
        private void Update()
        {
            if (GameManager.Source.CurrentGameState != GameState.OnPlay)
            {
                if (_agent.enabled && _agent.isOnNavMesh)
                { 
                    _agent.isStopped = true; 
                }
                return;
            }

            if (player == null || !_agent.isOnNavMesh) return;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (_isAttacking || _isPaused) return;

            if (distanceToPlayer <= attackRange)
            {
                if (!_isAttacking)
                { 
                    AttackPlayerAsync(_destroyCts.Token).Forget(); 
                }
            }
            else if (distanceToPlayer <= chaseRange)
            {
                StartChase();
                _lastSeenPlayerTime = Time.time;
            }
            else
            {
                if (_isChasing && Time.time - _lastSeenPlayerTime >= loseSightTime)
                {
                    ReturnToPatrol();
                }

                if (_isPatrolling)
                {
                    Patrol();
                }
            }
        }

        #region Patrol
        private void CalculatePatrolPoints()
        {
            if (patrolAxis == MoveAxis.X)
            {
                _patrolPointA = _patrolCenter + Vector3.right * patrolDistance;
                _patrolPointB = _patrolCenter - Vector3.right * patrolDistance;
            }
            else
            {
                _patrolPointA = _patrolCenter + Vector3.forward * patrolDistance;
                _patrolPointB = _patrolCenter - Vector3.forward * patrolDistance;
            }
        }

        private void StartPatrol()
        {
            _isPatrolling = true;
            _isChasing = false;
            _agent.speed = patrolSpeed;

            _currentTarget = _patrolPointA;
            if (_agent.isOnNavMesh)
                _agent.SetDestination(_currentTarget);
        }

        private void Patrol()
        {
            if (!_isPatrolling || !_agent.isOnNavMesh) return;

            if (!_agent.pathPending && _agent.remainingDistance <= 0.2f)
            {
                // Swap patrol direction
                _currentTarget = _currentTarget == _patrolPointA ? _patrolPointB : _patrolPointA;
                PausePatrolAsync(_destroyCts.Token).Forget();
            }
        }

        private async UniTaskVoid PausePatrolAsync(CancellationToken token)
        {
            _isPaused = true;
            _agent.isStopped = true;
            await UniTask.Delay((int)(pauseTime * 1000), cancellationToken: token).SuppressCancellationThrow();

            if (token.IsCancellationRequested) return;

            _isPaused = false;
            _agent.isStopped = false;

            if (_agent.isOnNavMesh)
                _agent.SetDestination(_currentTarget);
        }
        #endregion

        #region Chase
        private void StartChase()
        {
            if (!_agent.isOnNavMesh) return;

            _isPatrolling = false;
            _isChasing = true;
            _agent.speed = patrolSpeed * 1.8f;
            _agent.SetDestination(player.position);
        }

        private void ReturnToPatrol()
        {
            if (!_agent.isOnNavMesh) return;

            _isChasing = false;
            _isPatrolling = true;
            _agent.speed = patrolSpeed;
            _currentTarget = FindClosestPatrolPoint();
            _agent.SetDestination(_currentTarget);
        }

        private Vector3 FindClosestPatrolPoint()
        {
            float distA = Vector3.Distance(transform.position, _patrolPointA);
            float distB = Vector3.Distance(transform.position, _patrolPointB);
            return distA < distB ? _patrolPointA : _patrolPointB;
        }
        #endregion

        #region Attack
        private async UniTaskVoid AttackPlayerAsync(CancellationToken token)
        {
            if (!_agent.isOnNavMesh) return;

            _isAttacking = true;
            _isChasing = false;
            _agent.isStopped = true;
            _agent.enabled = false;

            _attackTween?.Kill();

            transform.DOLookAt(player.position, 0.2f);

            Vector3 attackDir = (player.position - transform.position);
            attackDir.y = 0f;
            attackDir.Normalize();

            float stopDistance = 0.8f;
            Vector3 endPos = player.position - attackDir * stopDistance;
            endPos.y = transform.position.y;

            float distance = Vector3.Distance(transform.position, endPos);
            float arcHeight = Mathf.Clamp(distance * 0.6f, 1f, 3f);

            float duration = attackDuration;
            float elapsed = 0f;
            Vector3 startPos = transform.position;

            while (elapsed < duration)
            {
                if (token.IsCancellationRequested) return;

                if (GameManager.Source.CurrentGameState != GameState.OnPlay) return;

                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                Vector3 pos = Vector3.Lerp(startPos, endPos, t);

                pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

                transform.position = pos;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            await UniTask.Delay(300, cancellationToken: token).SuppressCancellationThrow();
            if (token.IsCancellationRequested) return;

            if (GameManager.Source.CurrentGameState != GameState.OnPlay) return;

            if (this != null)
            {
                _agent.enabled = true;
                _agent.isStopped = false;
            }

            _isAttacking = false;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= chaseRange)
            {
                StartChase();
            }
            else
            {
                ReturnToPatrol();
            }
        }
        #endregion

        #region Cleanup
        private void OnDestroy()
        {
            Cleanup();
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _attackTween?.Kill();
            if (_destroyCts != null && !_destroyCts.IsCancellationRequested)
                _destroyCts.Cancel();
            _destroyCts?.Dispose();
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            if (Application.isPlaying)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(_patrolPointA, _patrolPointB);
            }
            else
            {
                Vector3 center = transform.position;
                Vector3 a, b;
                if (patrolAxis == MoveAxis.X)
                {
                    a = center + Vector3.right * patrolDistance;
                    b = center - Vector3.right * patrolDistance;
                }
                else
                {
                    a = center + Vector3.forward * patrolDistance;
                    b = center - Vector3.forward * patrolDistance;
                }
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(a, b);
            }
        }
#endif
    }
}
