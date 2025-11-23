using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Virus
{
    public class AchievementItemUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private CanvasGroup _canvasGroup;

        public void SetData(AchievementAttributes achievement, bool unlocked)
        {
            if (unlocked)
            {
                _titleText.text = achievement.title;
                _descriptionText.text = achievement.description;
                _canvasGroup.alpha = 1f;
            }
            else
            {
                _titleText.text = "????";
                _descriptionText.text = "????";

                _canvasGroup.alpha = 0.4f;
            }
        }
    }
}
