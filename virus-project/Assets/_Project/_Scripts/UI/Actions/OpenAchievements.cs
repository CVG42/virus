using UnityEngine;

namespace Virus
{
    [CreateAssetMenu(menuName = "UI/Actions/Open Achievements Button", fileName = "newButton")]
    public class OpenAchievements : UIActionData
    {
        public GameObject achievementsMenu;

        public override void Execute()
        {
            achievementsMenu.SetActive(true);
        }
    }
}
