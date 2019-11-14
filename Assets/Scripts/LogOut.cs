using UnityEngine;

public class LogOut : MonoBehaviour
{
    public void LogOutOfGame() {
        if (FireBaseScript.IsPlayerAnonymous()) {
            FireBaseScript.DeleteAccountData();
        }
        LocalUser.user = null;
        FireBaseScript.SignOut();
    }
}
