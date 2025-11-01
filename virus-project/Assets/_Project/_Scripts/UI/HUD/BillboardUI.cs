using UnityEngine;

namespace Virus
{
    public class BillboardUI : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_mainCamera == null) return;

            transform.rotation = Quaternion.LookRotation(transform.position - _mainCamera.transform.position);
        }
    }
}
