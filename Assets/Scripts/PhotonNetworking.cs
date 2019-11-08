using UnityEngine;

public class PhotonNetworking : MonoBehaviour {

    private string versionNumber = "1.3";

    public static PhotonNetworking Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectToPhoton() {
        PhotonNetwork.ConnectUsingSettings(versionNumber);
    }

    private void OnConnectedToMaster() {
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.autoCleanUpPlayerObjects = true;
    }

    //this is called when you fail to connect to photon
    //private void OnFailedToConnectToPhoton() {
    //    Debug.Log("Failed to connect to photon");
    //}

    private void OnJoinedLobby() {
        SceneController.LoadGameSetup();
    }

    //this is called when you get disconnected from photon
    private void OnDisconnectedFromPhoton() {
        Debug.Log("Disconnected from photon");
        if (!NetworkCheck()) {
            Instantiate(Resources.Load<GameObject>("DisconnectedFromPhotonCanvas") as GameObject);
        }
    }

    private void OnCreatedRoom() {
        Debug.Log("Room Created");
        if (!PhotonNetwork.offlineMode) {
            SceneController.LoadLobbyScene();
        }
    }

    //this method is called when you join a room automatically
    private void OnJoinedRoom() {
        Debug.Log("Joined room");
        SceneController.LoadLobbyScene();
    }

    private void OnPhotonPlayerConnected(PhotonPlayer player) {
        Debug.Log("Player Connected");
        Lobby lobby = GameObject.FindGameObjectWithTag("Lobby").GetComponent<Lobby>();
        lobby.UpdateWaitingPanel();
        if (PhotonNetwork.isMasterClient) {
            lobby.CheckReadyState();//this will check to see if the room is full and ready to go
        }
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer) {
        Debug.Log("Player Disconected");
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
                Debug.Log("Master Switched");
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameControl>().DetermineNPCTurn();
            }
        }
    }

    private bool NetworkCheck() {
        if (Application.internetReachability != NetworkReachability.NotReachable) {
            return true;
        } else {
            return false;
        }
    }

    private void OnApplicationQuit() {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

}
