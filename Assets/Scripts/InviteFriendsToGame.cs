using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class InviteFriendsToGame : MonoBehaviour {

    [SerializeField] GameObject friendsPanelParent;
    [SerializeField] Text infoText;

    private void OnEnable() {
        BuildFriendsList();
    }

    private void OnDisable() {
        infoText.gameObject.SetActive(false);
    }

    private IEnumerator BuildFriendsList() {
        DestroyFriendPanels();
        var userTask = FireBaseScript.GetCurrentUser();
        var usersTask = FireBaseScript.GetUsers();
        yield return new WaitUntil(() => userTask.IsCompleted && usersTask.IsCompleted);
        User user = JsonUtility.FromJson<User>(userTask.Result);//this is the local user
        List<User> friends = new List<User>();//this is an array to add the friends to to pass to the next step
        foreach (DataSnapshot snap in usersTask.Result.Children) {//we are looping through the reseult passed back from the server to get the users
            User snapUser = JsonUtility.FromJson<User>(snap.GetRawJsonValue());//we convert each piece of data into a user to compare to the friends list
            foreach (string id in user.friends) {//we loop through the list of friends
                if (snapUser.userID == id) {//if the users id matchs the id on the friends list then 
                    friends.Add(snapUser);//we add it to the list of friends
                    break;
                }
            }
        }

        //here we are converting the list of friends into an array so we can pass it to the PhotonNetworkfindfriends function
        string[] userFriendsArray = new string[user.friends.Count];
        for (int i = 0; i < user.friends.Count; i++) {
            userFriendsArray[i] = user.friends[i];
        }
        //FriendPanel
        PhotonNetwork.FindFriends(userFriendsArray);
        if (PhotonNetwork.Friends != null) {
            for (int i = 0; i < PhotonNetwork.Friends.Count; i++) {
                User friend = new User();
                for (int x = 0; x < friends.Count; x++) {
                    if (friends[x].userID == PhotonNetwork.Friends[i].UserId) {
                        friend = friends[x];
                        friends.Remove(friends[x]);
                        break;
                    }
                }
                if (PhotonNetwork.Friends[i].IsOnline) {
                    GameObject button = Instantiate(Resources.Load<GameObject>("Button"), friendsPanelParent.transform);
                    button.transform.GetChild(0).GetComponent<Text>().text = friend.userName;
                    button.transform.localScale = new Vector3(1,1,1);
                    button.GetComponent<Button>().onClick.AddListener(() => GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().SendGameInvite(friend.userID, PhotonNetwork.room.Name + "@" + friend.userName));
                } 
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
