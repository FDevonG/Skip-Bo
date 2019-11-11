using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public Text infoText;
    public GameObject emailInput;
    public GameObject passwordInput;

    public Button loginButton;
    Button CreateNewButton;

    public void LogIn() {
        FireBaseScript.LogIn(emailInput.GetComponent<InputField>().text, passwordInput.GetComponent<InputField>().text);
    }

    public void LogInAnonymously() {
        FireBaseScript.LogInAnonymous();
    }

    public void CreateNewAccount() {
        //FireBaseScript.CreateNewAccount(emailInput.GetComponent<InputField>().text, passwordInput.GetComponent<InputField>().text);
    }

    //public bool DoPasswordsMatch() {
    //    if (!string.IsNullOrEmpty(passwordInputOne.GetComponent<InputField>().text) && !string.IsNullOrEmpty(passwordInputTwo.GetComponent<InputField>().text)) {
    //        if (passwordInputOne.GetComponent<InputField>().text == passwordInputTwo.GetComponent<InputField>().text) {
    //            return true;
    //        } else {
    //            return false;
    //        }
    //    } else {
    //        return false;
    //    }
    //}

    public void LoginButtonCheck() {
        if (ValidateEmail(emailInput.GetComponent<InputField>().text) && !string.IsNullOrEmpty(passwordInput.GetComponent<InputField>().text) && !string.IsNullOrWhiteSpace(passwordInput.GetComponent<InputField>().text)) {
            loginButton.interactable = true;
        } else {
            loginButton.interactable = false;
        }
    }

    private bool ValidateEmail(string email) {
        string[] atCharacter;
        string[] dotCharacter;
        atCharacter = email.Split("@"[0]);
        if (atCharacter.Length == 2) {
            dotCharacter = atCharacter[1].Split("."[0]);
            if (dotCharacter.Length >= 2) {
                if (dotCharacter[dotCharacter.Length - 1].Length == 0) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    public void SetLoginInfoText (string message) {
        infoText.text = message;
        Debug.Log("Hi");
    }
}
