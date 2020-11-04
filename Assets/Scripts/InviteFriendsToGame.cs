using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InviteFriendsToGame : MonoBehaviour
{

    [SerializeField] GameObject friendsPanelParent;
    [SerializeField] Text infoText;
    [SerializeField] GameObject loadingPanel;

    private void OnEnable()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(BuildFriendsList());
    }

    private void OnDisable()
    {
        StopCoroutine(BuildFriendsList());
        infoText.gameObject.SetActive(false);
    }

    private IEnumerator BuildFriendsList()
    {
        DestroyFriendPanels();

        if (LocalUser.locUser.friends.Count > 0)
        {
            CoroutineWithData cd = new CoroutineWithData(this, GetFriends());
            yield return cd.result;
            string returnedString = cd.result as string;

            string[] strArr;
            strArr = returnedString.Split('#');
            loadingPanel.SetActive(false);
            foreach (string str in strArr)
            {
                if (!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str))
                {
                    User friend = JsonUtility.FromJson<User>(str);
                    Debug.Log(friend.userName);
                    foreach (FriendInfo pp in PhotonNetwork.Friends)
                    {
                        if (pp.UserId == friend.userID)
                        {
                            if (pp.IsOnline)
                            {
                                GameObject playerTab = Instantiate(Resources.Load<GameObject>("FriendInvitePanel"), friendsPanelParent.transform);
                                playerTab.transform.localScale = new Vector3(1, 1, 1);
                                playerTab.GetComponent<FriendInvitePanel>().nameText.text = friend.userName;
                                playerTab.GetComponent<FriendInvitePanel>().inviteButton.onClick.AddListener(() =>
                                {
                                    Achievments.Instance.UnlockAchievement("Better with friends");
                                    Chat.Instance.SendGameInvite(friend.userID, PhotonNetwork.room.Name + "@" + LocalUser.locUser.userName);
                                    SetInfoText(friend.userName + " has been invited");
                                    playerTab.GetComponent<FriendInvitePanel>().inviteButton.interactable = false;
                                });
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator GetFriends()
    {
        var task = BackendFunctions.GetUsers(LocalUser.locUser.friends);
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

    private void DestroyFriendPanels()
    {
        if (friendsPanelParent.transform.childCount > 0)
        {
            for (int i = 0; i < friendsPanelParent.transform.childCount; i++)
            {
                if(friendsPanelParent.transform.GetChild(0).gameObject != loadingPanel)
                    Destroy(friendsPanelParent.transform.GetChild(0).gameObject);
            }
        }
    }

    private void SetInfoText(string message)
    {
        infoText.gameObject.SetActive(true);
        infoText.text = message;
    }
}
