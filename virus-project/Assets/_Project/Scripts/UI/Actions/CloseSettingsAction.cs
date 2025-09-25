using UnityEngine;

namespace Virus
{
    [CreateAssetMenu(menuName = "UI/Actions/Close Settings Button", fileName = "newButton")]
    public class CloseSettingsAction : UIActionData
    {
        public override void Execute()
        {
            UIManager.Source.CloseSettingsScreen();
        }
    }
}
