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
        if (task.IsFaulted) {
            loadingCircle.SetActive(false);
            loadingText.SetActive(false);
            GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(task.Exception));
        } else {
            LocalUser.locUser = new User("", FirebaseAuthentication.AuthenitcationKey(), GameObject.FindGameObjectWithTag("AchievementManager").GetComponent<Achievments>().BuildAchievmentsList());
            var newUserTask = Database.WriteNewUser(LocalUser.locUser);
            yield return new WaitUntil(() => newUserTask.IsCompleted);
            if (newUserTask.IsFaulted) {
                loadingCircle.SetActive(false);
                loadingText.SetActive(false);
                GetComponent<ErrorText>().SetError(FirebaseError.GetErrorMessage(newUserTask.Exception));
            } else {
                GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<RemoveAds>().AdsCheck();
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
            }
        }
    }

    private void OnDisable()
    {
        loadingCircle.SetActive(false);
        loadingText.SetActive(false);
        GetComponent<ErrorText>().ClearError();
    }
}
