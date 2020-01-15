using System.Collections;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject logInPanel;
    public GameObject signUpPanel;
    public GameObject startMenu;
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
    public GameObject loadingScreen;
    public GameObject ratingPanel;

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
            DoesPlayerExist();
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
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == failedToConnectPanel) {
            activatePanel.SwitchPanel(startMenu);
            return;
        }
        if (activatePanel.activePanel == statsPanel) {
            activatePanel.SwitchPanel(startMenu);
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
            activatePanel.SwitchPanel(startGamePanel);
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
            activatePanel.SwitchPanel(statsPanel);
            return;
        }
    }

    private IEnumerator DoesPlayerExist() {
        if (!FireBaseScript.IsPlayerLoggedIn()) {
            //canvas.gameObject.SetActive(true);
            activatePanel.SwitchPanel(startGamePanel);
        } else {
            if (LocalUser.locUser == null) {
                activatePanel.SwitchPanel(loadingScreen);
                yield return StartCoroutine(LocalUser.LoadUser());
            }
            if (LocalUser.locUser.friends.Count > 0) {
                StartCoroutine(Friends.GetStartFriends());
            }
            //canvas.gameObject.SetActive(true);
            if (string.IsNullOrEmpty(LocalUser.locUser.userName)) {
                activatePanel.SwitchPanel(characterCreationPanel);
            } else {
                PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, LocalUser.locUser);
                GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton();
                if (!Rating.CheckRated()) {
                    if (Rating.CheckGamesPlayed()) {
                        activatePanel.SwitchPanel(ratingPanel);
                    } else {
                        activatePanel.SwitchPanel(startMenu);
                    }
                } else {
                    activatePanel.SwitchPanel(startMenu);
                }
            }
        }
        if (!GameObject.FindGameObjectWithTag("Announcer").GetComponent<Announcer>().welcomePlayed) {
            GameObject.FindGameObjectWithTag("Announcer").GetComponent<Announcer>().Welcome();
            GameObject.FindGameObjectWithTag("Announcer").GetComponent<Announcer>().welcomePlayed = true;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoBackAPanel();
        }
    }
}
