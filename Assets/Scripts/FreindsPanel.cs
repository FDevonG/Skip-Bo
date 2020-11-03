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
        LoadingScreen.Instance.TurnOnLoadingScreen();
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
            //PhotonNetwork.FindFriends(LocalUser.locUser.friends.ToArray());
            CoroutineWithData cd = new CoroutineWithData(this, GetFriends());
            yield return cd.result;
            string returnedString = cd.result as string;

            string[] strArr;
            strArr = returnedString.Split('#');

            foreach (string str in strArr)
            {
                if(!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str))
                {
                    User friend = JsonUtility.FromJson<User>(str);
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

    IEnumerator GetFriends()
    {
        var task = BackendFunctions.GetFriends();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted)
        {
            GetComponent<ErrorText>().SetError("Failed to load friends");
            yield return null;
        }
        else
        {
            yield return task.Result;
        }
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
