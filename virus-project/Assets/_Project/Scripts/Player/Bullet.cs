using UnityEngine;

namespace Virus
{
    public class Bullet : MonoBehaviour
    {
        private float _speed = 20f;
        private float _lifeTime = 2f;
        private float _timer;
        private BaseEnemy _target;

        public void Initialize(BaseEnemy enemy)
        {
            _target = enemy;
            _timer = _lifeTime;
        }

        private void Update()
        {
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
    }
}
