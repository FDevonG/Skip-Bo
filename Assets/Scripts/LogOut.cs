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
        GameObject.FindGameObjectWithTag("Chat").GetComponent<Chat>().Disconnect();
        FirebaseAuthentication.SignOut();
        StartCoroutine(GameObject.FindGameObjectWithTag("AdManager").GetComponent<AdManager>().ShowBannerAdd());
        GameObject.FindGameObjectWithTag("RemoveAdsPanel").GetComponent<RemoveAds>().HideButton();
    }
}
