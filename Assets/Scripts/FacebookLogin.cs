using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookLogin : MonoBehaviour
{

    public void FacebookLoginPressed()
    {
        GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOnLoadingScreen();
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
            GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
        }
    }

    IEnumerator FacebookFirebaseLogin(AccessToken aToken)
    {
        var task = FirebaseAuthentication.LoginWithFacebook(aToken);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsCanceled)
        {
            GetComponent<ErrorText>().SetError("Login was canceled");
            GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
            yield return null;
        }
        if (task.IsFaulted)
        {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
            GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
            yield return null;
        }

        Firebase.Auth.FirebaseUser newUser = task.Result;
        FB.API("/me?fields=email", HttpMethod.GET, GetFacebookInfo);
        
    }

    public void GetFacebookInfo(IResult result)
    {
        if (result.Error == null)
        {

            //Debug.Log(result.ResultDictionary["id"].ToString());
            //Debug.Log(result.ResultDictionary["name"].ToString());
            //Debug.Log(result.ResultDictionary["email"].ToString());
            StartCoroutine(DoesPlayerExist(result));
        }
        else
        {
            GetComponent<ErrorText>().SetError(result.Error);
            GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
        }
    }

    IEnumerator DoesPlayerExist(IResult result)
    {
        var task = Database.GetCurrentUser();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted)
        {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
            GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
            yield return null;
        }
        if (task.Result == null)
        {
            StartCoroutine(SaveNewPlayer(result));
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
                StartCoroutine(Friends.GetStartFriends());
                PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, LocalUser.locUser);
                GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton();
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
            }
            GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
        }
    }

    IEnumerator SaveNewPlayer(IResult result)
    {
        User newUser = new User(result.ResultDictionary["email"].ToString(), FirebaseAuthentication.AuthenitcationKey(), GameObject.FindGameObjectWithTag("AchievementManager").GetComponent<Achievments>().BuildAchievmentsList());
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
        GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
    }
}
