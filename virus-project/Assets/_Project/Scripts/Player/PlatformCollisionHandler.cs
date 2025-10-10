using UnityEngine;

namespace Virus
{
    public class PlatformCollisionHandler : MonoBehaviour
    {
        private Transform _platform; 

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                ContactPoint contact = other.GetContact(0);
                if (contact.normal.y < 0.5f) return;

                _platform = other.transform;
                transform.SetParent(_platform);
            }
        }

        void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                transform.SetParent(null);
                _platform = null;
            }
        }
    }
}
