using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentUser : MonoBehaviour
{
    public static User curUser;
    public static CurrentUser Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
}
