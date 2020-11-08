using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreindsPanel : MonoBehaviour
{

    [SerializeField] GameObject friendsPanelParent;
    [SerializeField] GameObject headerText;
    List<GameObject> friendPanels = new List<GameObject>();

    private void OnEnable()
    {
        LoadingScreen.Instance.TurnOnLoadingScreen("Loading");
        StartCoroutine(BuildFriends());
    }

    private void OnDisable()
    {
        GetComponent<ErrorText>().ClearError();
    }

    private IEnumerator BuildFriends()
    {
        DeleteFriends();
        if (LocalUser.locUser.friends.Count > 0)
        {
            if (!PhotonNetwork.connected)
            {
                yield return new WaitUntil(() => PhotonNetwork.connected);
            }
            var task = BackendFunctions.GetUsersArray(LocalUser.locUser.friends);
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.IsFaulted)
            {
                GetComponent<ErrorText>().SetError("Failed");
            }
            else
            {
                Debug.Log(task.Result);
                UserArray friends = JsonUtility.FromJson<UserArray>(task.Result);
                
                foreach (User friend in friends.users)
                {
                    foreach (FriendInfo pp in PhotonNetwork.Friends)
                    {
                        if (pp.UserId == friend.userID)
                        {
                            if (pp.IsOnline)
                                SpawnFriendPanel(friend, true);
                            else
                                SpawnFriendPanel(friend, false);
                            break;
                        }
                    }
                }
            }
        }
        LoadingScreen.Instance.TurnOffLoadingScreen();
    }

    void SpawnFriendPanel(User user, bool status)
    {
        if (!string.IsNullOrEmpty(user.userName) && !string.IsNullOrWhiteSpace(user.userName))
        {
            GameObject friendPanel = Instantiate(Resources.Load<GameObject>("FriendPanel"), friendsPanelParent.transform);
            friendPanel.transform.localScale = new Vector3(1, 1, 1);
            friendPanel.GetComponent<FriendListInfoPanel>().SetUpFriendPanel(user, status);
            friendPanels.Add(friendPanel);
        }
    }

    private void DeleteFriends()
    {
        foreach (GameObject panel in friendPanels)
        {
            Destroy(panel);
        }
        friendPanels.Clear();
    }

}
