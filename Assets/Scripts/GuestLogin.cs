using UnityEngine;
using System.Collections;

public class GuestLogin : MonoBehaviour
{
    public void LogInAnonymously() {
        StartCoroutine(LoginInAnon());
    }

    private IEnumerator LoginInAnon() {
        GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOnLoadingScreen();
        var task = FirebaseAuthentication.LogInAnonymous();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
        } else {
            LocalUser.locUser = new User("", FirebaseAuthentication.AuthenitcationKey(), GameObject.FindGameObjectWithTag("AchievementManager").GetComponent<Achievments>().BuildAchievmentsList());
            var newUserTask = Database.WriteNewUser(LocalUser.locUser);
            yield return new WaitUntil(() => newUserTask.IsCompleted);
            if (newUserTask.IsFaulted) {
                GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(newUserTask.Exception));
            } else {
                GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<RemoveAds>().AdsCheck();
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
            }
        }
        GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<LoadingScreen>().TurnOffLoadingScreen();
    }

    private void OnDisable()
    {
        GetComponent<ErrorText>().ClearError();
    }
}
