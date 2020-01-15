using UnityEngine;
using System.Collections;

public class GuestLogin : MonoBehaviour
{
    public void LogInAnonymously() {
        StartCoroutine(LoginInAnon());
    }

    private IEnumerator LoginInAnon() {
        var task = FirebaseAuthentication.LogInAnonymous();
        yield return new WaitUntil(() => task.IsCompleted);
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
