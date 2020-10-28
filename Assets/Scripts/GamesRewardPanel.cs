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
        StartCoroutine(AdManager.Instance.ShowRewardAdd());
        yield return new WaitForSeconds(1.0f);
        yield return new WaitUntil(() => !AdManager.Instance.rewardAddPlaying);
        GemControl.Instance.AddGems(gemPayout);
        RewardVideoCleanUp();
    }

    public void RewardVideoCleanUp()
    {
        LocalUser.locUser.rewardCounter = 0;
        Database.UpdateUser("rewardCounter", 0);
        Destroy(gameObject);
    }
}
