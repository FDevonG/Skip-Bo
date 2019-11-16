using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void QuitTheGame() {
        PhotonNetwork.LeaveRoom();
        Application.Quit();
    }
}
