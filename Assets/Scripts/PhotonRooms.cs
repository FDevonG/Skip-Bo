using UnityEngine;

public static class PhotonRooms
{
    public static void SetupPhotonPlayer() {
        PlayerData data = SaveSystem.LoadPlayer();
        Player player = new Player(data.name, data.hair, data.face, data.kit, data.body, data.uniqueID);
        PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, player);
    }

    public static void JoinRandomRoom() {
        SetupPhotonPlayer();
        PhotonNetwork.JoinRandomRoom();
    }

    public static void CreateOfflineRoom(int deckAmmount) {
            SetupPhotonPlayer();
            PhotonNetwork.CreateRoom(null, GetRoomOptions(deckAmmount), null);
            SceneController.LoadGameScene();
    }

    public static void CreateOnlineGame(string roomName, int deckAmmount) {
        if (!RoomNameExistCheck(roomName)) {
            SetupPhotonPlayer();
            RoomOptions roomOptions = GetRoomOptions(deckAmmount);
            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<OnlineGameOptionsSetup>().roomNameExistsPanel);
        }
    }

    public static string DeckSize() {
        return "DeckSize";
    }

    public static byte MaxPlayers() {
        return 4;
    }

    private static RoomOptions GetRoomOptions(int deckAmmount) {
        RoomOptions roomOptions = new RoomOptions();//create a new room options
        roomOptions.MaxPlayers = MaxPlayers();//the max number of players in the game is 4
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();//create a new custom room properties
        roomOptions.CustomRoomProperties[DeckSize()] = deckAmmount;
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
