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

        if (PhotonNetwork.room.MaxPlayers == 4) {
            voteToKickButton.gameObject.SetActive(true);
        } else {
            voteToKickButton.gameObject.SetActive(false);
        }

        if (Friends.FriendAlreadyAdded(photonPlayer.UserId)) {
            addFriendButton.GetComponentInChildren<Text>().text = "Delete Friend";
            addFriendButton.onClick.AddListener(() => DeleteFriend());
        }
        if (!Friends.FriendAlreadyAdded(photonPlayer.UserId)) {
            addFriendButton.GetComponentInChildren<Text>().text = "Add Friend";
            addFriendButton.onClick.AddListener(() => StartCoroutine(AddingFriend()));
        }

        if (Friends.IsPlayerBlocked(photonPlayer.UserId)) {
            blockButton.GetComponentInChildren<Text>().text = "Unblock";
            blockButton.onClick.AddListener(UnBlock);
        } else {
            blockButton.GetComponentInChildren<Text>().text = "Block";
            blockButton.onClick.AddListener(Block);
        }

        //here we will check to see if we are logged in anonoum and set the buttons acordingly
        if (FireBaseScript.IsPlayerAnonymous()) {
            blockButton.interactable = false;
            addFriendButton.interactable = false;
        } else {
            blockButton.interactable = true;
            addFriendButton.interactable = true;
        }
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

    public IEnumerator AddingFriend() {
        if (!Friends.FriendAlreadyAdded(photonPlayer.UserId)) {
            CoroutineWithData cd = new CoroutineWithData(this, Friends.AddFriend(photonPlayer.UserId));
            yield return cd.coroutine;
            SetInfoText((string)cd.result);
            if ((string)cd.result == "Friend added") {
                addFriendButton.GetComponentInChildren<Text>().text = "Delete Friend";
                addFriendButton.onClick.RemoveListener(() => StartCoroutine(AddingFriend()));
                addFriendButton.onClick.AddListener(DeleteFriend);
            }
        } //else {
        //    SetInfoText("Friend already added");
        //}
    }

    private void DeleteFriend() {
        User friendToDelete = new User();
        foreach (User friend in Friends.friends) {
            if (friend.userID == photonPlayer.UserId) {
                friendToDelete = friend;
                break;
            }
        }
        Friends.DeleteFriend(friendToDelete);
        SetInfoText("Friend Deleted");
        addFriendButton.GetComponentInChildren<Text>().text = "Add Friend";
        addFriendButton.onClick.RemoveListener(DeleteFriend);
        addFriendButton.onClick.AddListener(() => StartCoroutine(AddingFriend()));
    }

    public void Block() {
        Friends.BlockFriend(photonPlayer.UserId);
        SetInfoText((string)photonPlayer.CustomProperties["name"] + " has been blocked");
        blockButton.onClick.RemoveListener(Block);
        blockButton.onClick.AddListener(UnBlock);
        blockButton.GetComponentInChildren<Text>().text = "Unblock";
    }

    public void UnBlock() {
        Friends.UnblockPlayer(photonPlayer.UserId);
        SetInfoText(photonPlayer.CustomProperties["name"] + " has been unblocked");
        blockButton.onClick.RemoveListener(UnBlock);
        blockButton.onClick.AddListener(Block);
        blockButton.GetComponentInChildren<Text>().text = "Block";
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
