using UnityEngine;

public class LogOut : MonoBehaviour
{
    public void LogOutOfGame() {
        LocalUser.locUser = null;
        if (FirebaseAuthentication.IsPlayerAnonymous()) {
            Database.DeleteAccountData();
            FirebaseAuthentication.DeleteAccount();
        }
        PhotonNetwork.Disconnect();
        Chat.Instance.Disconnect();
        FirebaseAuthentication.SignOut();
        StartCoroutine(AdManager.Instance.ShowBannerAdd());
        RemoveAds.instance.HideButton();
    }
}
