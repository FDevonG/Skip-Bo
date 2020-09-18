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
            FB.Init();
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

}
