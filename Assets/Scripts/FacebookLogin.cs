using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookLogin : MonoBehaviour
{

    public void FacebookLoginPressed()
    {
        LoadingScreen.Instance.TurnOnLoadingScreen();
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, OnFacebookLoginResult);
    }

    private void OnFacebookLoginResult(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            StartCoroutine(FacebookFirebaseLogin(AccessToken.CurrentAccessToken));
        }
        else
        {
            GetComponent<ErrorText>().SetError("Failed to sign in");
            LoadingScreen.Instance.TurnOffLoadingScreen();
        }
    }

    IEnumerator FacebookFirebaseLogin(AccessToken aToken)
    {
        var task = FirebaseAuthentication.LoginWithFacebook(aToken);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsCanceled)
        {
            GetComponent<ErrorText>().SetError("Login was canceled");
            LoadingScreen.Instance.TurnOffLoadingScreen();
            yield return null;
        }
        if (task.IsFaulted)
        {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
            LoadingScreen.Instance.TurnOffLoadingScreen();
            yield return null;
        }

        StartCoroutine(DoesPlayerExist());

        //Firebase.Auth.FirebaseUser newUser = task.Result;
        //FB.API("/me?fields=email", HttpMethod.GET, GetFacebookInfo);
        
    }

    //public void GetFacebookInfo(IResult result)
    //{
    //    if (result.Error == null)
    //    {

    //        //Debug.Log(result.ResultDictionary["id"].ToString());
    //        //Debug.Log(result.ResultDictionary["name"].ToString());
    //        //Debug.Log(result.ResultDictionary["email"].ToString());
    //        StartCoroutine();
    //    }
    //    else
    //    {
    //        GetComponent<ErrorText>().SetError(result.Error);
    //        LoadingScreen.Instance.TurnOffLoadingScreen();
    //    }
    //}

    IEnumerator DoesPlayerExist()
    {
        var task = Database.GetCurrentUser();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted)
        {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
            LoadingScreen.Instance.TurnOffLoadingScreen();
            yield return null;
        }
        if (task.Result == null)
        {
            StartCoroutine(SaveNewPlayer());
        } else
        {
            LocalUser.locUser = JsonUtility.FromJson<User>(task.Result);
            //GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<RemoveAds>().AdsCheck();
            if (string.IsNullOrEmpty(LocalUser.locUser.userName) || string.IsNullOrWhiteSpace(LocalUser.locUser.userName))
            {
                //GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
            }
            else
            {
                PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, LocalUser.locUser);
                PhotonNetworking.Instance.ConnectToPhoton();
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
            }
            LoadingScreen.Instance.TurnOffLoadingScreen();
        }
    }

    IEnumerator SaveNewPlayer()
    {
        User newUser = new User(FirebaseAuthentication.AuthenitcationKey(), Achievments.Instance.BuildAchievmentsList());
        var newUserTask = Database.WriteNewUser(newUser);
        yield return new WaitUntil(() => newUserTask.IsCompleted);
        if (newUserTask.IsFaulted)
        {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(newUserTask.Exception));
        }
        else
        {
            LocalUser.locUser = newUser;
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
        }
        LoadingScreen.Instance.TurnOffLoadingScreen();
    }
}
