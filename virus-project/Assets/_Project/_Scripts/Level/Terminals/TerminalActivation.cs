using UnityEngine;

namespace Virus
{
    public class TerminalActivation : MonoBehaviour
    {
        [SerializeField] private Renderer _terminalRenderer;
        [SerializeField] private Material _newMaterial;
        
        private bool _isPlayerInRange = false;

        private void Start()
        {
            InputManager.Source.OnTerminalActivationPressed += ActivateTerminal;
        }

        private void ActivateTerminal()
        {
            if (_isPlayerInRange && InjectionManager.Source.IsFull)
            {
                if (InjectionManager.Source.TryUseProgress())
                {
                    Debug.Log("Terminal Activated");
                    _terminalRenderer.material = _newMaterial;
                    _isPlayerInRange = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            _isPlayerInRange = true;
        }

        private void OnTriggerExit(Collider other)
        {
            _isPlayerInRange = false;
        }

        private void OnDestroy()
        {
            InputManager.Source.OnTerminalActivationPressed -= ActivateTerminal;
        }
    }
}
