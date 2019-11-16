using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsButton : MonoBehaviour
{
    [SerializeField] GameObject friendButon;

    private void OnEnable() {
        if (FireBaseScript.IsPlayerAnonymous()) {
            friendButon.SetActive(false);
        } else {
            friendButon.SetActive(true);
        }
    }

}
