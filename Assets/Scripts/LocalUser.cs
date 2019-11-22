using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalUser
{
    public static User locUser;

    public static IEnumerator LoadUser() {
        var userTask = FireBaseScript.GetCurrentUser();
        yield return new WaitUntil(() => userTask.IsCompleted);
        locUser = JsonUtility.FromJson<User>(userTask.Result);//this is the local user
    }
}
