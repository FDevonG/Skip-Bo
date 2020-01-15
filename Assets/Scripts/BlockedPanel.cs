using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class BlockedPanel : MonoBehaviour
{
    [SerializeField] GameObject blockedPanelsParent;
    public static List<GameObject> blockedPlayers = new List<GameObject>();

    private void OnEnable() {
        StartCoroutine(BuildBlockedList());
    }

    private void OnDisable() {
        DestroyBlockedChildren();
    }

    private IEnumerator BuildBlockedList() {
        if (LocalUser.locUser.blocked.Count <= 0) {
            yield return null;
        }
        var userTask = FireBaseScript.GetUsers();
        yield return new WaitUntil(() => userTask.IsCompleted);
        if (!userTask.IsFaulted) {
            foreach (string blocked in LocalUser.locUser.blocked) {
                foreach (DataSnapshot snapshot in userTask.Result.Children) {
                    User tempUser = JsonUtility.FromJson<User>(snapshot.GetRawJsonValue());
                    if (tempUser.userID == blocked) {
                        SpawnBlockedPanel(tempUser);
                        break;
                    }
                }
            }
        }
    }

    private void SpawnBlockedPanel(User blockedUser) {
        GameObject blockedPlayerPanel = Instantiate(Resources.Load<GameObject>("BlockedPlayerPanel"), blockedPanelsParent.transform);
        blockedPlayerPanel.transform.localScale = new Vector3(1,1,1);
        blockedPlayerPanel.GetComponent<BlockedPlayerPanel>().SetUpPanel(blockedUser);
    }

    private void DestroyBlockedChildren() {
        foreach (GameObject child in blockedPlayers) {
            Destroy(child);
        }
        blockedPlayers.Clear();
    }
}
