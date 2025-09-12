using UnityEngine;

namespace Virus
{
    public class Bullet : MonoBehaviour
    {
        private float _speed = 20f;
        private float _lifeTime = 2f;
        private float _timer;
        private BaseEnemy _target;
        private BulletPool _pool;

        public void Initialize(BaseEnemy enemy, BulletPool bulletPool)
        {
            _target = enemy;
            _pool = bulletPool;
            _timer = _lifeTime;
        }

        private void Update()
        {
            if (_target == null)
            {
                _pool.ReturnBullet(gameObject);
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _target.transform.position) < 0.3f)
            {
                _target.TakeDamage();
                _pool.ReturnBullet(gameObject);
            }

            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _pool.ReturnBullet(gameObject);
            }
        }
    }
}
