using UnityEngine;

namespace Virus
{
    public class DefaultState : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Source.ChangeState(GameState.OnPlay);
        }
    }
}
