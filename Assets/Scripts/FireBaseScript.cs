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
            Debug.Log(registerTask.Exception);
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Login>().SetLoginInfoText("Please En");
        }
    }

    public static void LogInAnonymous() {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task => {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
        });
    }

    public static void CreateNewAccount(string email, string password) {
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(password)) {
            FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                
            });
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

    public static string GetErrorMessage(Exception exception) {
        Debug.Log(exception.ToString());
        Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
        if (firebaseEx != null) {
            var errorCode = (AuthError)firebaseEx.ErrorCode;
            return GetErrorMessage(errorCode);
        }

        return exception.ToString();
    }

    private static string GetErrorMessage(AuthError errorCode) {
        var message = "";
        switch (errorCode) {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Ya existe la cuenta con credenciales diferentes";
                break;
            case AuthError.MissingPassword:
                message = "Hace falta el Password";
                break;
            case AuthError.WeakPassword:
                message = "El password es debil";
                break;
            case AuthError.WrongPassword:
                message = "El password es Incorrecto";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "Ya existe la cuenta con ese correo electrónico";
                break;
            case AuthError.InvalidEmail:
                message = "Correo electronico invalido";
                break;
            case AuthError.MissingEmail:
                message = "Hace falta el correo electrónico";
                break;
            default:
                message = "Ocurrió un error";
                break;
        }
        return message;
    }

}
