using UnityEngine;
using UnityEngine.UI;

public class AchievementInfoPanel : MonoBehaviour
{
    [SerializeField] Image achievementIcon;
    [SerializeField] Text achievementTitle;
    [SerializeField] Text achievementDescription;

    public void BuildPanel(string image, string title, string description, bool unlocked) {
        achievementIcon.sprite = Resources.Load<Sprite>(image) as Sprite;
        if(unlocked)
        {
            achievementTitle.color = Colors.GetBaseColor();
            achievementDescription.color = Color.white;
        }
        else
        {
            achievementTitle.color = Colors.GetGreyTextColor();
            achievementDescription.color = Colors.GetGreyTextColor();
        }
        achievementTitle.text = title;
        achievementDescription.text = description;
    }
}
