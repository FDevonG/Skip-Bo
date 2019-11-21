using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    public void LogInButtonPressed() {
        StartCoroutine(LogIn());
    }

    private IEnumerator LogIn() {
        var task = FireBaseScript.LogIn(emailInput.GetComponent<InputField>().text, passwordInput.GetComponent<InputField>().text);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().logInPanel.GetComponent<Login>().SetLoginInfoText(FireBaseScript.GetErrorMessage(task.Exception));
        } else {
            var userTask = FireBaseScript.GetCurrentUser();
            yield return new WaitUntil(() => task.IsCompleted);
            User user = new User();
            if (!task.IsFaulted) {
                user = JsonUtility.FromJson<User>(userTask.Result);
            }
            //StartCoroutine(GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton());
            if (string.IsNullOrEmpty(user.userName)) {
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
            } else {
                StartCoroutine(GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton());
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
            }
        }
    }

    public void SetLoginInfoText(string message) {
        infoText.gameObject.SetActive(true);
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
