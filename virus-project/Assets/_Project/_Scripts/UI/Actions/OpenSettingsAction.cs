using UnityEngine;

namespace Virus
{
    [CreateAssetMenu(menuName = "UI/Actions/Open Settings Button", fileName = "newButton")]
    public class OpenSettingsAction : UIActionData
    {
        public override void Execute()
        {
            UIManager.Source.OpenSettingsScreen();
        }
    }
}
