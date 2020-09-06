using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RemoveAds : MonoBehaviour
{
    [SerializeField] GameObject adsButton;
    [SerializeField] GameObject adsPanel;
    [SerializeField] GameObject yesButton;
    [SerializeField] GameObject noButton;
    [SerializeField] Text text;

    public static RemoveAds instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(AdsCheck());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 3 && scene.buildIndex != 2)
        {
            StartCoroutine(AdsCheck());
        }
        else
        {
            adsButton.SetActive(false);
        }
    }

    IEnumerator AdsCheck()
    {
        yield return new WaitUntil(() => LocalUser.locUser != null);
        if (!LocalUser.locUser.adsBlocked)
        {
            adsButton.SetActive(true);
            adsButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(adsPanel);
                adsButton.SetActive(false);
            });
            noButton.GetComponent<Button>().onClick.AddListener(() => adsButton.SetActive(true));
            noButton.GetComponent<Button>().onClick.AddListener(() => GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().PreviousPanel());
        }
    }

    public void AdsRemoved()
    {
        text.text = "Ads have been removed";
        noButton.GetComponent<Button>().onClick.AddListener(() => adsButton.SetActive(false));
        noButton.GetComponentInChildren<Text>().text = "Close";
        GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>().TurnOffBannerAd();
        yesButton.SetActive(false);
        LocalUser.locUser.adsBlocked = true;
    }
}
