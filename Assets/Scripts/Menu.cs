﻿using System.Collections;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject logInPanel;
    public GameObject signUpPanel;
    public GameObject startMenu;
    public GameObject playGamePanel;
    public GameObject characterCreationPanel;
    public GameObject howToPlayPanel;
    public GameObject settingsPanel;
    public GameObject gameSetupPanel;
    public GameObject statsPanel;
    public GameObject failedToConnectPanel;
    public GameObject quitGamePanel;
    public GameObject failedToLogInPanel;
    public GameObject friendsPanel;
    public GameObject startGamePanel;
    public GameObject forgotPasswordPanel;
    public GameObject findFriendsPanel;
    public GameObject friendsSettingsPanel;
    public GameObject blockedPanel;
    public GameObject leaderboardPanel;
    public GameObject ratingPanel;
    public GameObject playerPanel;
    public GameObject achievementsPanel;
    public GameObject removeAdsPanel;
    public GameObject storePanel;
    public GameObject dailyRewardVideoPanel;


    public GameObject errorPanel;

    private ActivatePanel activatePanel;

    private void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;//keep the screen from fading
        Application.runInBackground = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        activatePanel = GetComponent<ActivatePanel>();
        StartCoroutine(DoesPlayerExist());
    }

    //this is called when you fail to connect to photon
    private void OnFailedToConnectToPhoton() {
        activatePanel.SwitchPanel(failedToConnectPanel);
    }

    private void GoBackAPanel() {
        if (activatePanel.activePanel == startGamePanel) {
            activatePanel.SwitchPanel(quitGamePanel);
            return;
        }
        if (activatePanel.activePanel == logInPanel) {
            activatePanel.SwitchPanel(startGamePanel);
            return;
        }
        if (activatePanel.activePanel == forgotPasswordPanel) {
            activatePanel.SwitchPanel(logInPanel);
            return;
        }
        if (activatePanel.activePanel == signUpPanel) {
            activatePanel.SwitchPanel(startGamePanel);
            return;
        }
        if (activatePanel.activePanel == startMenu) {
            activatePanel.SwitchPanel(quitGamePanel);
            return;
        }
        if (activatePanel.activePanel == quitGamePanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == characterCreationPanel) {
            StartCoroutine(DoesPlayerExist());
            return;
        }
        if (activatePanel.activePanel == howToPlayPanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == settingsPanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == gameSetupPanel) {
            activatePanel.SwitchPanel(playGamePanel);
            return;
        }
        if (activatePanel.activePanel == failedToConnectPanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == statsPanel) {
            activatePanel.SwitchPanel(playerPanel);
            return;
        }
        if (activatePanel.activePanel == achievementsPanel) {
            activatePanel.SwitchPanel(playerPanel);
            return;
        }
        if (activatePanel.activePanel == failedToLogInPanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == friendsPanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == findFriendsPanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == friendsSettingsPanel) {
            activatePanel.SwitchPanel(friendsPanel);
            return;
        }
        if (activatePanel.activePanel == blockedPanel) {
            activatePanel.SwitchPanel(friendsPanel);
            return;
        }
        if (activatePanel.activePanel == leaderboardPanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == playGamePanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == playerPanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == dailyRewardVideoPanel)
        {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == playerPanel)
        {
            activatePanel.SwitchPanel(storePanel);
            return;
        }
        if (activatePanel.activePanel == removeAdsPanel)
        {
            activatePanel.PreviousPanel();
            return;
        }
    }

    public IEnumerator DoesPlayerExist() {
        if (!FirebaseAuthentication.IsPlayerLoggedIn() && !FacebookScript.Instance.IsFacebookLoggedIn()) {
            StartCoroutine(AdManager.Instance.ShowBannerAdd());
            activatePanel.SwitchPanel(startGamePanel);
        } else {
            LoadingScreen.Instance.TurnOnLoadingScreen("Loading");
            if (LocalUser.locUser == null) {
                var task = Database.GetCurrentUser();
                yield return new WaitUntil(() => task.IsCompleted);
                if (task.IsFaulted) {
                    StartCoroutine(AdManager.Instance.ShowBannerAdd());
                    activatePanel.SwitchPanel(errorPanel);
                    LoadingScreen.Instance.TurnOffLoadingScreen();
                } else {
                    LocalUser.locUser = JsonUtility.FromJson<User>(task.Result);

                    if (LocalUser.locUser.achievments.Count == 0) {
                        LocalUser.locUser.achievments = Achievments.Instance.BuildAchievmentsList();
                        StartCoroutine(Achievments.Instance.SaveAchievments());
                    }
                    if (LevelSystem.GetExperienceToNextLevel() == 0) {
                        LocalUser.locUser.experienceToNextLevel = 100;
                        var experienceNeededTask = Database.UpdateUser("experienceToNextLevel", LocalUser.locUser.experienceToNextLevel);
                        yield return new WaitUntil(() => experienceNeededTask.IsCompleted);
                        while (experienceNeededTask.IsFaulted) {
                            experienceNeededTask = Database.UpdateUser("experienceToNextLevel", LocalUser.locUser.experienceToNextLevel);
                            yield return new WaitUntil(() => experienceNeededTask.IsCompleted);
                        }
                    }
                    if (LevelSystem.GetLevelNumber() == 0) {
                        LocalUser.locUser.level = 1;
                        var levelTask = Database.UpdateUser("level", LocalUser.locUser.level);
                        yield return new WaitUntil(() => levelTask.IsCompleted);
                        while (levelTask.IsFaulted) {
                            levelTask = Database.UpdateUser("level", LocalUser.locUser.level);
                            yield return new WaitUntil(() => levelTask.IsCompleted);
                        }
                    }
                }
            }

            if (LocalUser.locUser != null)
            {
                if (string.IsNullOrEmpty(LocalUser.locUser.userName) || string.IsNullOrWhiteSpace(LocalUser.locUser.userName))
                {
                    activatePanel.SwitchPanel(characterCreationPanel);
                }
                else
                {
                    PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, LocalUser.locUser);
                    PhotonNetworking.Instance.ConnectToPhoton();
                    if (RewardVideo.Instance.DailyRewardCanPlay())
                    {
                        activatePanel.SwitchPanel(dailyRewardVideoPanel);
                    }
                    else if (!Rating.CheckRated())
                    {
                        if (Rating.CheckGamesPlayed())
                        {
                            activatePanel.SwitchPanel(ratingPanel);
                        }
                        else
                        {
                            activatePanel.SwitchPanel(startMenu);
                        }
                    }
                    else
                    {
                        activatePanel.SwitchPanel(startMenu);
                    }
                }
            }
        }

        LoadingScreen.Instance.TurnOffLoadingScreen();

        if (!Announcer.Instance.welcomePlayed) {
            Announcer.Instance.Welcome();
            Announcer.Instance.welcomePlayed = true;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (LoadingScreen.Instance.loading)
                return;
            GoBackAPanel();
        }
    }
}
