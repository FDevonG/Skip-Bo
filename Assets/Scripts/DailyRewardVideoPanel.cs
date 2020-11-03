using UnityEngine;
using System;
using System.Collections;

public class DailyRewardVideoPanel : MonoBehaviour
{
    int gemPayout = 10;

    public static DailyRewardVideoPanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void WatchRewardAd()
    {
        StartCoroutine(WatchAd());
    }

    IEnumerator WatchAd()
    {
        CoroutineWithData cd = new CoroutineWithData(this, AdManager.Instance.ShowRewardAdd());
        yield return cd.result;
        RewardVideoCleanUp();
    }

    public void RewardVideoCleanUp()
    {
        GemControl.Instance.AddGems(gemPayout);
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
