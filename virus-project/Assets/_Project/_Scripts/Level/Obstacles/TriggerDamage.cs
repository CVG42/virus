using UnityEngine;

namespace Virus
{
    public class TriggerDamage : MonoBehaviour
    {
        [SerializeField] private int _damage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                AudioManager.Source.PlayLaserDamageSFX();
                EnemyManager.Source.Attack(_damage);
            }
        }
    }
}
