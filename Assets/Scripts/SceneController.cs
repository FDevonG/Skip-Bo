using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController: MonoBehaviour {

    public static void LoadStartMenu() {
        LoadingScreen();
        PhotonNetwork.LoadLevelAsync(0);
    }

    public static void LoadGameSetup() {
        LoadingScreen();
        PhotonNetwork.LoadLevelAsync(1);
    }

    public static void LoadLobbyScene() {
        LoadingScreen();
        if (PhotonNetwork.isMasterClient) 
            PhotonNetwork.LoadLevelAsync(2);
    }

    public static void LoadGameScene() {
        LoadingScreen();
        if (PhotonNetwork.offlineMode)
            PhotonNetwork.LoadLevel(3);
        else if (PhotonNetwork.isMasterClient)
            PhotonNetwork.LoadLevelAsync(3);
    }

    public static void ReloadScene() {
        LoadingScreen();
        Scene scene = SceneManager.GetActiveScene();
        PhotonNetwork.LoadLevel(scene.buildIndex);
    }

    public static void LoadingScreen()
    {
        Time.timeScale = 1;
        GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOnLoadingScreen();
    }

}
