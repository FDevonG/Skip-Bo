using UnityEngine;
using UnityEngine.UI;

public class AchievementInfoPanel : MonoBehaviour
{
    [SerializeField] Image achievementIcon;
    [SerializeField] Text achievementTitle;
    [SerializeField] Text achievementDescription;

    public void BuildPanel(string image, string title, string description) {
        achievementIcon.sprite = Resources.Load<Sprite>(image) as Sprite;
        achievementTitle.text = title;
        achievementDescription.text = description;
    }
}
