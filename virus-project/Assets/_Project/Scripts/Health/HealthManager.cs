using System;
using UnityEngine;

namespace Virus
{
    public class HealthManager : Singleton<IHealthSource>, IHealthSource
    {
        public int CurrentHealth { get ; set ; }

        public event Action OnDeath;
        public event Action OnHealthChanged;

        private int _maxHealth;

        public void Initialize(int maxHealth)
        {
            _maxHealth = maxHealth;
            CurrentHealth = _maxHealth;
            OnHealthChanged?.Invoke();
        }

        public void TakeDamage(int amount)
        {
            CurrentHealth -= amount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth);
            OnHealthChanged?.Invoke();

            if (CurrentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }

        public void RestoreFullHealth()
        {
            CurrentHealth = _maxHealth;
            OnHealthChanged?.Invoke();
        }

        public float GetHealthPercentage() => (float)CurrentHealth / _maxHealth;
    }
}
