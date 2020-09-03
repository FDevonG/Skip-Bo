using UnityEngine.SceneManagement;

public static class SceneController {

    public static void LoadStartMenu() {
        PhotonNetwork.LoadLevel(0);
    }

    public static void LoadGameSetup() {
        PhotonNetwork.LoadLevel(1);
    }

    public static void LoadLobbyScene() {
        if (PhotonNetwork.isMasterClient) {
            PhotonNetwork.LoadLevel(2);
        }
    }

    public static void LoadGameScene() {
        if (PhotonNetwork.isMasterClient) {
            PhotonNetwork.LoadLevel(3);
        }
    }

    public static void ReloadScene() {
        Scene scene = SceneManager.GetActiveScene();
        PhotonNetwork.LoadLevel(scene.buildIndex);
    }

}
