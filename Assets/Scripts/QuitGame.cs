using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void QuitTheGame() {
        GameObject.FindGameObjectWithTag("Announcer").GetComponent<Announcer>().GoodBye();
        PhotonNetwork.Disconnect();
        Application.Quit();
    }
}
