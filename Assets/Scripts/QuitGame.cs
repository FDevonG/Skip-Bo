using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void QuitTheGame() {
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
}
