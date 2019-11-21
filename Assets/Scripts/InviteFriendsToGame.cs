using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InviteFriendsToGame : MonoBehaviour
{

    [SerializeField] GameObject friendsPanelParent;

    private void OnEnable() {
        BuildFriendsList();
    }

    private void BuildFriendsList() {
        DestroyFriendPanels();
        for (int i = 0; i < PhotonNetwork.Friends.Count; i++) {
            if (PhotonNetwork.Friends[i].IsOnline) {
                Debug.Log("Instantiate friends here");
            }
        }
    }

    private void DestroyFriendPanels() {
        if (friendsPanelParent.transform.childCount > 0) {
            for (int i = 0; i < friendsPanelParent.transform.childCount; i++) {
                Destroy(friendsPanelParent.transform.GetChild(0).gameObject);
            }
        }
    }
}
