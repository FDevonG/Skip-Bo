using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour
{

    private string gameId;
    private bool testMode = true;
    private string regularPlacementString = "video";
    private string bannerPlacementString = "banner";

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

    private void Start() {
        StartCoroutine(ShowBannerAdd());
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
        yield return StartCoroutine(ShowRegularAd());
        StartCoroutine(GameObject.FindGameObjectWithTag("VictoryPanel").GetComponent<Victory>().ShowStandings());
    }

    //public IEnumerator PlayAgainAd()
    //{
    //    yield return StartCoroutine(ShowRegularAd());

    //    if (PhotonNetwork.offlineMode)
    //    {
    //        SceneController.ReloadScene();
    //    }
    //    else if (!PhotonNetwork.offlineMode)
    //    {
    //        SceneController.LoadGameSetup();
    //    }
    //}

    public IEnumerator LeaveMatchAd()
    {
        if (!LocalUser.locUser.adsBlocked) {
            SceneController.LoadingScreen();
            yield return StartCoroutine(ShowRegularAd());
        }
        SceneController.LoadStartMenu();
    }

    public IEnumerator ShowBannerAdd() {
        yield return new WaitUntil(() => LocalUser.locUser != null);
        if (!LocalUser.locUser.adsBlocked)
        {
            yield return new WaitUntil(() => Advertisement.IsReady(bannerPlacementString));
            //while (!Advertisement.IsReady(bannerPlacementString)) {
            //    yield return new WaitForSeconds(0.5f);
            //}
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(bannerPlacementString);
        }
        else
        {
            TurnOffBannerAd();
        }
    }

    public void TurnOffBannerAd()
    {
        Advertisement.Banner.Hide();
    }
}
