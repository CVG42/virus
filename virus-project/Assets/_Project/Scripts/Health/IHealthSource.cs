using System;

namespace Virus
{
    public interface IHealthSource 
    {
        int CurrentHealth { get; set; }
        float GetHealthPercentage();

        void Initialize(int maxHealth);
        void TakeDamage(int amount);
        void RestoreFullHealth();

        event Action OnDeath;
        event Action OnHealthChanged;
    }
}
