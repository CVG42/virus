using UnityEngine;

namespace Virus
{
    public class PlayerHealthController : MonoBehaviour
    {
        [SerializeField] private int _playerHealth; 

        private void Start()
        {
            HealthManager.Source.CurrentHealth = _playerHealth;
            EnemyManager.Source.OnEnemyAttack += TakeDamage;
            HealthManager.Source.OnDeath += PlayerDead;
        }

        private void OnDestroy()
        {
            EnemyManager.Source.OnEnemyAttack -= TakeDamage;
            HealthManager.Source.OnDeath -= PlayerDead;
        }

        private void TakeDamage(int damage)
        {
            HealthManager.Source.TakeDamage(damage);
        }

        private void PlayerDead()
        {
            Debug.Log("GAME OVER");
        }
    }
}
