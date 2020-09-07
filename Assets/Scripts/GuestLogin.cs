using UnityEngine;
using System.Collections;

public class GuestLogin : MonoBehaviour
{

    [SerializeField] GameObject loadingCircle;
    [SerializeField] GameObject loadingText;

    public void LogInAnonymously() {
        StartCoroutine(LoginInAnon());
    }

    private IEnumerator LoginInAnon() {
        loadingCircle.SetActive(true);
        loadingText.SetActive(true);
        var task = FirebaseAuthentication.LogInAnonymous();
        yield return new WaitUntil(() => task.IsCompleted);
        loadingCircle.SetActive(false);
        loadingText.SetActive(false);
        if (task.IsFaulted) {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
        } else {
            User newUser = new User("", FirebaseAuthentication.AuthenitcationKey(), GameObject.FindGameObjectWithTag("AchievementManager").GetComponent<Achievments>().BuildAchievmentsList());
            var newUserTask = Database.WriteNewUser(newUser);
            yield return new WaitUntil(() => newUserTask.IsCompleted);
            if (newUserTask.IsFaulted) {
                GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(newUserTask.Exception));
            } else {
                LocalUser.locUser = newUser;
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
            }
        }
    }
}
