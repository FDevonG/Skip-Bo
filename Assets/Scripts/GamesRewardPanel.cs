using System.Collections;
using UnityEngine;

public class GamesRewardPanel : MonoBehaviour
{

    int gemPayout = 10;
    public static GamesRewardPanel Instance { get; private set; }

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
        LocalUser.locUser.rewardCounter = 0;
        Database.UpdateUser("rewardCounter", 0);
        Destroy(gameObject);
    }
}
