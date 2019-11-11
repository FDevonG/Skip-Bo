﻿using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject logInPanel;
    public GameObject startMenu;
    public GameObject characterCreationPanel;
    public GameObject howToPlayPanel;
    public GameObject settingsPanel;
    public GameObject gameSetupPanel;
    public GameObject statsPanel;
    public GameObject failedToConnectPanel;
    public GameObject quitGamePanel;
    public GameObject failedToLogInPanel;

    private ActivatePanel activatePanel;

    private void Awake() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;//keep the screen from fading
        Application.runInBackground = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        activatePanel = GetComponent<ActivatePanel>();
        DoesPlayerExist();
    }

    //this is called when you fail to connect to photon
    private void OnFailedToConnectToPhoton() {
        activatePanel.SwitchPanel(failedToConnectPanel);
    }

    private void GoBackAPanel() {
        if (activatePanel.activePanel == logInPanel) {
            activatePanel.SwitchPanel(quitGamePanel);
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
    }

    private void DoesPlayerExist() {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data == null) {
            activatePanel.SwitchPanel(logInPanel);
        } else {
            activatePanel.SwitchPanel(startMenu);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoBackAPanel();
        }
    }
}
