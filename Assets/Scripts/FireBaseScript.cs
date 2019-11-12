using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;

public class FireBaseScript : MonoBehaviour
{
    public static FireBaseScript Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public static IEnumerator LogIn(string email, string password) {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.Exception != null) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().logInPanel.GetComponent<Login>().SetLoginInfoText(registerTask.Exception.Message);
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
        }
    }

    public static IEnumerator LogInAnonymous() {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.Exception != null) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().logInPanel.GetComponent<Login>().SetLoginInfoText(registerTask.Exception.Message);
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
        }

    }

    public static IEnumerator CreateNewAccount(string email, string password) {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.Exception != null) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().signUpPanel.GetComponent<SignUpPanel>().ChangeSignUpErrorText(registerTask.Exception.Message);
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
        }
    }

    public static IEnumerator ForgotPassword(string emailAddress) {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.SendPasswordResetEmailAsync(emailAddress);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.Exception != null) {

        }
    }

    public static void SignOut() {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    public static bool IsPlayerLoggedIn() {
        bool loggedIn = true;
        if (FirebaseAuth.DefaultInstance.CurrentUser == null) {
            loggedIn = false;
        }
        return loggedIn;
    }

}
