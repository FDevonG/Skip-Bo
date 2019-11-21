using System.Collections;
using System.Collections.Generic;
using Firebase.Unity.Editor;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Threading.Tasks;

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

    public static string AuthenitcationKey() {
        return FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    #region Authentiation
    public static Task<FirebaseUser> LogIn(string email, string password) {
        return FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            return task.Result;
        });
    }

    public static Task<FirebaseUser> LogInAnonymous() {
        return FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task => {
            return task.Result;
        });
    }

    public static Task<FirebaseUser> CreateNewAccount(string email, string password) {
        return FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            return task.Result;
        });
    }

    public static Task<bool> ForgotPassword(string emailAddress) {
        return FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(emailAddress).ContinueWith(task => {
            return task.IsFaulted;
        });
    }

    public static void SignOut() {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    public static void DeleteAccount() {
        FirebaseAuth.DefaultInstance.CurrentUser.DeleteAsync();
    }

    #endregion
    
    #region Database

    public static void WriteNewUser(User user) {
        string json = JsonUtility.ToJson(user);
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("users").Child(AuthenitcationKey()).SetRawJsonValueAsync(json);
    }

    public static void UpdateUser(User user) {
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        databaseReference.Child("users").Child(AuthenitcationKey()).Child("userName").SetValueAsync(user.userName);
        databaseReference.Child("users").Child(AuthenitcationKey()).Child("hair").SetValueAsync(user.hair);
        databaseReference.Child("users").Child(AuthenitcationKey()).Child("face").SetValueAsync(user.face);
        databaseReference.Child("users").Child(AuthenitcationKey()).Child("kit").SetValueAsync(user.kit);
        databaseReference.Child("users").Child(AuthenitcationKey()).Child("body").SetValueAsync(user.body);
        databaseReference.Child("users").Child(AuthenitcationKey()).Child("offlineGamesPlayed").SetValueAsync(user.offlineGamesPlayed);
        databaseReference.Child("users").Child(AuthenitcationKey()).Child("onlineGamesPlayed").SetValueAsync(user.onlineGamesPlayed);
        databaseReference.Child("users").Child(AuthenitcationKey()).Child("offlineGamesWon").SetValueAsync(user.offlineGamesWon);
        databaseReference.Child("users").Child(AuthenitcationKey()).Child("onlineGamesWon").SetValueAsync(user.onlineGamesWon);
        databaseReference.Child("users").Child(AuthenitcationKey()).Child("friends").SetValueAsync(user.friends);
    }

    public static void DeleteAccountData() {
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(AuthenitcationKey());
        databaseReference.RemoveValueAsync();
    }

    public static Task<string> GetCurrentUser() {
        return FirebaseDatabase.DefaultInstance.GetReference("users").Child(AuthenitcationKey()).GetValueAsync().ContinueWith(task => {
            return task.Result.GetRawJsonValue();
        });
    }

    public static Task<DataSnapshot> GetUsers() {
        return FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWith(task => {
            return task.Result;
        });
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

}
