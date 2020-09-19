using UnityEngine;
using UnityEngine.UI;

public class ButtonOnOffControl : MonoBehaviour
{
    [SerializeField] GameObject friendButon;
    [SerializeField] GameObject leaderBoadButtun;
    [SerializeField] GameObject inviteFacebookFriendsButton;

    private void OnEnable() {
        if (FirebaseAuthentication.IsPlayerAnonymous()) {
            friendButon.GetComponent<Button>().interactable = false;
            leaderBoadButtun.GetComponent<Button>().interactable = false;
        } else {
            friendButon.GetComponent<Button>().interactable = true;
            leaderBoadButtun.GetComponent<Button>().interactable = true;
        }
        if (FacebookScript.Instance.IsFacebookLoggedIn())
        {
            inviteFacebookFriendsButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            inviteFacebookFriendsButton.GetComponent<Button>().interactable = false;
        }
    }

}
