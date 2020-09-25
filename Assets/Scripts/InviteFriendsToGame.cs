using UnityEngine;
using UnityEngine.UI;

public class InviteFriendsToGame : MonoBehaviour {

    [SerializeField] GameObject friendsPanelParent;
    [SerializeField] Text infoText;

    private void OnEnable() {
        BuildFriendsList();
    }
    
    private void OnDisable() {
        infoText.gameObject.SetActive(false);
    }

    private void BuildFriendsList() {
        DestroyFriendPanels();
        for (int i = 0; i < Chat.friendsOnline.Count; i++) {
            User friend = new User();
            for (int x = 0; x < Friends.friends.Count; x++) {
                if (Friends.friends[x].userID == Chat.friendsOnline[i]) {
                    friend = Friends.friends[x];
                    break;
                }
            }
            if (!Friends.AmIBlocked(friend)) {
                GameObject playerTab = Instantiate(Resources.Load<GameObject>("FriendInvitePanel"), friendsPanelParent.transform);
                playerTab.transform.localScale = new Vector3(1,1,1);
                playerTab.GetComponent<FriendInvitePanel>().emailText.text = friend.userName;
                playerTab.GetComponent<FriendInvitePanel>().inviteButton.onClick.AddListener(() => {
                    GameObject.FindGameObjectWithTag("AchievementManager").GetComponent<Achievments>().UnlockAchievement("Better with friends");
                    GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().SendGameInvite(friend.userID, PhotonNetwork.room.Name + "@" + LocalUser.locUser.userName);
                    SetInfoText(friend.userName + " has been invited");
                    playerTab.GetComponent<FriendInvitePanel>().inviteButton.interactable = false;
                });
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

    private void SetInfoText(string message) {
        infoText.gameObject.SetActive(true);
        infoText.text = message;
    }
}
