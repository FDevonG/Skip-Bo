using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanel : MonoBehaviour
{
    public Button signUpNewButton;
    public GameObject signUpEmailInput;
    public GameObject signUpPasswordInput;
    public GameObject signUpPasswordConfirmInput;
    public Text passwordText;
    public Text signUpErrorText;
    private bool passwordsMatch = false;

    private void OnDisable() {
        passwordsMatch = false;
        signUpNewButton.interactable = false;
        signUpErrorText.gameObject.SetActive(false);
        signUpEmailInput.GetComponent<InputField>().text = "";
        signUpPasswordInput.GetComponent<InputField>().text = "";
        signUpPasswordConfirmInput.GetComponent<InputField>().text = "";
        passwordText.gameObject.SetActive(false);
    }

    public void CreateNewAccount() {
        StartCoroutine(FireBaseScript.CreateNewAccount(signUpEmailInput.GetComponent<InputField>().text, signUpPasswordInput.GetComponent<InputField>().text));
    }

    public void CheckSignUpButton() {
        if (ArePasswordsFilledOut() && passwordsMatch && VerifyEmail.ValidateEmail(signUpEmailInput.GetComponent<InputField>().text)) {
            signUpNewButton.interactable = true;
        } else {
            signUpNewButton.interactable = false;
        }
    }

    private bool ArePasswordsFilledOut() {
        string passOne = signUpPasswordInput.GetComponent<InputField>().text;
        string passTwo = signUpPasswordConfirmInput.GetComponent<InputField>().text;
        if (!string.IsNullOrEmpty(passOne) && !string.IsNullOrWhiteSpace(passOne) && !string.IsNullOrEmpty(passTwo) && !string.IsNullOrWhiteSpace(passTwo)) {
            return true;
        } else {
            return false;
        }
    }

    public void DoPasswordsMatch() {
        if (signUpPasswordInput.GetComponent<InputField>().text == signUpPasswordConfirmInput.GetComponent<InputField>().text) {
            signUpPasswordConfirmInput.GetComponent<Outline>().enabled = false;
            passwordText.gameObject.SetActive(false);
            passwordsMatch = true;
        } else {
            signUpPasswordConfirmInput.GetComponent<Outline>().enabled = true;
            passwordText.gameObject.SetActive(true);
            passwordsMatch = false;
        }
    }

    public void ChangeSignUpErrorText(string message) {
        if (signUpErrorText.gameObject.active == false) {
            signUpErrorText.gameObject.SetActive(true);
        }
        signUpErrorText.text = message;
    }
}
