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
            PlayerData data = SaveSystem.LoadPlayer();
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().TurnOnCanvas();
            if (success) {
                if (data == null) {
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().characterCreationPanel);
                } else {
                    GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().startMenu);
                }
            } else {
                GameObject.FindGameObjectWithTag("GameManager").GetComponent<ActivatePanel>().SwitchPanel(GameObject.FindGameObjectWithTag("GameManager").GetComponent<Menu>().failedToLogInPanel);
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
