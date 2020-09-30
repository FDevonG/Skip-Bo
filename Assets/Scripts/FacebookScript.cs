using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookScript : MonoBehaviour
{
    public static FacebookScript Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Couldn't initialize");
            });
        }
        else
        {
            FB.ActivateApp();
        }
    }

    public bool IsFacebookLoggedIn()
    {
        if (FB.IsLoggedIn)
            return true;
        else
            return false;
    }

    public void ShareScreenShotToFacebook()
    {
        //FB.s
    }

    public void InviteFacebookFriendToGame()
    {
        FB.AppRequest("Come play some SkipBo!", title:"SkipBo");
    }

    public Dictionary<string, object> GetFriendsPlayingThisGame()
    {
        //"/me/friends"
        var friendsList = new Dictionary<string, object>();
        FB.API("/me/friends", HttpMethod.GET, result =>
        {
            friendsList = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            //friendsList = (List<object>)dictionary["data"];
        });
        return friendsList;
    }

}
