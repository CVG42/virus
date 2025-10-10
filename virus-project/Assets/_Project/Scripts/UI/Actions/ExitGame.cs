using UnityEngine;

namespace Virus
{
    [CreateAssetMenu(menuName = "UI/Actions/Exit Button", fileName = "Exit Button")]
    public class ExitGame : UIActionData
    {
        public override void Execute()
        {
            Application.Quit();
        }
    }
}
