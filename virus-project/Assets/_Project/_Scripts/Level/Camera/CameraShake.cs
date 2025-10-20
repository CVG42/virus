using Cinemachine;
using UnityEngine;

namespace Virus
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource _impulseSource;

        public void Shake(float force)
        {
            _impulseSource.GenerateImpulse(force);
        }
    }
}
