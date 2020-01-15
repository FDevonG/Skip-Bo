using UnityEngine;
using System.Collections;

public class PhotonRooms : MonoBehaviour
{

    public static PhotonRooms Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void JoinRandomRoom() {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateOfflineRoom(int deckAmmount) {
        PhotonNetwork.CreateRoom(null, GetRoomOptions(deckAmmount, 4, true), null);
        SceneController.LoadGameScene();
    }

    public void CreateOnlineGame(string roomName, int deckAmmount, byte maxPlayers, bool priv) {
        if (!RoomNameExistCheck(roomName)) {
            RoomOptions roomOptions = GetRoomOptions(deckAmmount, maxPlayers, priv);
            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<OnlineGameOptionsSetup>().roomNameExistsPanel);
        }
    }

    public static string DeckSize() {
        return "DeckSize";
    }

    public static string PrivateRoom() {
        return "Private";
    }

    private static RoomOptions GetRoomOptions(int deckAmmount, byte maxPlayers, bool priv) {
        RoomOptions roomOptions = new RoomOptions();//create a new room options
        roomOptions.MaxPlayers = maxPlayers;//the max number of players in the game is 4
        roomOptions.IsVisible = priv;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();//create a new custom room properties
        roomOptions.CustomRoomProperties[DeckSize()] = deckAmmount;
        roomOptions.CustomRoomProperties[PrivateRoom()] = priv;
        roomOptions.PublishUserId = true;
        roomOptions.CustomRoomPropertiesForLobby = new string[] {
            DeckSize(),
        };
        return roomOptions;
    }

    private static bool RoomNameExistCheck(string roomName) {
        bool roomExists = false;
        foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
            if (game.Name == roomName) {
                roomExists = true;
            }
        }
        return roomExists;
    }

}
