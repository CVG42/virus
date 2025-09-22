using System;
using UnityEngine;

namespace Virus
{
    public interface IEnemySource
    {
        event Action<int> OnEnemyAttack;
        void Attack(int damage);
    }
}
