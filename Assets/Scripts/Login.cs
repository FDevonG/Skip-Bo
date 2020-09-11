using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Login : MonoBehaviour {

    public GameObject emailInput;
    public GameObject passwordInput;
    public Button loginButton;

    private void OnDisable() {
        emailInput.GetComponent<InputField>().text = "";
        passwordInput.GetComponent<InputField>().text = "";
        loginButton.interactable = false;
        GetComponent<ErrorText>().ClearError();
    }

    public void LogInButtonPressed() {
        StartCoroutine(LogIn());
    }

    private IEnumerator LogIn() {
        var task = FirebaseAuthentication.LogIn(emailInput.GetComponent<InputField>().text, passwordInput.GetComponent<InputField>().text);
        GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOnLoadingScreen();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
        } else {
            var userTask = Database.GetCurrentUser();
            yield return new WaitUntil(() => userTask.IsCompleted);
            //User user = new User();
            if (!userTask.IsFaulted) {
                LocalUser.locUser = JsonUtility.FromJson<User>(userTask.Result);
                GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<RemoveAds>().AdsCheck();
                if (string.IsNullOrEmpty(LocalUser.locUser.userName) || string.IsNullOrWhiteSpace(LocalUser.locUser.userName)) {
                    GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
                } else {
                    StartCoroutine(Friends.GetStartFriends());
                    PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, LocalUser.locUser);
                    GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton();
                    GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
                }
            } else {
                GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(userTask.Exception));
                GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
            }            
        }
    }

    public void LoginButtonCheck() {
        if (VerifyEmail.ValidateEmail(emailInput.GetComponent<InputField>().text) && !string.IsNullOrEmpty(passwordInput.GetComponent<InputField>().text) && !string.IsNullOrWhiteSpace(passwordInput.GetComponent<InputField>().text)) {
            loginButton.interactable = true;
        } else {
            loginButton.interactable = false;
        }
    }

}
