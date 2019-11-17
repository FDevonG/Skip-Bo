using UnityEngine;

public class LogOut : MonoBehaviour
{
    public void LogOutOfGame() {
        if (FireBaseScript.IsPlayerAnonymous()) {
            FireBaseScript.DeleteAccountData();
            FireBaseScript.DeleteAccount();
        }
        PhotonNetwork.Disconnect();
        FireBaseScript.SignOut();
    }
}
