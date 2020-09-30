using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonNetworking : MonoBehaviour {

    public static PhotonNetworking Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 0) {
            ConnectToPhoton();
        }
    }

    public void ConnectToPhoton() {
        if (!PhotonNetwork.connected) {
            PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, LocalUser.locUser);
            PhotonNetwork.ConnectUsingSettings(GameGlobalSettings.Version());
        }
    }

    private void OnConnectedToMaster() {
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.autoCleanUpPlayerObjects = true;
        Chat.Instance.ConnectToChat();
        if (LocalUser.locUser.friends.Count > 0) {
            PhotonNetwork.FindFriends(LocalUser.locUser.friends.ToArray());
        }
    }

    //this is called when you fail to connect to photon
    //private void OnFailedToConnectToPhoton() {
    //    Debug.Log("Failed to connect to photon");
    //}

    //private void OnJoinedLobby() {
    //    SceneController.LoadGameSetup();
    //}

    //this is called when you get disconnected from photon
    private void OnDisconnectedFromPhoton() {
        Debug.Log("Disconnected from photon");
    }

    private void OnCreatedRoom() {
        if (!PhotonNetwork.offlineMode) {
            SceneController.LoadLobbyScene();
            Chat.Instance.SubcsribeToChannel(PhotonNetwork.room.Name);
        }
    }

    //this method is called when you join a room automatically
    private void OnJoinedRoom() {
        SceneController.LoadLobbyScene();
        if (!PhotonNetwork.isMasterClient) {
            Chat.Instance.SubcsribeToChannel(PhotonNetwork.room.Name);
        }
    }

    private void OnPhotonPlayerConnected(PhotonPlayer player) {
        Lobby lobby = GameObject.FindGameObjectWithTag("Lobby").GetComponent<Lobby>();
        lobby.UpdateWaitingPanel();
        if (PhotonNetwork.isMasterClient) {
            lobby.CheckReadyState();//this will check to see if the room is full and ready to go
        }
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer) {
        if (!Friends.IsPlayerBlocked(photonPlayer.UserId)) {
            if (ChatPanel.Instance != null) {
                ChatPanel.Instance.PlayerStatus(photonPlayer.CustomProperties["name"] + " : Has left.");
            }
        }
        if (GameObject.FindGameObjectWithTag("Lobby") != null) {
            GameObject.FindGameObjectWithTag("Lobby").GetComponent<Lobby>().UpdateWaitingPanel();
        }
        if (PhotonNetwork.isMasterClient) {
            if (GameObject.FindGameObjectWithTag("GameManager") != null) {
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>().DetermineNPCTurn();
            }
        }
    }

    private void OnMasterClientSwitched() {
        if (PhotonNetwork.isMasterClient) {
            if (GameObject.FindGameObjectWithTag("GameManager") != null) {
                if (!GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>().playerWon) {
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>().DetermineNPCTurn();
                } else {
                    if (GameObject.FindGameObjectWithTag("VictoryPanel") != null) {
                        if (GameObject.FindGameObjectWithTag("VictoryPanel").GetComponent<Victory>().playerStandings.Length == 0) {
                            GameObject.FindGameObjectWithTag("VictoryPanel").GetComponent<Victory>().playerStandings = GameObject.FindGameObjectWithTag("VictoryPanel").GetComponent<Victory>().GetPlayerStandings();
                        }
                    }
                }
            }
        }
    }

    private void OnApplicationQuit() {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

}
