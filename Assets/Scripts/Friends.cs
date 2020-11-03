using System.Collections;
using UnityEngine;

public static class Friends
{
    public static IEnumerator AddFriend(string UserID)
    {
        if (!string.IsNullOrEmpty(UserID) && !string.IsNullOrWhiteSpace(UserID))
        {
            LocalUser.locUser.friends.Add(UserID);
            var addFriendTask = Database.UpdateUser("friends", LocalUser.locUser.friends);
            yield return new WaitUntil(() => addFriendTask.IsCompleted);
            if (addFriendTask.IsFaulted)
            {
                yield return "Failed to add friend";
            }
            else
            {
                var getUserTask = BackendFunctions.GetUser(UserID);
                yield return new WaitUntil(() => getUserTask.IsCompleted);
                if (!getUserTask.IsFaulted)
                {
                    string user = getUserTask.Result;
                    User tempUser = JsonUtility.FromJson<User>(user);
                    string[] addedFriend = new string[1];
                    addedFriend[0] = UserID;
                    Chat.Instance.AddFriend(addedFriend);
                    Achievments.Instance.FreindAdded();
                    yield return "Friend added";
                }
            }
        }
        else
        {
            yield return "Failed to add friend";
        }
    }

    public static void DeleteFriend(User friend)
    {
        for (int i = 0; i < LocalUser.locUser.friends.Count; i++)
        {
            if (LocalUser.locUser.friends[i] == friend.userID)
            {
                string[] removedFriend = new string[1];
                removedFriend[0] = LocalUser.locUser.friends[i];
                Chat.Instance.DeleteFriends(removedFriend);
                LocalUser.locUser.friends.Remove(LocalUser.locUser.friends[i]);
                break;
            }
        }
        Database.UpdateUser("friends", LocalUser.locUser.friends);
    }

    public static void BlockFriend(string userID)
    {
        if (!IsPlayerBlocked(userID))
        {
            LocalUser.locUser.blocked.Add(userID);
            Database.UpdateUser("blocked", LocalUser.locUser.blocked);
        }
    }

    public static bool IsPlayerBlocked(string userID)
    {
        bool alreadyBlocked = false;
        foreach (string block in LocalUser.locUser.blocked)
        {
            if (block == userID)
            {
                alreadyBlocked = true;
                break;
            }
        }
        return alreadyBlocked;
    }

    public static void UnblockPlayer(string userId)
    {
        LocalUser.locUser.blocked.Remove(userId);
        Database.UpdateUser("blocked", LocalUser.locUser.blocked);
    }

    public static bool FriendAlreadyAdded(string userID)
    {
        bool friendAlreadyAdded = false;
        for (int i = 0; i < LocalUser.locUser.friends.Count; i++)
        {
            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrWhiteSpace(userID))
            {
                if (LocalUser.locUser.friends[i].ToUpper() == userID.ToUpper())
                {
                    friendAlreadyAdded = true;
                }
            }
        }
        return friendAlreadyAdded;
    }

    public static bool AmIBlocked(User friend)
    {
        bool blocked = false;
        foreach (string block in friend.blocked)
        {
            if (block == LocalUser.locUser.userID)
            {
                blocked = true;
                break;
            }
        }
        return blocked;
    }
}
