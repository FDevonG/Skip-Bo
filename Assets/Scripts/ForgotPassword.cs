using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ForgotPassword : MonoBehaviour {
    public GameObject emailInput;
    public Button sendEmailButton;

    public void ForgotPass() {
        StartCoroutine(SendPasswordEmail());
    }

    private IEnumerator SendPasswordEmail() {
        var task = FirebaseAuthentication.ForgotPassword(emailInput.GetComponent<InputField>().text);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
        } else {
            EmailSent();
        }
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
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().logInPanel);
    }

    private void OnDisable() {
        emailInput.GetComponent<InputField>().text = "";
        sendEmailButton.interactable = false;
    }

}
