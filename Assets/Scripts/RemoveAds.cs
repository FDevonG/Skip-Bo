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
    [SerializeField] Text price;

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
        //AdsCheck();
        adsButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(adsPanel);
            adsButton.SetActive(false);
        });
        noButton.GetComponent<Button>().onClick.AddListener(() => adsButton.SetActive(true));
        noButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().PreviousPanel();
            GetComponent<ErrorText>().ClearError();
        });
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 3 && scene.buildIndex != 2)
        {
            AdsCheck();
        }
        else
        {
            adsButton.SetActive(false);
        }
    }

    public void HideButton(){
        adsButton.SetActive(false);
    }

    public void AdsCheck()
    {
        if (!LocalUser.locUser.adsBlocked)
        {
            adsButton.SetActive(true);
            if (FirebaseAuthentication.IsPlayerAnonymous())
            {
                adsButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                adsButton.GetComponent<Button>().interactable = true;
            }
            StartCoroutine(GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>().ShowBannerAdd());
        }
        else
        {
            adsButton.SetActive(false);
            GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>().TurnOffBannerAd();
        }
    }

    public void AdsRemoved()
    {
        LocalUser.locUser.adsBlocked = true;
        var task = Database.UpdateUser("adsBlocked", LocalUser.locUser.adsBlocked);
        //yield return new WaitUntil(() => task.IsCompleted);
        text.text = "Ads have been removed";
        price.gameObject.SetActive(false);
        noButton.GetComponent<Button>().onClick.AddListener(() => adsButton.SetActive(false));
        noButton.GetComponentInChildren<Text>().text = "Close";
        GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>().TurnOffBannerAd();
        yesButton.SetActive(false);
        GetComponent<ErrorText>().ClearError();
    }
}
