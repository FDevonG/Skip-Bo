using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievmentPanel : MonoBehaviour
{
    [SerializeField] Transform achievmentSpawnParent;
    [SerializeField] Dropdown filterDropDown;
    List<GameObject> achievmentPanels = new List<GameObject>();

    private void OnEnable() {
        BuildAchievments();
    }

    private void OnDisable() {
        DestroyPanelContents();
    }

    public void BuildAchievments() {
        DestroyPanelContents();
        List<Achievment> unlocked = new List<Achievment>();
        List<Achievment> locked = new List<Achievment>();
        foreach (Achievment achievment in LocalUser.locUser.achievments) {
            if (achievment.unlocked) {
                unlocked.Add(achievment);
            } else {
                locked.Add(achievment);
            }
        }
        if (filterDropDown.value == 0) {
            SpawnAchievements(unlocked);
            SpawnAchievements(locked);
        }
        if (filterDropDown.value == 1) {
            SpawnAchievements(unlocked);
        }
        if (filterDropDown.value == 2) {
            SpawnAchievements(locked);
        }
    }

    private void SpawnAchievements(List<Achievment> achievements) {
        foreach (Achievment achievement in achievements) {
            GameObject achievementpanel = Instantiate(Resources.Load<GameObject>("AchievementPanel"), achievmentSpawnParent);
            achievmentPanels.Add(achievementpanel);
            achievementpanel.transform.localScale = new Vector3(1,1,1);
            string imagaePath = "";
            if (achievement.unlocked) {
                imagaePath = achievement.iconName;
            } else {
                imagaePath = achievement.iconNameGreyscale;
            }
            achievementpanel.GetComponent<AchievementInfoPanel>().BuildPanel(imagaePath, achievement.name, achievement.description, achievement.unlocked);
        }
    }

    private void DestroyPanelContents() {
        foreach(GameObject child in achievmentPanels) {
            Destroy(child);
        }
        achievmentPanels.Clear();
    }
}
