using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreindsPanel : MonoBehaviour
{

    [SerializeField] GameObject friendsPanelParent;
    [SerializeField] GameObject headerText;
    List<GameObject> friendPanels = new List<GameObject>();
    GameObject skipboFriendsText;
    GameObject facebookFriendsText;
    private void OnEnable() {
        StartCoroutine(BuildFriends());
    }

    private IEnumerator BuildFriends() {
        DeleteFriends();
        PhotonNetwork.Friends = null;
        if (LocalUser.locUser.friends.Count > 0) {
            PhotonNetwork.FindFriends(LocalUser.locUser.friends.ToArray());
            while (PhotonNetwork.Friends == null) {
                yield return new WaitForSeconds(1);
                PhotonNetwork.FindFriends(LocalUser.locUser.friends.ToArray());
            }
        }
        if (PhotonNetwork.Friends != null) {
            //skipboFriendsText = Instantiate(headerText, friendsPanelParent.transform);
            //skipboFriendsText.GetComponent<Text>().text = "Skip-Bo Friends";
            for (int i = 0; i < PhotonNetwork.Friends.Count; i++) {
                User friend = new User();
                for (int x =0; x < Friends.friends.Count; x++) {
                    if (Friends.friends[x].userID == PhotonNetwork.Friends[i].UserId) {
                        friend = Friends.friends[x];
                        break;
                    }
                }
                if (!Friends.AmIBlocked(friend)) {
                    if (PhotonNetwork.Friends[i].IsOnline) {
                        SpawnFriendPanel(friend, true);
                    } else {
                        SpawnFriendPanel(friend, false);
                    }
                }
            }
        }
        
        //if (FacebookScript.Instance.IsFacebookLoggedIn())
        //{
        //    Dictionary<string, object> facebookFriends = FacebookScript.Instance.GetFriendsPlayingThisGame();
        //    if (facebookFriends.Count > 0)
        //    {
        //        facebookFriendsText = Instantiate(headerText, friendsPanelParent.transform);
        //        facebookFriendsText.GetComponent<Text>().text = "Facebook Friends";
        //    }
        //    Debug.Log(facebookFriends);
        //}
    }

    void SpawnFriendPanel(User user, bool status) {
        if (!string.IsNullOrEmpty(user.userName) && !string.IsNullOrWhiteSpace(user.userName)) {
            GameObject friendPanel = Instantiate(Resources.Load<GameObject>("FriendPanel"), friendsPanelParent.transform);
            friendPanel.transform.localScale = new Vector3(1, 1, 1);
            friendPanel.GetComponent<FriendListInfoPanel>().SetUpFriendPanel(user, status);
            friendPanels.Add(friendPanel);
        }
    }

    private void DeleteFriends() {
        foreach (GameObject panel in friendPanels) {
            Destroy(panel);
        }
        friendPanels.Clear();
    }

}
