using UnityEngine;

public class GameOptions : MonoBehaviour
{
    public GameObject connectingPanel;

    public void OnlineGame() {
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton();
        PhotonNetwork.offlineMode = false;
        GetComponent<ActivatePanel>().SwitchPanel(connectingPanel);
    }

    public void OfflineGame() {
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = true;
    }

    public void CreateOfflineGame(int deckAmmount) {
        PhotonRooms.CreateOfflineRoom(deckAmmount);
    }
}
