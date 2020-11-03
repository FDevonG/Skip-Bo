using UnityEngine;
using System.Collections;

public class GuestLogin : MonoBehaviour
{
    public void LogInAnonymously() {
        StartCoroutine(LoginInAnon());
    }

    private IEnumerator LoginInAnon() {
        LoadingScreen.Instance.TurnOnLoadingScreen();
        var task = FirebaseAuthentication.LogInAnonymous();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
        } else {
            LocalUser.locUser = new User(FirebaseAuthentication.AuthenitcationKey(), Achievments.Instance.BuildAchievmentsList());
            var newUserTask = Database.WriteNewUser(LocalUser.locUser);
            yield return new WaitUntil(() => newUserTask.IsCompleted);
            if (newUserTask.IsFaulted) {
                GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(newUserTask.Exception));
            } else {
                RemoveAds.instance.AdsCheck();
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
            }
        }
        LoadingScreen.Instance.TurnOffLoadingScreen();
    }

    private void OnDisable()
    {
        GetComponent<ErrorText>().ClearError();
    }
}
