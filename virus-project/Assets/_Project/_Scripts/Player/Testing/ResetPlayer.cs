using UnityEngine;

namespace Virus
{
    public class ResetPlayer : MonoBehaviour
    {
        [SerializeField] private Transform _checkpoint2;
        [SerializeField] private Transform _checkpoint3;
        [SerializeField] private Transform _checkpoint4;

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

            if (other.CompareTag("Pit2"))
            {
                ResetCheckpoint2();
            }

            if (other.CompareTag("Pit3"))
            {
                ResetCheckpoint3();
            }
        }

        private void ResetTransform()
        {
            transform.position = _startPosition;
            transform.rotation = Quaternion.identity;
        }

        private void ResetCheckpoint2()
        {
            transform.position = _checkpoint2.position;
            transform.rotation = Quaternion.identity;
        }

        private void ResetCheckpoint3()
        {
            transform.position = _checkpoint3.position;
            transform.rotation = Quaternion.identity;
        }

        private void ResetCheckpoint4()
        {
            transform.position = _checkpoint4.position;
            transform.rotation = Quaternion.identity;
        }
    }
}
