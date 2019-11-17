using UnityEngine;

public class GameOptions : MonoBehaviour
{
    public GameObject connectingPanel;

    public void OnlineGame() {
        //GetComponent<ActivatePanel>().SwitchPanel(connectingPanel);
        if (!PhotonNetwork.connected) {
            StartCoroutine(GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton());
        }
        SceneController.LoadGameSetup();
    }

    public void OfflineGame() {
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = true;
    }

    public void CreateOfflineGame(int deckAmmount) {
        StartCoroutine(GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonRooms>().CreateOfflineRoom(deckAmmount));
    }
}
