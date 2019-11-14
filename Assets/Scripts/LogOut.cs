using UnityEngine;

public class LogOut : MonoBehaviour
{
    public void LogOutOfGame() {
        if (FireBaseScript.IsPlayerAnonymous()) {
            FireBaseScript.DeleteAccount();
        }
        FireBaseScript.SignOut();
    }
}
