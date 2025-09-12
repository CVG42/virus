using UnityEngine;

namespace Virus
{
    public class BaseEnemy : MonoBehaviour
    {
        [SerializeField] private char _letter;
        public char Letter => _letter;

        public void Initialize(char letter)
        {
            _letter = char.ToUpper(letter);
        }

        public void TakeDamage()
        {
            Destroy(gameObject);
        }
    }
}
