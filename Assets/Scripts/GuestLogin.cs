using UnityEngine;

public class GuestLogin : MonoBehaviour
{
    public void LogInAnonymously() {
        StartCoroutine(FireBaseScript.LogInAnonymous());
    }
}
