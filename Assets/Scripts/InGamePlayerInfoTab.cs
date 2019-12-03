using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePlayerInfoTab : MonoBehaviour {

    [SerializeField] Text nameText;
    [SerializeField] Text infoText;
    [SerializeField] Button voteToKickButton;
    [SerializeField] Button blockButton;
    [SerializeField] Button addFriendButton;
    [SerializeField] Button closeButton;
    [SerializeField] PhotonView photonView;

    PhotonPlayer photonPlayer;

    public void SetUpPanel(PhotonPlayer sentPhotonPlayer) {
        photonPlayer = sentPhotonPlayer;
        gameObject.GetComponent<Image>().color = new Color(0.8313726f, 0.7137255f, 0.5411765f, 1);
        nameText.gameObject.SetActive(true);
        infoText.gameObject.SetActive(false);
        blockButton.gameObject.SetActive(true);
        addFriendButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
        nameText.text = (string)photonPlayer.CustomProperties["name"];

        //if (PhotonNetwork.room.MaxPlayers == 4) {
            voteToKickButton.gameObject.SetActive(true);
        //} else {
        //    voteToKickButton.gameObject.SetActive(false);
        //}
    }

    public void KickPlayer() {
        if (!GameObject.FindGameObjectWithTag("VoteToKick").GetComponent<VoteToKickPanel>().kickInProgress) {
            photonView.RPC("VoteToKick", PhotonTargets.Others, photonPlayer);
            SetInfoText("Vote to kick " + photonPlayer.CustomProperties["name"] + " sent");
        } else {
            SetInfoText("Kick in progress");
        }
    }

    [PunRPC]
    public void VoteToKick(PhotonPlayer photonPlayerToKick) {
        StartCoroutine(GameObject.FindGameObjectWithTag("VoteToKick").GetComponent<VoteToKickPanel>().SetUpKickPanel(photonPlayerToKick));
    }

    public void AddFriend() {
        StartCoroutine(AddingFriend());
    }

    public IEnumerator AddingFriend() {
        if (!Friends.FriendAlreadyAdded(photonPlayer.UserId)) {
            CoroutineWithData cd = new CoroutineWithData(this, Friends.AddFriend(photonPlayer.UserId));
            yield return cd.coroutine;
            SetInfoText((string)cd.result);
        } else {
            SetInfoText("Friend already added");
        }
    }

    public void Block() {
        Friends.BlockFriend(photonPlayer.UserId);
        SetInfoText((string)photonPlayer.CustomProperties["name"] + " has been blocked");
    }

    public void ClosePanel() {
        photonPlayer = null;
        voteToKickButton.gameObject.SetActive(false);
        nameText.gameObject.SetActive(false);
        infoText.gameObject.SetActive(false);
        blockButton.gameObject.SetActive(false);
        addFriendButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        gameObject.GetComponent<Image>().color = new Color(0.8313726f, 0.7137255f, 0.5411765f, 0);
    }

    private void SetInfoText(string message) {
        infoText.gameObject.SetActive(true);
        infoText.text = message;
    }

    private void OnDisable() {
        infoText.gameObject.SetActive(false);
    }
}
