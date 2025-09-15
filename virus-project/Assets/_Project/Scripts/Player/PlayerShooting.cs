using UnityEngine;

namespace Virus
{
    public class PlayerShooting : MonoBehaviour
    {
        [SerializeField] private float _shootRange = 30f;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _bullet;

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
            GameObject bullet = ObjectPoolManager.Source.Borrow(_bullet);
            bullet.transform.position = _firePoint.position;

            Bullet instantiatedBullet = bullet.GetComponent<Bullet>();
            instantiatedBullet.Initialize(enemy);
        }
    }
}
