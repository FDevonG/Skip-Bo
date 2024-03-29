using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{

    private string gameId;
    private bool testMode = true;
    private string regularPlacementString = "video";
    private string bannerPlacementString = "banner";
    private string rewardPlacementString = "rewardedVideo";

    public static AdManager Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        if (DeviceType.IsDeviceIos()) {
            gameId = "3313553";
        }
        if (DeviceType.IsDeviceAndroid()) {
            gameId = "3313552";
        }
        Advertisement.Initialize(gameId, testMode);
    }

    public IEnumerator ShowDailyRewardAdd()
    {
        LoadingScreen.Instance.TurnOnLoadingScreen("Loading");
        yield return new WaitUntil(() => Advertisement.IsReady(rewardPlacementString));
        LoadingScreen.Instance.TurnOffLoadingScreen();
        GemControl.Instance.AddGems(GameGlobalSettings.RewardVideoGemAmmount());
        DailyRewardVideoPanel.Instance.RewardVideoCleanUp();
        Advertisement.Show(rewardPlacementString);
        yield return new WaitUntil(() => Advertisement.isShowing);
        yield return new WaitUntil(() => !Advertisement.isShowing);
    }

    public IEnumerator ShowGamesRewardAdd()
    {
        LoadingScreen.Instance.TurnOnLoadingScreen("Loading");
        yield return new WaitUntil(() => Advertisement.IsReady(rewardPlacementString));
        LoadingScreen.Instance.TurnOffLoadingScreen();
        GemControl.Instance.AddGems(GameGlobalSettings.RewardVideoGemAmmount());
        GamesRewardPanel.Instance.RewardVideoCleanUp();
        Advertisement.Show(rewardPlacementString);
        yield return new WaitUntil(() => Advertisement.isShowing);
        yield return new WaitUntil(() => !Advertisement.isShowing);
    }

    public IEnumerator ShowRegularAd() {
        if (LocalUser.locUser.adsBlocked)
            yield return null;
        else
        {
            if (!Advertisement.isInitialized)
            {
                Advertisement.Initialize(gameId, testMode);
                yield return new WaitUntil(() => Advertisement.isInitialized);
            }
            if (Advertisement.isInitialized)
            {
                yield return new WaitUntil(() => Advertisement.IsReady(regularPlacementString));
                Advertisement.Show(regularPlacementString);
                yield return new WaitUntil(() => Advertisement.isShowing);
                yield return new WaitUntil(() => !Advertisement.isShowing);
            }
        }
    }

    public IEnumerator Victory()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ShowRegularAd());
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(GameObject.FindGameObjectWithTag("VictoryPanel").GetComponent<Victory>().ShowStandings());
    }

    public void LeaveMatchAd()
    {
        SceneController.LoadStartMenu();
        if (!LocalUser.locUser.adsBlocked) {
            StartCoroutine(ShowRegularAd());
        }        
    }

    public IEnumerator ShowBannerAdd() {
        if(!Advertisement.IsReady(bannerPlacementString))
            yield return new WaitUntil(() => Advertisement.IsReady(bannerPlacementString));

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(bannerPlacementString);
    }

    public void TurnOnBannerAd()
    {
        if (LocalUser.locUser.adsBlocked)
            return;

        Advertisement.Banner.Show(bannerPlacementString);
    }

    public void TurnOffBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    public bool IsAdPlaying()
    {
        if (Advertisement.isShowing)
            return true;
        else
            return false;
    }
}
