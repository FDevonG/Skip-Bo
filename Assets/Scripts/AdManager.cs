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
            Destroy(this.gameObject);
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
        //StartCoroutine(ShowBannerAdd());
    }

    public IEnumerator ShowRegularAd() {
        if (Advertisement.isInitialized) {
            yield return new WaitForSeconds(1.5f);
            while (!Advertisement.IsReady(regularPlacementString)) {
                yield return new WaitForSeconds(0.5f);
            }
            Advertisement.Show(regularPlacementString);
            yield return new WaitForSeconds(1.0f);
            while (Advertisement.isShowing) {
                yield return new WaitForSeconds(0.5f);
            }
        }
        StartCoroutine(GameObject.FindGameObjectWithTag("VictoryPanel").GetComponent<Victory>().ShowStandings());
    }

    public IEnumerator ShowBannerAdd() {
        while (!Advertisement.IsReady(bannerPlacementString)) {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(bannerPlacementString);
    }
}
