using UnityEngine;

public class GameOptions : MonoBehaviour
{
    public GameObject connectingPanel;

    public void OnlineGame() {
        //GetComponent<ActivatePanel>().SwitchPanel(connectingPanel);
        if (!PhotonNetwork.connected) {
            PhotonNetworking.Instance.ConnectToPhoton();
        }
        SceneController.LoadGameSetup();
    }

    public void OfflineGame() {
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = true;
    }

    public void CreateOfflineGame(int deckAmmount) {
        PhotonRooms.Instance.CreateOfflineRoom(deckAmmount);
    }
}
