using UnityEngine;

namespace Virus
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private char _letter;
        [SerializeField] private EnemyData _enemy;

        private float _currentHealth;

        public char Letter => _letter;

        private void Start()
        {
            _currentHealth = _enemy.EnemyHealth;
        }

        public void Initialize(char letter)
        {
            _letter = char.ToUpper(letter);
        }

        public void TakeDamage()
        {
            _currentHealth--;
            EnemyDead();
        }

        private void EnemyDead()
        {
            if (_currentHealth <= 0)
            {
                gameObject.SetActive(false);
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
