using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController: MonoBehaviour {

    public static void LoadStartMenu() {
        LoadScreen();
        PhotonNetwork.LoadLevelAsync(0);
    }

    public static void LoadGameSetup() {
        LoadScreen();
        PhotonNetwork.LoadLevelAsync(1);
    }

    public static void LoadLobbyScene() {
        LoadScreen();
        if (PhotonNetwork.isMasterClient) 
            PhotonNetwork.LoadLevelAsync(2);
    }

    public static void LoadGameScene() {
        LoadScreen();
        if (PhotonNetwork.offlineMode)
            PhotonNetwork.LoadLevel(3);
        else if (PhotonNetwork.isMasterClient)
            PhotonNetwork.LoadLevelAsync(3);
    }

    public static void ReloadScene() {
        LoadScreen();
        Scene scene = SceneManager.GetActiveScene();
        PhotonNetwork.LoadLevel(scene.buildIndex);
    }

    public static void LoadScreen()
    {
        Time.timeScale = 1;
        LoadingScreen.Instance.TurnOnLoadingScreen();
    }

}
