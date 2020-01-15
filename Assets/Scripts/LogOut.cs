using UnityEngine;

public class LogOut : MonoBehaviour
{
    public void LogOutOfGame() {
        if (FirebaseAuthentication.IsPlayerAnonymous()) {
            Database.DeleteAccountData();
            FirebaseAuthentication.DeleteAccount();
        }
        PhotonNetwork.Disconnect();
        GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().Disconnect();
        FirebaseAuthentication.SignOut();
    }
}
