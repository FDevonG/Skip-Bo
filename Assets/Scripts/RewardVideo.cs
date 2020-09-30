using System;
using UnityEngine;

public class RewardVideo : MonoBehaviour
{
    public static RewardVideo Instance { get; private set; }

    int rewardCounterCap = 10;

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
        DontDestroyOnLoad(gameObject);
    }

    public bool DailyRewardCanPlay()
    {
        if (!LocalUser.locUser.dailyRewardGotten)
        {
            return true;
        }
        else
        {
            DateTime nextReward = DateTime.FromBinary(Convert.ToInt64(LocalUser.locUser.nextRewardUnlock));
            if (DateTime.Now >= nextReward)
            {
                LocalUser.locUser.dailyRewardGotten = false;
                Database.UpdateUser("dailyRewardGotten", false);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void GamesPlayedRewardCounter()
    {
        LocalUser.locUser.rewardCounter++;
        Database.UpdateUser("rewardCounter", LocalUser.locUser.rewardCounter);
        if (LocalUser.locUser.rewardCounter >= rewardCounterCap)
        {
            Instantiate(Resources.Load<GameObject>("GamesRewardVideo") as GameObject);
        }
    }

}
