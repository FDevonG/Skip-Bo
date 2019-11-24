using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

public static class Friends
{
    public static List<User> friends = new List<User>();

    public static IEnumerator GetStartFriends() {
        var usersTask = FireBaseScript.GetUsers();
        yield return new WaitUntil(() => usersTask.IsCompleted);
        foreach (DataSnapshot snap in usersTask.Result.Children) {//we are looping through the reseult passed back from the server to get the users
            User snapUser = JsonUtility.FromJson<User>(snap.GetRawJsonValue());//we convert each piece of data into a user to compare to the friends list
            foreach (string id in LocalUser.locUser.friends) {//we loop through the list of friends
                if (snapUser.userID == id) {//if the users id matchs the id on the friends list then 
                    friends.Add(snapUser);//we add it to the list of friends
                }
            }
        }
    }

    public static void DeleteFriend(User friend) {

        foreach (User friendUser in friends) {
            if (friendUser == friend) {
                friends.Remove(friendUser);
                break;
            }
        }

        for (int i = 0; i < LocalUser.locUser.friends.Count; i++) {
            if (LocalUser.locUser.friends[i] == friend.userID) {
                string[] removedFriend = new string[1];
                removedFriend[0] = LocalUser.locUser.friends[i];
                GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().DeleteFriends(removedFriend);
                LocalUser.locUser.friends.Remove(LocalUser.locUser.friends[i]);
                break;
            }
        }
        PhotonNetwork.Friends = null;
        if (LocalUser.locUser.friends.Count > 0) {
            PhotonNetwork.FindFriends(LocalUser.locUser.friends.ToArray());
        }
        FireBaseScript.UpdateUser("friends", LocalUser.locUser.friends);
    }

    public static void BlockFriend(User friend) {
        LocalUser.locUser.blocked.Add(friend.userID);
        FireBaseScript.UpdateUser("blocked", LocalUser.locUser.blocked);
    }
}
