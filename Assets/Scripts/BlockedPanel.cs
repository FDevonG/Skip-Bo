using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockedPanel : MonoBehaviour
{
    [SerializeField] GameObject blockedPanelsParent;
    public static List<GameObject> blockedPlayers = new List<GameObject>();

    private void OnEnable()
    {
        LoadingScreen.Instance.TurnOnLoadingScreen();
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
            var task = BackendFunctions.GetUsers(LocalUser.locUser.blocked);
            yield return new WaitUntil(() => task.IsCompleted);
            if (!task.IsFaulted)
            {
                string[] strArr;
                strArr = task.Result.Split('#');

                foreach (string str in strArr)
                {
                    if (!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str))
                    {
                        //User friend = JsonUtility.FromJson<User>(str);
                        SpawnBlockedPanel(JsonUtility.FromJson<User>(str));
                    }
                }
            }
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
