using UnityEngine;

namespace Virus
{
    public class Bullet : MonoBehaviour
    {
        private float _speed = 20f;
        private float _lifeTime = 2f;
        private float _timer;
        private EnemyController _target;
        private bool _isPaused = false;

        private void OnEnable()
        {
            GameManager.Source.OnGamePaused += PauseBullet;
            GameManager.Source.OnGameUnpaused += ResumeBullet;
        }

        private void OnDisable()
        {
            GameManager.Source.OnGamePaused -= PauseBullet;
            GameManager.Source.OnGameUnpaused -= ResumeBullet;
        }

        public void Initialize(EnemyController enemy)
        {
            _target = enemy;
            _timer = _lifeTime;
        }

        private void Update()
        {
            if (_isPaused) return;

            if (_target == null)
            {
                ObjectPoolManager.Source.Return(gameObject);
                return;
            }

            BulletShot();
            DestroyBulletAfterTime();
        }

        private void BulletShot()
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _target.transform.position) < 0.3f)
            {
                _target.TakeDamage();
                ObjectPoolManager.Source.Return(gameObject);
            }
        }

        private void DestroyBulletAfterTime()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                ObjectPoolManager.Source.Return(gameObject);
            }
        }

        private void PauseBullet()
        {
            _isPaused = true;
        }

        private void ResumeBullet()
        {
            _isPaused = false;
        }
    }
}
