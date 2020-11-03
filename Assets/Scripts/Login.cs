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
        LoadingScreen.Instance.TurnOnLoadingScreen();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            LoadingScreen.Instance.TurnOffLoadingScreen();
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
        } else {
            var userTask = Database.GetCurrentUser();
            yield return new WaitUntil(() => userTask.IsCompleted);
            if (!userTask.IsFaulted) {
                LocalUser.locUser = JsonUtility.FromJson<User>(userTask.Result);
                RemoveAds.instance.AdsCheck();
                if (string.IsNullOrEmpty(LocalUser.locUser.userName) || string.IsNullOrWhiteSpace(LocalUser.locUser.userName)) {
                    LoadingScreen.Instance.TurnOffLoadingScreen();
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
                } else {
                    PhotonPlayerSetup.BuildPhotonPlayer(PhotonNetwork.player, LocalUser.locUser);
                    PhotonNetworking.Instance.ConnectToPhoton();
                    LoadingScreen.Instance.TurnOffLoadingScreen();
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
                }
            } else {
                GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(userTask.Exception));
                LoadingScreen.Instance.TurnOffLoadingScreen();
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
