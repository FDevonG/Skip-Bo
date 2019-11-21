using UnityEngine;

public class LogOut : MonoBehaviour
{
    public void LogOutOfGame() {
        if (FireBaseScript.IsPlayerAnonymous()) {
            FireBaseScript.DeleteAccountData();
            FireBaseScript.DeleteAccount();
        }
        PhotonNetwork.Disconnect();
        GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().Disconnect();
        FireBaseScript.SignOut();
    }
}
