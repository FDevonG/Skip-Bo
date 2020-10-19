using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreindsPanel : MonoBehaviour
{

    [SerializeField] GameObject friendsPanelParent;
    [SerializeField] GameObject headerText;
    List<GameObject> friendPanels = new List<GameObject>();

    private void OnEnable() {
        LoadingScreen.Instance.TurnOnLoadingScreen();
        StartCoroutine(BuildFriends());
    }

    private IEnumerator BuildFriends() {
        DeleteFriends();
        if (LocalUser.locUser.friends.Count > 0)
        {
            PhotonNetwork.FindFriends(LocalUser.locUser.friends.ToArray());
            yield return new WaitUntil(() => PhotonNetwork.Friends != null);

            if (Friends.friends.Count != LocalUser.locUser.friends.Count)
            {
                yield return new WaitUntil(() => Friends.friends.Count == LocalUser.locUser.friends.Count);
            }
            Debug.Log(PhotonNetwork.Friends.Count);
            if (PhotonNetwork.Friends != null)
            {
                for (int i = 0; i < PhotonNetwork.Friends.Count; i++)
                {
                    User friend = new User();
                    for (int x = 0; x < Friends.friends.Count; x++)
                    {
                        if (Friends.friends[x].userID == PhotonNetwork.Friends[i].UserId)
                        {
                            friend = Friends.friends[x];
                            break;
                        }
                    }
                    if (!Friends.AmIBlocked(friend))
                    {
                        if (PhotonNetwork.Friends[i].IsOnline)
                        {
                            SpawnFriendPanel(friend, true);
                        }
                        else
                        {
                            SpawnFriendPanel(friend, false);
                        }
                    }
                }
            }
        }
        LoadingScreen.Instance.TurnOffLoadingScreen();
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
