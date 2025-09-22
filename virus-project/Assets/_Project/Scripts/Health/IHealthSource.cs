using System;

namespace Virus
{
    public interface IHealthSource 
    {
        int CurrentHealth { get; set; }
        void TakeDamage(int amount);
        void RestoreFullHealth();

        event Action OnDeath;
        event Action OnHealthChanged;
    }
}
