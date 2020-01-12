using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] Image body;
    [SerializeField] Image hair;
    [SerializeField] Image face;
    [SerializeField] Image kit;
    [SerializeField] Text levelText;
    [SerializeField] Slider experienceSlider;
    [SerializeField] Text experienceText;

    private void OnEnable() {
        body.sprite = Resources.Load<Sprite>("Faces/Bodies/" + LocalUser.locUser.body) as Sprite;
        face.sprite = Resources.Load<Sprite>("Faces/Faces/" + LocalUser.locUser.face) as Sprite;
        hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + LocalUser.locUser.hair) as Sprite;
        kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + LocalUser.locUser.kit) as Sprite;
        levelText.text = "Level " + LocalUser.locUser.level.ToString();
        experienceSlider.maxValue = LocalUser.locUser.experienceToNextLevel;
        experienceSlider.value = LocalUser.locUser.experience;
        experienceText.text = LocalUser.locUser.experience.ToString() + "/" + LocalUser.locUser.experienceToNextLevel.ToString() + "xp";
    }
}
