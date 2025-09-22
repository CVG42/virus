using UnityEngine;

namespace Virus
{
    [CreateAssetMenu(menuName = "Enemies/EnemyData", fileName = "NewEnemy")]
    public class EnemyData : ScriptableObject
    {
        public int AttackDamage;
        public float EnemyHealth;
    }
}
