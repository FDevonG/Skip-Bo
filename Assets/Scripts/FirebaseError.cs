using System;
using Firebase.Auth;
using UnityEngine;

public class FirebaseError : MonoBehaviour
{
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
