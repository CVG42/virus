using UnityEngine;

namespace Virus
{
    public class PlayerShooting : MonoBehaviour
    {
        [SerializeField] private float _shootRange = 30f;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private BulletPool _bulletPool;

        private void Start()
        {
            InputManager.Source.OnShootLetterPressed += TryShootEnemy;
        }

        private void OnDestroy()
        {
            InputManager.Source.OnShootLetterPressed -= TryShootEnemy;
        }

        private void TryShootEnemy(char letter)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _shootRange, _enemyLayer);

            foreach (Collider hit in hits)
            {
                BaseEnemy enemy = hit.GetComponent<BaseEnemy>();
                if (enemy != null && enemy.Letter == letter)
                {
                    Shoot(enemy);
                    return;
                }
            }
        }

        private void Shoot(BaseEnemy enemy)
        {
            GameObject bulletObj = _bulletPool.GetBullet();
            bulletObj.transform.position = _firePoint.position;

            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Initialize(enemy, _bulletPool);
        }
    }
}
