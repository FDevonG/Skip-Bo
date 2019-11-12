using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public Text infoText;
    public GameObject emailInput;
    public GameObject passwordInput;
    public Button loginButton;

    private void OnDisable() {
        infoText.gameObject.SetActive(false);
        emailInput.GetComponent<InputField>().text = "";
        passwordInput.GetComponent<InputField>().text = "";
        loginButton.interactable = false;
    }

    public void LogIn() {
        StartCoroutine(FireBaseScript.LogIn(emailInput.GetComponent<InputField>().text, passwordInput.GetComponent<InputField>().text));
    }

    public void SetLoginInfoText(string message) {
        if (infoText.gameObject.active == false) {
            infoText.gameObject.SetActive(true);
        }
        infoText.text = message;
    }

    public void LoginButtonCheck() {
        if (VerifyEmail.ValidateEmail(emailInput.GetComponent<InputField>().text) && !string.IsNullOrEmpty(passwordInput.GetComponent<InputField>().text) && !string.IsNullOrWhiteSpace(passwordInput.GetComponent<InputField>().text)) {
            loginButton.interactable = true;
        } else {
            loginButton.interactable = false;
        }
    }

}
