using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class InviteFriendsToGame : MonoBehaviour {

    [SerializeField] GameObject friendsPanelParent;
    [SerializeField] Text infoText;

    private void OnEnable() {
        BuildFriendsList();
    }
    
    private void OnDisable() {
        infoText.gameObject.SetActive(false);
    }

    private void BuildFriendsList() {
        DestroyFriendPanels();
        for (int i = 0; i < Chat.friendsOnline.Count; i++) {
            User friend = new User();
            for (int x = 0; x < Friends.friends.Count; x++) {
                if (Friends.friends[x].userID == Chat.friendsOnline[i]) {
                    friend = Friends.friends[x];
                    break;
                }
            }
            GameObject button = Instantiate(Resources.Load<GameObject>("Button"), friendsPanelParent.transform);
            button.transform.GetChild(0).GetComponent<Text>().text = friend.userName;
            button.transform.localScale = new Vector3(1, 1, 1);
            button.GetComponent<Button>().onClick.AddListener(() => GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().SendGameInvite(friend.userID, PhotonNetwork.room.Name + "@" + LocalUser.locUser.userName));
            button.GetComponent<Button>().onClick.AddListener(() => SetInfoText(friend.userName + " has been invited"));
            button.GetComponent<Button>().onClick.AddListener(() => button.GetComponent<Button>().interactable = false);
        }
    }

    private void DestroyFriendPanels() {
        if (friendsPanelParent.transform.childCount > 0) {
            for (int i = 0; i < friendsPanelParent.transform.childCount; i++) {
                Destroy(friendsPanelParent.transform.GetChild(0).gameObject);
            }
        }
    }

    private void SetInfoText(string message) {
        infoText.gameObject.SetActive(true);
        infoText.text = message;
    }
}
