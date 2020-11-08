using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockedPanel : MonoBehaviour
{
    [SerializeField] GameObject blockedPanelsParent;
    public static List<GameObject> blockedPlayers = new List<GameObject>();

    private void OnEnable()
    {
        LoadingScreen.Instance.TurnOnLoadingScreen("Loading");
        StartCoroutine(BuildBlockedList());
    }

    private void OnDisable()
    {
        DestroyBlockedChildren();
    }

    private IEnumerator BuildBlockedList()
    {
        if (LocalUser.locUser.blocked.Count > 0)
        {
            var task = BackendFunctions.GetUsersArray(LocalUser.locUser.blocked);
            yield return new WaitUntil(() => task.IsCompleted);
            if (!task.IsFaulted)
            {
                UserArray friends = JsonUtility.FromJson<UserArray>(task.Result);

                foreach (User str in friends.users)
                {
                    SpawnBlockedPanel(str);
                }
            }
            else
                GetComponent<ErrorText>().SetError("Failed");
        }
        LoadingScreen.Instance.TurnOffLoadingScreen();
    }

    private void SpawnBlockedPanel(User blockedUser)
    {
        GameObject blockedPlayerPanel = Instantiate(Resources.Load<GameObject>("BlockedPlayerPanel"), blockedPanelsParent.transform);
        blockedPlayerPanel.transform.localScale = new Vector3(1, 1, 1);
        blockedPlayerPanel.GetComponent<BlockedPlayerPanel>().SetUpPanel(blockedUser);
    }

    private void DestroyBlockedChildren()
    {
        foreach (GameObject child in blockedPlayers)
        {
            Destroy(child);
        }
        blockedPlayers.Clear();
    }
}
