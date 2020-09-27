﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public LoadingScreen Instance { get; private set; }

    [SerializeField] GameObject mainPanel;
    [SerializeField] Text loadingText;

    bool loading = false;

    bool textLerp = false;

    private float timeTakenDuringLerp = 1.0f;
    private float timeStartedLerping;

    RemoveAds removeAds;

    void Awake()
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

    private void Start()
    {
        removeAds = GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<RemoveAds>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TurnOffLoadingScreen();
    }

    // Update is called once per frame
    void Update()
    {


        if (loading)
        {

            if (textLerp)
            {
                float timeSinceStarted = Time.time - timeStartedLerping;
                float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                loadingText.color = Color.Lerp(new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 1), new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 0), percentageComplete);
                if (percentageComplete >= timeTakenDuringLerp)
                {
                    timeStartedLerping = Time.time;
                    textLerp = false;
                }
            }

            if (!textLerp)
            {
                float timeSinceStarted = Time.time - timeStartedLerping;
                float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                loadingText.color = Color.Lerp(new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 0), new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 1), percentageComplete);
                if (percentageComplete >= timeTakenDuringLerp)
                {
                    timeStartedLerping = Time.time;
                    textLerp = true;
                }
            }
        }
    }


    public void TurnOnLoadingScreen()
    {
        removeAds.HideButton();
        mainPanel.SetActive(true);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        loading = true;
        textLerp = true;
        timeStartedLerping = Time.time;
    }

    public void TurnOffLoadingScreen()
    {
        removeAds.AdsCheck();
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, 1);
        loading = false;
        mainPanel.SetActive(false);
    }
}