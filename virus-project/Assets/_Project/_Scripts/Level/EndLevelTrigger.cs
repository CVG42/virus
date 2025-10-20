using UnityEngine;
using Virus.Flow;

namespace Virus
{
    public class EndLevelTrigger : MonoBehaviour
    {
        [SerializeField] private string _sceneName;

        private void OnTriggerEnter(Collider other)
        {
            FlowManager.Source.LoadScene(_sceneName);
        }
    }
}
