using UnityEngine;
using UnityEngine.UI;

public class FriendSettingsPanel : MonoBehaviour
{

    [SerializeField] Text nameText;
    [SerializeField] GameObject character;
    [SerializeField] Image hair;
    [SerializeField] Image face;
    [SerializeField] Image kit;
    [SerializeField] Image body;
    [SerializeField] Button deleteFriendButton;
    [SerializeField] Button blockUserButton;
    [SerializeField] Button joinRoomButton;
    [SerializeField] Button closeButton;

    [SerializeField] GameObject deleteFriendPanel;
    [SerializeField] Text areYouSureText;
    [SerializeField] Button confirmActionButton;

    User friend;

    public void SetUpFriendPanel(User user)
    {
        friend = user;
        nameText.text = user.userName;
        hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + user.hair) as Sprite;
        face.sprite = Resources.Load<Sprite>("Faces/Faces/" + user.face) as Sprite;
        body.sprite = Resources.Load<Sprite>("Faces/Bodies/" + user.body) as Sprite;
        kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + user.kit) as Sprite;
        foreach (FriendInfo photonFriend in PhotonNetwork.Friends)
        {
            if (photonFriend.UserId == friend.userID)
            {
                if (photonFriend.IsInRoom)
                {
                    joinRoomButton.interactable = true;
                    joinRoomButton.onClick.AddListener(() => {
                        foreach (RoomInfo game in PhotonNetwork.GetRoomList())
                        {
                            if (game.Name == photonFriend.Room)
                            {
                                if (game.IsOpen)
                                {
                                    PhotonNetwork.JoinRoom(game.Name);
                                }
                                else
                                {
                                    GetComponent<ErrorText>().SetError("Room Full");
                                }
                            }
                        }
                    });
                }
                else
                {
                    joinRoomButton.interactable = false;
                }
            }
        }
        SetBlockedButton();
    }

    private void SetBlockedButton()
    {

        if (Friends.IsPlayerBlocked(friend.userID))
        {
            blockUserButton.GetComponentInChildren<Text>().text = "Unblock";
        }
        else
        {
            blockUserButton.GetComponentInChildren<Text>().text = "Block";
        }
    }

    public void BlockedButtonPressed()
    {
        if (Friends.IsPlayerBlocked(friend.userID))
        {
            Friends.UnblockPlayer(friend.userID);
            blockUserButton.GetComponentInChildren<Text>().text = "Block";
        }
        else
        {
            ShowDeleteFrindPanel();
            SetUpBlockPanel();
        }
    }

    public void ShowDeleteFrindPanel()
    {
        deleteFriendPanel.SetActive(true);
        nameText.gameObject.SetActive(false);
        character.gameObject.SetActive(false);
        deleteFriendButton.gameObject.SetActive(false);
        joinRoomButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        blockUserButton.gameObject.SetActive(false);
    }

    public void SetUpDeletePanle()
    {
        areYouSureText.text = "Are you sure you want to delete " + friend.userName + "?";
        confirmActionButton.onClick.AddListener(() => Friends.DeleteFriend(friend));
    }

    public void SetUpBlockPanel()
    {
        areYouSureText.text = "Are you sure you want to block " + friend.userName + "?";
        confirmActionButton.onClick.AddListener(() => Friends.BlockFriend(friend.userID));
    }

    public void HideDeleteFriendPanel()
    {
        deleteFriendPanel.SetActive(false);
        nameText.gameObject.SetActive(true);
        character.gameObject.SetActive(true);
        deleteFriendButton.gameObject.SetActive(true);
        joinRoomButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
        blockUserButton.gameObject.SetActive(true);

        SetBlockedButton();
    }

    private void OnDisable()
    {
        HideDeleteFriendPanel();
        GetComponent<ErrorText>().ClearError();
    }

}
