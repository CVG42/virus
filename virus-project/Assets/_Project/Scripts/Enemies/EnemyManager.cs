using System;
using UnityEngine;

namespace Virus
{
    public class EnemyManager : Singleton<IEnemySource>, IEnemySource
    {
        public event Action<int> OnEnemyAttack;
        public event Action<int> OnDamageReceived;

        public void Attack(int damage)
        {
            OnEnemyAttack?.Invoke(damage);
        }

        public void TakeDamage(int amount)
        {
            OnDamageReceived?.Invoke(amount);
        }
    }
}
