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

    public static IEnumerator AddFriend(string UserID) {
        if (!string.IsNullOrEmpty(UserID) && !string.IsNullOrWhiteSpace(UserID)) {
            LocalUser.locUser.friends.Add(UserID);
            var usersTask = FireBaseScript.GetUsers();
            var addFriendTask = FireBaseScript.UpdateUser("friends", LocalUser.locUser.friends);
            yield return new WaitUntil(() => addFriendTask.IsCompleted && usersTask.IsCompleted);
            if (addFriendTask.IsFaulted || usersTask.IsFaulted) {
                yield return "Failed to add friend";
            } else {
                foreach (DataSnapshot snap in usersTask.Result.Children) {
                    User tempUser = JsonUtility.FromJson<User>(snap.GetRawJsonValue());
                    if (tempUser.userID == UserID) {
                        friends.Add(tempUser);
                    }
                }
                string[] addedFriend = new string[1];
                addedFriend[0] = UserID;
                GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().AddFriend(addedFriend);
                yield return "Friend added";
            }
        } else {
            yield return "Failed to add friend";
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
        FireBaseScript.UpdateUser("friends", LocalUser.locUser.friends);
    }

    public static void BlockFriend(string userID) {
        if (!IsPlayerAlreadyBlocked(userID)) {
            LocalUser.locUser.blocked.Add(userID);
            FireBaseScript.UpdateUser("blocked", LocalUser.locUser.blocked);
        }
    }

    public static bool IsPlayerAlreadyBlocked(string userID) {
        bool alreadyBlocked = false;
        foreach (string block in LocalUser.locUser.blocked) {
            if (block == userID) {
                alreadyBlocked = true;
                break;
            }
        }
        return alreadyBlocked;
    }

    public static void UnblockPlayer(string userId) {
        LocalUser.locUser.blocked.Remove(userId);
        FireBaseScript.UpdateUser("blocked", LocalUser.locUser.blocked);
    }

    public static bool FriendAlreadyAdded(string userID) {
        bool friendAlreadyAdded = false;
        for (int i = 0; i < LocalUser.locUser.friends.Count; i++) {
            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrWhiteSpace(userID)) {
                if (LocalUser.locUser.friends[i].ToUpper() == userID.ToUpper()) {
                    friendAlreadyAdded = true;
                }
            }
        }
        return friendAlreadyAdded;
    }

    public static bool AmIBlocked(User friend) {
        bool blocked = false;
        foreach (string block in friend.blocked) {
            if (block == LocalUser.locUser.userID) {
                blocked = true;
                break;
            }
        }
        return blocked;
    }
}
