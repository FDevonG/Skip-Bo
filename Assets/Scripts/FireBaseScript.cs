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
            var task = FirebaseCloudFunctions.GetUser();
            User user = new User();
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.IsFaulted) {
                Debug.Log("Failed to load User");
            } else {
                user = JsonUtility.FromJson<User>(task.Result);
            }
            //while (LocalUser.user == null) {
            //    yield return new WaitForSeconds(0.2f);
            //    GetCurrentUser();
            //}
            if (string.IsNullOrEmpty(user.userName)) {
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
            User newUser = new User("GuestLogin", FirebaseAuth.DefaultInstance.CurrentUser.UserId);
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

    public static void DeleteAccount() {
        FirebaseAuth.DefaultInstance.CurrentUser.DeleteAsync();
    }

    #endregion
    //GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel.GetComponent<CharacterCreation>().ErrorWithCharacterEdit("Failed to save to server");
    #region Database

    public static void WriteNewUser(User user) {
        string json = JsonUtility.ToJson(user);
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).SetRawJsonValueAsync(json);
    }

    public static void UpdateUser(User user) {
        string json = JsonUtility.ToJson(user);
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("userName").SetValueAsync(user.userName);
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("hair").SetValueAsync(user.hair);
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("face").SetValueAsync(user.face);
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("kit").SetValueAsync(user.kit);
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("body").SetValueAsync(user.body);
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("offlineGamesPlayed").SetValueAsync(user.offlineGamesPlayed);
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("onlineGamesPlayed").SetValueAsync(user.onlineGamesPlayed);
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("offlineGamesWon").SetValueAsync(user.offlineGamesWon);
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("onlineGamesWon").SetValueAsync(user.onlineGamesWon);
        databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("friends").SetValueAsync(user.friends);
    }

    public static void DeleteAccountData() {
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        databaseReference.RemoveValueAsync();
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

    //public static void GetCurrentUser() {
    //    FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task => {
    //        if (task.IsFaulted) {
    //            Debug.Log("Failed Getting Character");
    //        } else if (task.IsCompleted) {
    //            DataSnapshot snapshot = task.Result;
    //            string json = snapshot.Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).GetRawJsonValue();
    //            LocalUser.user = JsonUtility.FromJson<User>(json);
    //        }
    //    });
    //}

    public static IEnumerator DoesUserNameExist() {
        var auth = FirebaseAuth.DefaultInstance;
        var task = FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.Exception == null) {
            DataSnapshot snapshot = task.Result;
            bool userNameExists = false;
            foreach (DataSnapshot snap in snapshot.Children) {
                User user = JsonUtility.FromJson<User>(snap.GetRawJsonValue());
                if (user.userName.ToUpper() == LocalUser.user.userName.ToUpper()) {
                    if (FirebaseAuth.DefaultInstance.CurrentUser.UserId != user.userID) {
                        userNameExists = true;
                        break;
                    }
                }
            }
            
            if (userNameExists) {
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel.GetComponent<CharacterCreation>().ErrorWithCharacterEdit("Username is taken");
            } else {
                FireBaseScript.UpdateUser(LocalUser.user);
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
            }
        }
    }

}
