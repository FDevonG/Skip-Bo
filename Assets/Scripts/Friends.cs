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
}
