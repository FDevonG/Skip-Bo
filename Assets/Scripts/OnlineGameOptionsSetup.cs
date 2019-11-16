using UnityEngine;

public class OnlineGameOptionsSetup : MonoBehaviour
{

    public GameObject noGamesPanel;//this is used when there are no rooms to join with the ranom button
    public GameObject roomNameExistsPanel;

    public GameObject mainPanel;
    public GameObject roomCreationPanel;
    public GameObject joinRoomPanel;
    public GameObject roomBrowserPanel;
    public GameObject gameFiltersPanel;

    ActivatePanel activatePanel;

    private void Start() {
        activatePanel = GetComponent<ActivatePanel>();
    }

    public void JoinRandomRoom() {
        if (PhotonNetwork.GetRoomList().Length != 0) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<PhotonRooms>().JoinRandomRoom();
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(noGamesPanel);
        }
    }

    public void LoadStartMenu() {
        PhotonNetwork.Disconnect();
        SceneController.LoadStartMenu();
    }

    private void GoBackAPanel() {
        if (activatePanel.activePanel == mainPanel) {
            PhotonNetwork.Disconnect();
            SceneController.LoadStartMenu();
            return;
        }
        if (activatePanel.activePanel == roomCreationPanel) {
            activatePanel.SwitchPanel(mainPanel);
            return;
        }
        if (activatePanel.activePanel == joinRoomPanel) {
            activatePanel.SwitchPanel(mainPanel);
            return;
        }
        if (activatePanel.activePanel == roomBrowserPanel) {
            activatePanel.SwitchPanel(mainPanel);
            return;
        }
        if (activatePanel.activePanel == gameFiltersPanel) {
            activatePanel.SwitchPanel(roomBrowserPanel);
            return;
        }
        if (activatePanel.activePanel == noGamesPanel) {
            activatePanel.SwitchPanel(mainPanel);
            return;
        }
        if (activatePanel.activePanel == roomNameExistsPanel) {
            activatePanel.SwitchPanel(roomCreationPanel);
            return;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GoBackAPanel();
        }
    }
}
