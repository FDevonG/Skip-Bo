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

    public IEnumerator SetupPhotonPlayer() {
        var task = FireBaseScript.GetCurrentUser();
        yield return new WaitUntil(() => task.IsCompleted);
        User user = new User();
        if (!task.IsFaulted) {
            user = JsonUtility.FromJson<User>(task.Result);
            PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, user);
        } else {
            Debug.Log("");
        }
    }

    public IEnumerator JoinRandomRoom() {
        yield return StartCoroutine(SetupPhotonPlayer());
        PhotonNetwork.JoinRandomRoom();
    }

    public IEnumerator CreateOfflineRoom(int deckAmmount) {
        yield return StartCoroutine(SetupPhotonPlayer());
        PhotonNetwork.CreateRoom(null, GetRoomOptions(deckAmmount), null);
        SceneController.LoadGameScene();
    }

    public IEnumerator CreateOnlineGame(string roomName, int deckAmmount) {
        if (!RoomNameExistCheck(roomName)) {
            yield return StartCoroutine(SetupPhotonPlayer());
            RoomOptions roomOptions = GetRoomOptions(deckAmmount);
            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<OnlineGameOptionsSetup>().roomNameExistsPanel);
        }
    }

    public IEnumerator JoinRoom(string name) {
        yield return StartCoroutine(SetupPhotonPlayer());
        PhotonNetwork.JoinRoom(name);
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
