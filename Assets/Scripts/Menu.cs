using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    GameObject activePanel;

    public GameObject logInPanel;
    public GameObject startMenu;
    public GameObject characterCreationPanel;
    public GameObject howToPlayPanel;
    public GameObject settingsPanel;
    public GameObject gameSetupPanel;
    public GameObject statsPanel;
    public GameObject failedToConnectPanel;
    public GameObject quitGamePanel;

    private void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;//keep the screen from fading
        Application.runInBackground = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        DoesPlayerExist();
    }

    public void SwitchPanel(GameObject panel) {
        if (activePanel != null) {
            activePanel.SetActive(false);
        }
        panel.SetActive(true);
        activePanel = panel;
    }

    //this is called when you fail to connect to photon
    private void OnFailedToConnectToPhoton() {
        SwitchPanel(failedToConnectPanel);
    }

    private void GoBackAPanel() {
        if (activePanel == logInPanel) {
            SwitchPanel(quitGamePanel);
            return;
        }
        if (activePanel == startMenu) {
            SwitchPanel(quitGamePanel);
            return;
        }
        if (activePanel == quitGamePanel) {
            SwitchPanel(startMenu);
            return;
        }
        if (activePanel == characterCreationPanel) {
            DoesPlayerExist();
            return;
        }
        if (activePanel == howToPlayPanel) {
            SwitchPanel(startMenu);
            return;
        }
        if (activePanel == settingsPanel) {
            SwitchPanel(startMenu);
            return;
        }
        if (activePanel == gameSetupPanel) {
            SwitchPanel(startMenu);
            return;
        }
        if (activePanel == failedToConnectPanel) {
            SwitchPanel(startMenu);
            return;
        }
        if (activePanel == statsPanel) {
            SwitchPanel(startMenu);
            return;
        }
    }

    private void DoesPlayerExist() {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data == null) {
            SwitchPanel(logInPanel);
        } else {
            SwitchPanel(startMenu);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoBackAPanel();
        }
    }
}
