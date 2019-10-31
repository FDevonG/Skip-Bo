using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    [SerializeField]
    Text infoText;
    [SerializeField]
    Text numberText;
    [SerializeField]
    PhotonView photonView;
    [SerializeField]
    Image loadinImage;
    [SerializeField]
    Button leaveButton;

    bool gameStarting = false;

    private void OnEnable() {
        UpdateWaitingPanel();
    }

    public void UpdateWaitingPanel() {
        infoText.text = "Waiting For More Players:";
        numberText.text = PhotonNetwork.room.PlayerCount + "/" + PhotonNetwork.room.MaxPlayers;
        loadinImage.gameObject.SetActive(true);
        leaveButton.gameObject.SetActive(true);
        if (PhotonNetwork.isMasterClient && !PhotonNetwork.offlineMode) {
            PhotonNetwork.room.IsOpen = true;
            PhotonNetwork.room.IsVisible = true;
        }
    }

    public void CheckReadyState() {
        if (PhotonNetwork.isMasterClient) {
            if (PhotonNetwork.playerList.Length == PhotonNetwork.room.MaxPlayers) {
                PhotonNetwork.room.IsOpen = false;
                PhotonNetwork.room.IsVisible = false;
                photonView.RPC("LaunchingGame", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    private IEnumerator LaunchingGame() {
        gameStarting = true;
        yield return new WaitForSeconds(1);
        int countDown = 3;
        infoText.text = "Game Starting In";
        numberText.text = countDown + "!";
        loadinImage.gameObject.SetActive(false);
        leaveButton.gameObject.SetActive(false);
        while (countDown > 0) {
            yield return new WaitForSeconds(1);
            if (PhotonNetwork.room.PlayerCount != PhotonNetwork.room.MaxPlayers) {
                UpdateWaitingPanel();
                countDown = 0;
            } else {
                countDown--;
                numberText.text = countDown + "!";
            }
        }
        if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers) {
            SceneController.LoadGameScene();
        }
    }

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneController.LoadStartMenu();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!gameStarting) {
                LeaveRoom();
            }
        }
    }

}
