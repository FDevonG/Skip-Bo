using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Button joinRoomButton;
    [SerializeField] Button closeButton;
    [SerializeField] Text infoText;
    [SerializeField] GameObject deleteFriendPanel;
    [SerializeField] Text areYouSureText;

    User friend;

    public void SetUpFriendPanel(User user) {
        friend = user;
        nameText.text = user.userName;
        hair.sprite = Resources.Load<Sprite>("Faces/Hairs/" + user.hair) as Sprite;
        face.sprite = Resources.Load<Sprite>("Faces/Faces/" + user.face) as Sprite;
        body.sprite = Resources.Load<Sprite>("Faces/Bodies/" + user.body) as Sprite;
        kit.sprite = Resources.Load<Sprite>("Faces/Kits/" + user.kit) as Sprite;
        foreach (FriendInfo photonFriend in PhotonNetwork.Friends) {
            if (photonFriend.UserId == friend.userID) {
                if (photonFriend.IsInRoom) {
                    joinRoomButton.interactable = true;
                    joinRoomButton.onClick.AddListener(() => {
                        foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
                            if (game.Name == photonFriend.Room) {
                                if (game.IsOpen) {
                                    PhotonNetwork.JoinRoom(game.Name);
                                } else {
                                    infoText.gameObject.SetActive(true);
                                    infoText.text = "Room Full";
                                }
                            }
                        }
                    });
                } else {
                    joinRoomButton.interactable = false;
                }
            }
        }
    }

    public void ShowDeleteFrindPanel() {
        deleteFriendPanel.SetActive(true);
        areYouSureText.text = "Are you sure you want to delete " + friend.userName;
        nameText.gameObject.SetActive(false);
        character.gameObject.SetActive(false);
        deleteFriendButton.gameObject.SetActive(false);
        joinRoomButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    public void HideDeleteFriendPanel() {
        deleteFriendPanel.SetActive(false);
        nameText.gameObject.SetActive(true);
        character.gameObject.SetActive(true);
        deleteFriendButton.gameObject.SetActive(true);
        joinRoomButton.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }

    public void DeleteFriend() {
        
        foreach (User friendUser in Friends.friends) {
            if (friendUser == friend) {
                Friends.friends.Remove(friendUser);
                break;
            }
        }

        for (int i = 0; i < LocalUser.locUser.friends.Count; i++) {
            if (LocalUser.locUser.friends[i] == friend.userID) {
                string[] removedFriend = new string[1];
                removedFriend[0] = LocalUser.locUser.friends[i];
                GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().DeleteFriends(removedFriend);
                LocalUser.locUser.friends.Remove(LocalUser.locUser.friends[i]);
                break;
            }
        }
        PhotonNetwork.Friends = null;
        if (LocalUser.locUser.friends.Count > 0) {
            PhotonNetwork.FindFriends(LocalUser.locUser.friends.ToArray());
        }
        FireBaseScript.UpdateUserFriends(LocalUser.locUser.friends);
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().friendsPanel);
    }

    private void OnDisable() {
        HideDeleteFriendPanel();
    }

}
