using UnityEngine;

public class LobbyPrep : MonoBehaviour
{
    public GameObject connectingPanel;
    public GameObject gameSetupPanel;
    public static string deckAmmount = "DeckSize";
    byte maxPlayers = 4;

    //this creates a photon room in the event that there are no rooms made already
    private void CreateRoom(int deckSize) {

        RoomOptions roomOptions = new RoomOptions();//create a new room options
        roomOptions.MaxPlayers = maxPlayers;//the max number of players in the game is 4
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();//create a new custom room properties
        roomOptions.CustomRoomProperties[deckAmmount] = deckSize;
        roomOptions.CustomRoomPropertiesForLobby = new string[] {
            deckAmmount,
        };

        PhotonNetwork.CreateRoom(null, roomOptions, null);

        if (PhotonNetwork.offlineMode) {
            SceneController.LoadGameScene();
        } 
    }

    //this function attmepts to join a room and if none match the selected deck size then it will create a room
    public void JoinRoom(int deckSize) {
        foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
            int deck = (int)game.CustomProperties[deckAmmount];
            if (deck == deckSize) {
                if (game.IsOpen) {
                    PhotonNetwork.JoinRoom(game.Name);
                    return;
                }
            }
        }
        CreateRoom(deckSize);
    }

    public void LobbySetUp(int deckAmmount) {
        PlayerData data = SaveSystem.LoadPlayer();
        Player player = new Player(data.name, data.hair, data.face, data.kit, data.body);
        Debug.Log(player.Name);
        PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, player);
        if (PhotonNetwork.offlineMode) {
            CreateRoom(deckAmmount);
        } else {
            JoinRoom(deckAmmount);
        }
    }

    public void OnlineGame() {
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton();
        PhotonNetwork.offlineMode = false;
        GetComponent<Menu>().SwitchPanel(connectingPanel);
    }

    public void OfflineGame() {
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = true;
    }
}
