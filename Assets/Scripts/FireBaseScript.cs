using System.Collections;
using System.Collections.Generic;
using Firebase.Unity.Editor;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
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

    private void Start() {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://skip-bo-52535022.firebaseio.com/");
        
    }

    #region Authentiation
    public static IEnumerator LogIn(string email, string password) {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.Exception != null) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().logInPanel.GetComponent<Login>().SetLoginInfoText(GetErrorMessage(registerTask.Exception));
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
        }
    }

    public static IEnumerator LogInAnonymous() {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.Exception != null) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().logInPanel.GetComponent<Login>().SetLoginInfoText(GetErrorMessage(registerTask.Exception));
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
        }

    }

    public static IEnumerator CreateNewAccount(string email, string password) {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.Exception != null) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().signUpPanel.GetComponent<SignUpPanel>().ChangeSignUpErrorText(GetErrorMessage(registerTask.Exception));
        } else {
            User newUser = new User(email, FirebaseAuth.DefaultInstance.CurrentUser.UserId);
            WriteNewUser(newUser);
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
        }
    }

    public static IEnumerator ForgotPassword(string emailAddress) {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        var registerTask = auth.SendPasswordResetEmailAsync(emailAddress);
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.Exception != null) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().forgotPasswordPanel.GetComponent<ForgotPassword>().SetInfoText(GetErrorMessage(registerTask.Exception));
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().forgotPasswordPanel.GetComponent<ForgotPassword>().EmailSent();
        }
    }

    public static string GetErrorMessage(Exception exception) {
        Firebase.FirebaseException firebaseEx = exception.GetBaseException() as Firebase.FirebaseException;
        if (firebaseEx != null) {
            var errorCode = (AuthError)firebaseEx.ErrorCode;
            return GetErrorMessage(errorCode);
        } else {
            return "An error has occured";
        }
    }

    private static string GetErrorMessage(AuthError errorCode) {
        var message = "";
        switch (errorCode) {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Account exists with different credentials";
                break;
            case AuthError.MissingPassword:
                message = "Password is missing";
                break;
            case AuthError.WeakPassword:
                message = "Password is to weak";
                break;
            case AuthError.WrongPassword:
                message = "Password is incorrect";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Email is already in use";
                break;
            case AuthError.InvalidEmail:
                message = "Email is not valid";
                break;
            case AuthError.MissingEmail:
                message = "Email is missing";
                break;
            default:
                message = "An error has occured";
                break;
        }
        return message;
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
    #endregion

    #region Database

    public static void WriteNewUser(User user) {
        string json = JsonUtility.ToJson(user);
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("users").Child(user.UserID).SetRawJsonValueAsync(json);
    }

    public static IEnumerator GetUsers() {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync();
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null) {
            Debug.Log(registerTask.Result);
            Debug.Log("Getting Users went wrong");
        } else {
            Debug.Log(registerTask.Result);
            Debug.Log("Getting Users");
        }
    }

    

    #endregion

}
