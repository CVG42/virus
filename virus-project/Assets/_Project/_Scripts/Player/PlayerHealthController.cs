using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Virus
{
    public class PlayerHealthController : MonoBehaviour
    {
        [SerializeField] private int _playerHealth;
        [SerializeField] private PlayerHealthBar _healthBar;
        [SerializeField] private CameraShake _cameraShake;

        private Vector3 _startPosition;

        private void Start()
        {
            HealthManager.Source.Initialize(_playerHealth);
            _healthBar.UpdateHealthBarInstant(1f);

            HealthManager.Source.OnHealthChanged += UpdateHealthBar;
            EnemyManager.Source.OnEnemyAttack += TakeDamage;
            HealthManager.Source.OnDeath += PlayerDead;

            _startPosition = transform.position;
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
            ResetTransform();
        }

        private void ResetTransform()
        {
            transform.position = _startPosition;
            transform.rotation = Quaternion.identity;
            RestoreHealth();
        }

        private async void RestoreHealth()
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));
            HealthManager.Source.RestoreFullHealth();
        }
    }
}
