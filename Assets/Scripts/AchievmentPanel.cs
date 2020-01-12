using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievmentPanel : MonoBehaviour
{
    [SerializeField] Transform achievmentSpawnParent;
    List<GameObject> achievmentPanels = new List<GameObject>();

    private void OnEnable() {
        BuildAchievments();
    }

    private void OnDisable() {
        DestroyPanelContents();
    }

    private void BuildAchievments() {
        List<Achievment> unlocked = new List<Achievment>();
        List<Achievment> locked = new List<Achievment>();
        //foreach(Achievment achievment in LocalUser.locUser.achievments) {
        //    if (achievment.unlocked) {
        //        unlocked.Add(achievment);
        //    } else {
        //        locked.Add(achievment);
        //    }
        //}
    }

    private void DestroyPanelContents() {
        foreach(GameObject child in achievmentPanels) {
            Destroy(child);
        }
        achievmentPanels.Clear();
    }
}
