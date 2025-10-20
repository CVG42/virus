using UnityEngine;

namespace Virus
{
    public class ResetPlayer : MonoBehaviour
    {
        private Vector3 _startPosition;

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Pit"))
            {
                ResetTransform();
            }
        }

        private void ResetTransform()
        {
            transform.position = _startPosition;
            transform.rotation = Quaternion.identity;
        }
    }
}
