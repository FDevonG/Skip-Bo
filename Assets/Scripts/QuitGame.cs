using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void QuitTheGame() {
        Announcer.Instance.GoodBye();
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
}
