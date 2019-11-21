using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public class Freinds : MonoBehaviour
{

    [SerializeField] GameObject friendsPanelParent;

    private void OnEnable() {
        StartCoroutine(BuildFriends());
    }

    private IEnumerator BuildFriends() {
        DeleteFriends();
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
        Debug.Log(userFriendsArray);
        //FriendPanel
        PhotonNetwork.FindFriends(userFriendsArray);
        Debug.Log(PhotonNetwork.Friends);
        if (PhotonNetwork.Friends != null) {
            for (int i = 0; i < PhotonNetwork.Friends.Count; i++) {
                User friend = new User();
                for (int x =0; x < friends.Count; x++) {
                    if (friends[x].userID == PhotonNetwork.Friends[i].UserId) {
                        Debug.Log(PhotonNetwork.Friends[i].IsOnline);
                        friend = friends[x];
                        friends.Remove(friends[x]);
                        break;
                    }
                }
                if (PhotonNetwork.Friends[i].IsOnline) {
                    SpawnFriendPanel(friend, true);
                } else {
                    SpawnFriendPanel(friend, false);
                }
            }
        }
        for (int i = 0; i < friends.Count; i++) {
            SpawnFriendPanel(friends[i], false);
        }
    }

    void SpawnFriendPanel(User user, bool status) {
        GameObject friendPanel = Instantiate(Resources.Load<GameObject>("FriendPanel"), friendsPanelParent.transform);
        friendPanel.transform.localScale = new Vector3(1, 1, 1);
        friendPanel.GetComponent<FriendListInfoPanel>().SetUpFriendPanel(user, status);
    }

    private void DeleteFriends() {
        for (int i = 0; i < friendsPanelParent.transform.childCount; i++) {
            Destroy(friendsPanelParent.transform.GetChild(0).gameObject);
        }
    }

}
