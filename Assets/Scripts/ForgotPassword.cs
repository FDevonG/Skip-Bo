using UnityEngine;
using UnityEngine.UI;

public class ForgotPassword : MonoBehaviour
{
    public GameObject emailInput;
    public Text infoText;
    public Button sendEmailButton;

    public void ForgotPass() {
        StartCoroutine(FireBaseScript.ForgotPassword(emailInput.GetComponent<InputField>().text));
    }

    public void SetInfoText(string message) {
        if (infoText.gameObject.active == false) {
            infoText.gameObject.SetActive(true);
        }
        infoText.text = message;
    }

    public void ForgotPasswordEmail() {
        if (VerifyEmail.ValidateEmail(emailInput.GetComponent<InputField>().text)) {
            sendEmailButton.interactable = true;
        } else {
            sendEmailButton.interactable = false;
        }
    }

    private void OnDisable() {
        infoText.gameObject.SetActive(false);
        emailInput.GetComponent<InputField>().text = "";
        sendEmailButton.interactable = false;
    }

}
