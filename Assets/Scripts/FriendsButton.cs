using UnityEngine;
using UnityEngine.UI;

public class FriendsButton : MonoBehaviour
{
    [SerializeField] GameObject friendButon;
    [SerializeField] GameObject leaderBoadButtun;

    private void OnEnable() {
        if (FirebaseAuthentication.IsPlayerAnonymous()) {
            friendButon.GetComponent<Button>().interactable = false;
            leaderBoadButtun.GetComponent<Button>().interactable = false;
        } else {
            friendButon.GetComponent<Button>().interactable = true;
            leaderBoadButtun.GetComponent<Button>().interactable = true;
        }
    }

}
