using UnityEngine;

public class OnlineGameOptionsSetup : MonoBehaviour
{

    public GameObject noGamesPanel;//this is used when there are no rooms to join with the ranom button
    public GameObject roomNameExistsPanel;

    public void JoinRandomRoom() {
        if (PhotonNetwork.GetRoomList().Length != 0) {
            PhotonRooms.JoinRandomRoom();
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(noGamesPanel);
        }
    }

    public void LoadStartMenu() {
        PhotonNetwork.Disconnect();
        SceneController.LoadStartMenu();
    }
}
