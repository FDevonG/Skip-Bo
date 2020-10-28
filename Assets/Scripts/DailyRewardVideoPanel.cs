﻿using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;

public class DailyRewardVideoPanel : MonoBehaviour
{
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    int gemPayout = 10;

    public void WatchRewardAd()
    {
        StartCoroutine(WatchAd());
    }

    IEnumerator WatchAd()
    {
        StartCoroutine(AdManager.Instance.ShowRewardAdd());
        yield return new WaitForSeconds(1.0f);
        yield return new WaitUntil(() => !AdManager.Instance.rewardAddPlaying);
        GemControl.Instance.AddGems(gemPayout);
        RewardVideoCleanUp();
    }

    public void RewardVideoCleanUp()
    {

        LocalUser.locUser.dailyRewardGotten = true;
        Database.UpdateUser("dailyRewardGotten", true);

        DateTime currentDate = DateTime.Now;
        Debug.Log(currentDate.Hour);

        DateTime nextUnockDate = DateTime.Today;
        nextUnockDate.AddHours(19);

        if (currentDate.Hour >= 19)
        {
            Database.UpdateUser("nextRewardUnlock", nextUnockDate.AddDays(1).AddHours(19).ToBinary().ToString());
        }
        else
        {
            Database.UpdateUser("nextRewardUnlock", nextUnockDate.AddHours(19).ToBinary().ToString());
        }
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
    }

}
