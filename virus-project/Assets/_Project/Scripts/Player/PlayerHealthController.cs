using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Virus
{
    public class PlayerHealthController : MonoBehaviour
    {
        [SerializeField] private int _playerHealth;
        [SerializeField] private PlayerHealthBar _healthBar;
        [SerializeField] private CameraShake _cameraShake;

        private void Start()
        {
            HealthManager.Source.Initialize(_playerHealth);
            _healthBar.UpdateHealthBarInstant(1f);

            HealthManager.Source.OnHealthChanged += UpdateHealthBar;
            EnemyManager.Source.OnEnemyAttack += TakeDamage;
            HealthManager.Source.OnDeath += PlayerDead;
        }

        private void OnDestroy()
        {
            EnemyManager.Source.OnEnemyAttack -= TakeDamage;
            HealthManager.Source.OnDeath -= PlayerDead;
            HealthManager.Source.OnHealthChanged -= UpdateHealthBar;
        }

        private void TakeDamage(int damage)
        {
            HealthManager.Source.TakeDamage(damage);
            _cameraShake.Shake(.1f);
        }

        private void UpdateHealthBar()
        {
            float percentage = HealthManager.Source.GetHealthPercentage();
            _healthBar.AnimateHealthChangeAsync(percentage).Forget();
        }

        private void PlayerDead()
        {
            Debug.Log("GAME OVER");
        }
    }
}
