using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

public class FirebaseAuthentication : MonoBehaviour
{
    public static string AuthenitcationKey() {
        return FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

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
}
