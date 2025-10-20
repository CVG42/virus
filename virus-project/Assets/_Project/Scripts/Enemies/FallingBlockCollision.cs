using UnityEngine;

namespace Virus
{
    public class FallingBlockCollision : MonoBehaviour
    {
        [SerializeField] private int _damage = 5;
        [SerializeField] private float _temporalSpeed = 5; 
        [SerializeField] private float _shrinkDuration = 3f;
        [SerializeField] private Vector3 _shrinkPorpotions;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var scaler = other.GetComponent<PlayerScale>();
            scaler?.Shrink(_shrinkPorpotions, _temporalSpeed, _shrinkDuration);

            EnemyManager.Source.Attack(_damage);
        }
    }
}
