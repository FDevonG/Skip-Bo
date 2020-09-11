using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanel : MonoBehaviour
{
    public Button signUpNewButton;
    public GameObject signUpEmailInput;
    public GameObject signUpPasswordInput;
    public GameObject signUpPasswordConfirmInput;
    private bool passwordsMatch = false;

    private void OnDisable() {
        passwordsMatch = false;
        signUpNewButton.interactable = false;
        signUpEmailInput.GetComponent<InputField>().text = "";
        signUpPasswordInput.GetComponent<InputField>().text = "";
        signUpPasswordConfirmInput.GetComponent<InputField>().text = "";
        signUpPasswordConfirmInput.GetComponentInChildren<Text>().color = Colors.GetBaseColor();
        GetComponent<ErrorText>().ClearError();
    }

    public void CreateNewAccount() {
        StartCoroutine(CreatingNewAccount());
    }

    private IEnumerator CreatingNewAccount() {
        GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOnLoadingScreen();
        var task = FirebaseAuthentication.CreateNewAccount(signUpEmailInput.GetComponent<InputField>().text, signUpPasswordInput.GetComponent<InputField>().text);
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
            GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
        } else {
            User newUser = new User(signUpEmailInput.GetComponent<InputField>().text, FirebaseAuthentication.AuthenitcationKey(), GameObject.FindGameObjectWithTag("AchievementManager").GetComponent<Achievments>().BuildAchievmentsList());
            var newUserTask = Database.WriteNewUser(newUser);
            yield return new WaitUntil(() => newUserTask.IsCompleted);
            if (newUserTask.IsFaulted) {
                GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(newUserTask.Exception));
            } else {
                LocalUser.locUser = newUser;
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
            }
            GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
        }
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
            signUpPasswordConfirmInput.GetComponentInChildren<Text>().color = Colors.GetBaseColor();
            GetComponent<ErrorText>().ClearError();
            passwordsMatch = true;
        } else {
            signUpPasswordConfirmInput.GetComponentInChildren<Text>().color = Color.red;
            GetComponent<ErrorText>().SetError("Passwords do not match");
            passwordsMatch = false;
        }
    }
}
