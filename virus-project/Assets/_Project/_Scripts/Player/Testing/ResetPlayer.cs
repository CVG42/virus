using UnityEngine;

namespace Virus
{
    public class ResetPlayer : MonoBehaviour
    {
        private Vector3 _startPosition;
        private Transform _currentCheckpoint;

        private void Start()
        {
            _startPosition = transform.position;
            _currentCheckpoint = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Pit"))
            {
                Respawn();
            }

            if (other.CompareTag("Checkpoint"))
            {
                SetCheckpoint(other.transform);
            }
        }

        public void SetCheckpoint(Transform checkpointTransform)
        {
            _currentCheckpoint = checkpointTransform;
        }

        private void Respawn()
        {
            if (_currentCheckpoint != null)
            {
                transform.position = _currentCheckpoint.position;
                transform.rotation = _currentCheckpoint.rotation;
            }
            else
            {
                transform.position = _startPosition;
                transform.rotation = Quaternion.identity;
            }
        }
    }
}
