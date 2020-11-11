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
        StartCoroutine(AdManager.Instance.ShowGamesRewardAdd());
        while (!AdManager.Instance.IsAdPlaying())
        {
            yield return new WaitForSeconds(0.1f);
        }
        while (AdManager.Instance.IsAdPlaying())
        {
            yield return new WaitForSeconds(0.1f);
        }
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
