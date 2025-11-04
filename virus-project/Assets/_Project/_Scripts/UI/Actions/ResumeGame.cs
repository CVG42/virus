using UnityEngine;

namespace Virus
{
    [CreateAssetMenu(menuName = "UI/Actions/Resume Game Button", fileName = "ResumeGame")]
    public class ResumeGame : UIActionData
    {
        public override void Execute()
        {
            GameManager.Source.ResumePreviousState();
            UIManager.Source.ClosePauseScreen();
        }
    }
}
