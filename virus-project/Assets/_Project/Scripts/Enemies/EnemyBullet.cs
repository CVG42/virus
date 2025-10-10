using UnityEngine;

namespace Virus
{
    public class EnemyBullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 20f;
        
        private float _lifeTime = 2f;
        private float _timer;
        private PlayerController _target;

        public void Initialize(PlayerController player)
        {
            _target = player;
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
