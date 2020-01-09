using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VoteToKickPanel : MonoBehaviour
{
    [SerializeField] Text mainText;
    [SerializeField] Text timerText;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] PhotonView photonView;

    int timeLeft = 10;
    int votesToKick = 0;

    public bool kickInProgress = false;

    PhotonPlayer photonPlayer;

    public IEnumerator SetUpKickPanel(PhotonPlayer player) {
        if (player != PhotonNetwork.player) {
            while (CardDragHandler.itemBeingDragged != null) {
                yield return new WaitUntil(() => CardDragHandler.itemBeingDragged = null);
            }
            mainText.gameObject.SetActive(true);
            timerText.gameObject.SetActive(true);
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
            gameObject.GetComponent<Image>().color = new Color(0.8313726f, 0.7137255f, 0.5411765f, 1);
            mainText.text = "Kick " + (string)player.CustomProperties["name"] + "?";
            timeLeft = 10;
            timerText.text = timeLeft.ToString();
        }
        kickInProgress = true;
        photonPlayer = player;
        votesToKick = 0;
        if (PhotonNetwork.isMasterClient) {
            AddVoteToKick();
        }
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown() {
        while (kickInProgress) {
            yield return new WaitForSeconds(1);
            timeLeft--;
            if (timeLeft < 0) {
                ClosePanel();
                if (PhotonNetwork.isMasterClient) {
                    CheckVotes();
                }
                kickInProgress = false;
                photonPlayer = null;
            }
            timerText.text = timeLeft.ToString();
        }
    }

    public void SendVote() {
        photonView.RPC("AddVoteToKick", PhotonTargets.MasterClient);
        ClosePanel();
    }

    [PunRPC]
    public void AddVoteToKick() {
        if (PhotonNetwork.isMasterClient) {
            votesToKick++;
            CheckVotes();
        }
    }

    private void CheckVotes() {
        if (votesToKick >= 0) {
            kickInProgress = false;
            photonView.RPC("KickPlayer", photonPlayer);
        }
    }

    [PunRPC]
    public void KickPlayer() {
        PhotonNetwork.LeaveRoom();
        SceneController.LoadStartMenu();
        GameObject notificationPanel = Instantiate(Resources.Load<GameObject>("NotificationPanel"));
        notificationPanel.GetComponent<NotificationPanel>().SetText("You have been kicked");
    }

    public void ClosePanel() {
        mainText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        gameObject.GetComponent<Image>().color = new Color(0.8313726f, 0.7137255f, 0.5411765f, 0);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(votesToKick);
            stream.SendNext(kickInProgress);
        } else {
            votesToKick = (int)stream.ReceiveNext();
            kickInProgress = (bool)stream.ReceiveNext();
        }
    }
}
