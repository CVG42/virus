using UnityEngine;

namespace Virus
{
    [CreateAssetMenu(fileName = "Achievement", menuName = "Achievements/Achievement")]
    public class AchievementAttributes : ScriptableObject
    {
        public string id;
        public string title;
        [TextArea] public string description;
        public Sprite icon;
        public bool unlockByDefault = false;
    }
}
