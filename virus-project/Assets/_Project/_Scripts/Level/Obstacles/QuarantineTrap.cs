using UnityEngine;

namespace Virus
{
    public class QuarantineTrap : MonoBehaviour
    {
        [SerializeField] private Renderer _platformRenderer;
        [SerializeField] private Material _newMaterial;

        private GameTypingEvent _typingEvent;

        private void Start()
        {
            _typingEvent = GetComponent<GameTypingEvent>();

            _typingEvent.OnTypingCompleted += ChangeMaterial;
        }

        private void ChangeMaterial()
        {
            if (_platformRenderer == null && _newMaterial == null) return;
            
            _platformRenderer.material = _newMaterial;
        }

        private void OnDestroy()
        {
            _typingEvent.OnTypingCompleted -= ChangeMaterial;
        }
    }
}
