using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GooglePlayServices : MonoBehaviour
{

    public static GooglePlayServices Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Authenticate();
    }

    void Authenticate() {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    public void SignIn() {
        Social.localUser.Authenticate((bool success) => {
            if (success) {
                
            } else {
                Debug.Log("Failed to sign into google play servcies");
            }
        });
    }

    public static void SignOut() {
        PlayGamesPlatform.Instance.SignOut();
    }

    public static bool IsGooglePlayLoggedIn() {
        return Social.localUser.authenticated;
    }
}
