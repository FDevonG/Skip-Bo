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
            GetCurrentUser();
            if (string.IsNullOrEmpty(CurrentUser.curUser.userName)) {
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
            } else {
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
            }
        }
    }

    public static IEnumerator LogInAnonymous() {
        var auth = FirebaseAuth.DefaultInstance;
        var registerTask = FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync();
        yield return new WaitUntil(() => registerTask.IsCompleted);
        if (registerTask.Exception != null) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().logInPanel.GetComponent<Login>().SetLoginInfoText(GetErrorMessage(registerTask.Exception));
        } else {
            User newUser = new User();
            WriteNewUser(newUser);
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
            newUser.BuildStartingStatsDictionary();//build the stats dictionary for new character
            WriteNewUser(newUser);
            UpdatePlayerStats(newUser.playerStats);
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

    #endregion
    //GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel.GetComponent<CharacterCreation>().ErrorWithCharacterEdit("Failed to save to server");
    #region Database

    public static void WriteNewUser(User user) {
        string json = JsonUtility.ToJson(user);
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Push().SetRawJsonValueAsync(json);
    }

    public static void UpdateUserAvatar(Dictionary<string, string> avatar) {
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("avatar").SetValueAsync(avatar);
    }

    public static void UpdateUserName(string name) {
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("userName").SetValueAsync(name);
    }

    public static void UpdatePlayerStats(Dictionary<string, int> stats) {
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("playerStats").SetValueAsync(stats);
    }

    #endregion

    public static bool IsPlayerLoggedIn() {
        bool loggedIn = true;
        if (FirebaseAuth.DefaultInstance.CurrentUser == null) {
            loggedIn = false;
        }
        return loggedIn;
    }

    public static bool IsPlayerAnonymous() {
        if (FirebaseAuth.DefaultInstance.CurrentUser.IsAnonymous) {
            return true;
        } else {
            return false;
        }
    }

    public static IEnumerator GetCurrentUser() {
        var auth = FirebaseAuth.DefaultInstance;
        var task = FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        Debug.Log("Geting CurrentPlayer");
        if (task.Exception != null) {
            Debug.Log("Failed to get current user");
        } else {
            DataSnapshot snapshot = task.Result;
            string json = snapshot.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).ToString();
            CurrentUser.curUser = JsonUtility.FromJson<User>(json);
            Debug.Log(CurrentUser.curUser.avatar.Keys);
        }
    }

    public static bool DoesUserNameExist(string username) {
        bool userNameExist = false;
        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                List<User> users = JsonUtility.FromJson<List<User>>(snapshot.ToString());
                for (int i = 0; i < users.Count; i++) {
                    if (users[i].userName == username) {
                        userNameExist = true;
                        break;
                    }
                }
            }
        });
        return userNameExist;
    }

}
