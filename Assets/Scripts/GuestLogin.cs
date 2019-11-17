using UnityEngine;
using System.Collections;

public class GuestLogin : MonoBehaviour
{
    public void LogInAnonymously() {
        StartCoroutine(LoginInAnon());
    }

    private IEnumerator LoginInAnon() {
        var task = FireBaseScript.LogInAnonymous();
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().logInPanel.GetComponent<Login>().SetLoginInfoText(FireBaseScript.GetErrorMessage(task.Exception));
        } else {
            User newUser = new User("", FireBaseScript.AuthenitcationKey());
            FireBaseScript.WriteNewUser(newUser);
            StartCoroutine(GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PhotonNetworking>().ConnectToPhoton());
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
        }
    }
}
