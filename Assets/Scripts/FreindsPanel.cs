using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class FreindsPanel : MonoBehaviour
{

    [SerializeField] GameObject friendsPanelParent;

    private void OnEnable() {
        BuildFriends();
    }

    private void BuildFriends() {
        DeleteFriends();

        if (LocalUser.locUser.friends.Count > 0) {
            PhotonNetwork.FindFriends(LocalUser.locUser.friends.ToArray());
        }
        
        if (PhotonNetwork.Friends != null) {
            for (int i = 0; i < PhotonNetwork.Friends.Count; i++) {
                User friend = new User();
                for (int x =0; x < Friends.friends.Count; x++) {
                    if (Friends.friends[x].userID == PhotonNetwork.Friends[i].UserId) {
                        friend = Friends.friends[x];
                        break;
                    }
                }
                if (PhotonNetwork.Friends[i].IsOnline) {
                    SpawnFriendPanel(friend, true);
                } else {
                    SpawnFriendPanel(friend, false);
                }
            }
        }
    }

    void SpawnFriendPanel(User user, bool status) {
        GameObject friendPanel = Instantiate(Resources.Load<GameObject>("FriendPanel"), friendsPanelParent.transform);
        friendPanel.transform.localScale = new Vector3(1, 1, 1);
        friendPanel.GetComponent<FriendListInfoPanel>().SetUpFriendPanel(user, status);
    }

    private void DeleteFriends() {
        for (int i = 0; i < friendsPanelParent.transform.childCount; i++) {
            Destroy(friendsPanelParent.transform.GetChild(0).gameObject);
        }
    }

}
