using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ForgotPassword : MonoBehaviour {
    public GameObject emailInput;
    public Text infoText;
    public Button sendEmailButton;

    public void ForgotPass() {
        StartCoroutine(SendPasswordEmail());
    }

    private IEnumerator SendPasswordEmail() {
        var task = FireBaseScript.ForgotPassword(emailInput.GetComponent<InputField>().text);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().forgotPasswordPanel.GetComponent<ForgotPassword>().SetInfoText(FireBaseScript.GetErrorMessage(task.Exception));
        } else {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().forgotPasswordPanel.GetComponent<ForgotPassword>().EmailSent();
        }
    }

    public void SetInfoText(string message) {
        infoText.gameObject.SetActive(true);
        infoText.text = message;
    }

    public void ForgotPasswordEmail() {
        if (VerifyEmail.ValidateEmail(emailInput.GetComponent<InputField>().text)) {
            sendEmailButton.interactable = true;
        } else {
            sendEmailButton.interactable = false;
        }
    }

    public void SendEmailButton() {
        if (VerifyEmail.ValidateEmail(emailInput.GetComponent<InputField>().text)) {
            sendEmailButton.interactable = true;
        } else {
            sendEmailButton.interactable = false;
        }
    }

    public void EmailSent() {
        sendEmailButton.interactable = false;
        SetInfoText("Email has been sent to " + emailInput.GetComponent<InputField>().text);
        emailInput.GetComponent<InputField>().text = "";
    }

    private void OnDisable() {
        infoText.gameObject.SetActive(false);
        emailInput.GetComponent<InputField>().text = "";
        sendEmailButton.interactable = false;
    }

}
