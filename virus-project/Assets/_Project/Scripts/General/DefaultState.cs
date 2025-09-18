using UnityEngine;

namespace Virus
{
    public class DefaultState : MonoBehaviour
    {
        private void Start()
        {
            GameStateManager.Source.ChangeState(GameState.OnPlay);
        }
    }
}
