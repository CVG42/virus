using UnityEngine;
using Virus.Flow;

namespace Virus
{
    [CreateAssetMenu(menuName = "UI/Actions/Change Scene Button", fileName = "newButton")]
    public class ChangeSceneAction : UIActionData
    {
        [SerializeField] private string _sceneName;

        public override void Execute()
        {
            FlowManager.Source.LoadScene(_sceneName);
        }
    }
}
