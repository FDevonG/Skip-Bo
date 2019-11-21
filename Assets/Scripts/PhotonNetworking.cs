using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PhotonNetworking : MonoBehaviour {

    public static PhotonNetworking Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        if (FireBaseScript.IsPlayerLoggedIn()) {
            StartCoroutine(ConnectToPhoton());
        }
    }

    void Start() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 0) {
            StartCoroutine(ConnectToPhoton());
        }
    }

    public IEnumerator ConnectToPhoton() {
        if (!PhotonNetwork.connected) {
            yield return StartCoroutine(GetComponent<PhotonRooms>().SetupPhotonPlayer());
            Debug.Log(PhotonNetwork.player.UserId);
            PhotonNetwork.ConnectUsingSettings(GameGlobalSettings.Version());
        }
    }

    private void OnConnectedToMaster() {
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.autoCleanUpPlayerObjects = true;
        GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().ConnectToChat();
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

    private void OnApplicationQuit() {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

}
