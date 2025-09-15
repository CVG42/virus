using UnityEngine;

namespace Virus
{
    [CreateAssetMenu(fileName = "PlayerVariables", menuName = "Player Input/Variables")]
    public class PlayerInputVariables : ScriptableObject
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _fallMultiplier;
        [SerializeField] private float _ascendMultiplier;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private LayerMask _groundLayer;

        public float MoveSpeed => _moveSpeed;
        public float JumpForce => _jumpForce;
        public float FallMultiplier => _fallMultiplier;
        public float AscendMultiplier => _ascendMultiplier;
        public float RotationSpeed => _rotationSpeed;
        public LayerMask GroundLayer => _groundLayer;
    }
}
