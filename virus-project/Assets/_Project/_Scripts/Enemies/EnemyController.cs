using UnityEngine;

namespace Virus
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private char _letter;
        [SerializeField] private EnemyData _enemy;

        private float _currentHealth;
        private DissolveEffect _dissolveEffect;
        private Collider _enemyCollider;

        public char Letter => _letter;

        private void Start()
        {
            _currentHealth = _enemy.EnemyHealth;

            _dissolveEffect = GetComponent<DissolveEffect>();
            _enemyCollider = GetComponent<Collider>();
        }

        public void Initialize(char letter)
        {
            _letter = char.ToUpper(letter);
        }

        public void TakeDamage()
        {
            _currentHealth--;
            EnemyDeath();
        }

        private void EnemyDeath()
        {
            if (_currentHealth <= 0)
            {
                if (_enemyCollider != null)
                {
                    _enemyCollider.enabled = false;
                }

                if (_dissolveEffect != null)
                {
                    _dissolveEffect.StartDissolve();
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                EnemyManager.Source.Attack(_enemy.AttackDamage);
            }
        }
    }
}
