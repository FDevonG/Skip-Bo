using UnityEngine;
using UnityEngine.UI;

public class FriendsButton : MonoBehaviour
{
    [SerializeField] GameObject friendButon;

    private void OnEnable() {
        if (FirebaseAuthentication.IsPlayerAnonymous()) {
            friendButon.GetComponent<Button>().interactable = false;
        } else {
            friendButon.GetComponent<Button>().interactable = true;
        }
    }

}
